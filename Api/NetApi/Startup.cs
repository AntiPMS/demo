using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NetApi.Common;
using NetApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetApi
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }


        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            #region ����
            services.AddCors(options =>
            options.AddPolicy("NetApiCors", p => p
                               .WithMethods("GET", "POST", "HEAD", "PUT", "DELETE", "OPTIONS")
                               .DisallowCredentials()
                               .AllowAnyHeader()
                               .AllowAnyOrigin()
                               ));
            #endregion

            #region ע��mysql���ݿ�
            services.AddDbContext<NetApiContext>(
                op => op.UseMySql(Configuration.GetConnectionString("NetApiConnection"))//�ȼ���Configuration["ConnectionStrings:NetApiConnection"]
            );
            #endregion

            #region swagger����
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NetApi", Version = "v1" });
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//��ȡӦ�ó�������Ŀ¼
                var xmlPath = Path.Combine(basePath, "NetApi.xml");
                //SwaggerUI���Header����
                c.OperationFilter<ApiHeaderFilter>();
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference()
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        }, Array.Empty<string>()
                    }
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "������Token����ʽΪBearer [token]",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.IncludeXmlComments(xmlPath); //����ע��ָ���xml�ļ�
            });
            #endregion

            #region ���JWT
            var jwtTokenConfig = Configuration.GetSection("jwtTokenConfig").Get<JwtTokenConfig>();
            services.AddSingleton(jwtTokenConfig);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                //x.RequireHttpsMetadata = true; //https
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtTokenConfig.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)),
                    ValidAudience = jwtTokenConfig.Audience,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(jwtTokenConfig.RefreshTokenExpiration)
                };
            });

            //ע��Jwt����
            services.AddSingleton<IJwtAuthManager>(m => new JwtAuthManager(jwtTokenConfig));
            #endregion

            #region ���socket�������
            services.AddSingleton<IWebsocketManager, WebsocketManager>();
            #endregion

        }

        /// <summary>
        ///  This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //else if (env.IsProduction())
            //{

            //}

            //����swagger
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NetApi v1"));

            //http�ض���httpsЭ��
            //app.UseHttpsRedirection();

            //����·��
            app.UseRouting();

            //��Ȩ��֤
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            #region websocket����
            app.UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
            });
            app.UseMiddleware<MyWebSocketMiddleware>();
            #endregion

            #region ������Ҫ���ļ���
            try
            {
                List<string> DirectorList = new List<string> {
                    Configuration["Appsettings:UploadFilesLocation"]//�ϴ��ļ�Ŀ¼
                };
                DirectorList.ForEach(m =>
                {
                    if (!string.IsNullOrEmpty(m))
                    {
                        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, m);
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                    }
                });
            }
            catch (Exception e)
            {
            }
            #endregion
        }
    }
}
