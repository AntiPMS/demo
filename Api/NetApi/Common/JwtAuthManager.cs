using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NetApi.Common
{
    /// <summary>
    /// 
    /// </summary>
    public interface IJwtAuthManager
    {
        /// <summary>
        /// 
        /// </summary>
        IImmutableDictionary<string, RefreshToken> UsersRefreshTokensReadOnlyDictionary { get; }

        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        JwtAuthResult GenerateTokens(TokenUser user, Claim[] claims);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        JwtAuthResult Refresh(string refreshToken, string accessToken);

        /// <summary>
        ///  按时间清除所有token
        /// </summary>
        /// <param name="now">截止时间</param>
        void RemoveExpiredRefreshTokens(DateTime now);

        /// <summary>
        /// 按用户Id清除所有token
        /// </summary>
        /// <param name="userId">用户Id</param>
        void RemoveRefreshTokenByAccount(string userId);

        /// <summary>
        /// 解析token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token);
    }

    /// <summary>
    /// Jwt工具类
    /// </summary>
    public class JwtAuthManager : IJwtAuthManager
    {
        /// <summary>
        /// 
        /// </summary>
        public IImmutableDictionary<string, RefreshToken> UsersRefreshTokensReadOnlyDictionary => _usersRefreshTokens.ToImmutableDictionary();

        private readonly ConcurrentDictionary<string, RefreshToken> _usersRefreshTokens;  // can store in a database or a distributed cache
        private readonly JwtTokenConfig _jwtTokenConfig;
        private readonly byte[] _secret;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="jwtTokenConfig"></param>
        public JwtAuthManager(JwtTokenConfig jwtTokenConfig)
        {
            _jwtTokenConfig = jwtTokenConfig;
            _usersRefreshTokens = new ConcurrentDictionary<string, RefreshToken>();
            _secret = Encoding.ASCII.GetBytes(jwtTokenConfig.Secret);
        }

        /// <summary>
        ///  清除入参时间前的token
        /// </summary>
        /// <param name="now"></param>
        public void RemoveExpiredRefreshTokens(DateTime now)
        {
            var expiredTokens = _usersRefreshTokens.Where(x => x.Value.ExpireAt < now).ToList();
            foreach (var expiredToken in expiredTokens)
            {
                _usersRefreshTokens.TryRemove(expiredToken.Key, out _);
            }
        }

        /// <summary>
        /// 清除入参帐号的所有token
        /// </summary>
        /// <param name="userId"></param>
        public void RemoveRefreshTokenByAccount(string userId)
        {
            var refreshTokens = _usersRefreshTokens.Where(x => x.Value.User.Id == userId).ToList();
            foreach (var refreshToken in refreshTokens)
            {
                _usersRefreshTokens.TryRemove(refreshToken.Key, out _);
            }
        }

        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="User"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public JwtAuthResult GenerateTokens(TokenUser User, Claim[] claims)
        {
            var now = DateTime.Now;
            var shouldAddAudienceClaim = string.IsNullOrWhiteSpace(claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);
            var jwtToken = new JwtSecurityToken(
                _jwtTokenConfig.Issuer,
                shouldAddAudienceClaim ? _jwtTokenConfig.Audience : string.Empty,
                claims,
                expires: now.AddMinutes(_jwtTokenConfig.AccessTokenExpiration),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature));
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var refreshToken = new RefreshToken
            {
                User = User,
                TokenString = GenerateRefreshTokenString(),
                ExpireAt = now.AddMinutes(_jwtTokenConfig.RefreshTokenExpiration)
            };
            _usersRefreshTokens.AddOrUpdate(refreshToken.TokenString, refreshToken, (s, t) => refreshToken);

            return new JwtAuthResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        /// <summary>
        /// 获取二次授权token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public JwtAuthResult Refresh(string refreshToken, string accessToken)
        {
            var now = DateTime.Now;
            var (principal, jwtToken) = DecodeJwtToken(accessToken);
            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
            {
                throw new SecurityTokenException("Invalid token");
            }

            if (!_usersRefreshTokens.TryGetValue(refreshToken, out var existingRefreshToken))
            {
                throw new SecurityTokenException("Invalid token");
            }

            #region 验证解析后UserId的是否匹配缓存
            var decodeUserId = principal.FindFirst("Id").Value;
            if (existingRefreshToken.User.Id != decodeUserId || existingRefreshToken.ExpireAt < now)
            {
                throw new SecurityTokenException("Invalid token");
            }
            #endregion
            var tokenUser = new TokenUser()
            {
                Id = principal.FindFirst("Id").Value,
                Account = principal.FindFirst("Account").Value,
                Name = principal.FindFirst("Name").Value,
            };
            return GenerateTokens(tokenUser, principal.Claims.ToArray()); // need to recover the original claims
        }

        /// <summary>
        /// 解析token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new SecurityTokenException("非法的token");
            }
            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(token,
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = _jwtTokenConfig.Issuer,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(_secret),
                        ValidAudience = _jwtTokenConfig.Audience,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(_jwtTokenConfig.RefreshTokenExpiration)
                    },
                    out var validatedToken);
            return (principal, validatedToken as JwtSecurityToken);
        }

        private static string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TokenUser
    {
        public string Id { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        // 可以往后继续加字段进token
    }


    /// <summary>
    /// 
    /// </summary>
    public class RefreshToken
    {
        /// <summary>
        /// 自定义保存进token的字段:用户Id
        /// </summary>
        [JsonPropertyName("user")]
        public TokenUser User { get; set; }    // 便于验证时追踪token身份

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("tokenString")]
        public string TokenString { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("expireAt")]
        public DateTime ExpireAt { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class JwtAuthResult
    {
        /// <summary>
        /// 当前token
        /// </summary>
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        /// <summary>
        /// 二次请求刷新的token
        /// </summary>
        [JsonPropertyName("refreshToken")]
        public RefreshToken RefreshToken { get; set; }
    }

    /// <summary>
    /// Jwt配置类
    /// </summary>
    public class JwtTokenConfig
    {
        /// <summary>
        /// 密钥
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// 发行人
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 接收人
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// 当前toekn时效(分钟)
        /// </summary>
        public int AccessTokenExpiration { get; set; }

        /// <summary>
        /// 二次token时效(分钟)
        /// </summary>
        public int RefreshTokenExpiration { get; set; }
    }
}
