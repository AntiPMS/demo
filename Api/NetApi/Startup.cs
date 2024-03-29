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

            #region 跨域
            services.AddCors(options =>
            options.AddPolicy("NetApiCors", p => p
                               .WithMethods("GET", "POST", "HEAD", "PUT", "DELETE", "OPTIONS")
                               .DisallowCredentials()
                               .AllowAnyHeader()
                               .AllowAnyOrigin()
                               ));
            #endregion

            #region 注册mysql数据库
            //ILoggerFactory sqlLogFactory = LoggerFactory.Create(m => m.AddConsole());//打印sql到console便于debug
            services.AddDbContext<NetApiContext>(
                op =>
                {
                    var serverVersion = new MySqlServerVersion("8.0.25");//new Version(8, 0, 25)
                    op.UseMySql(Configuration.GetConnectionString("NetApiConnection"), serverVersion);//等价于Configuration["ConnectionStrings:NetApiConnection"]
                    //op.UseLoggerFactory(sqlLogFactory);
                }
            );
            #endregion

            #region swagger配置
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NetApi", Version = "v1" });
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录
                var xmlPath = Path.Combine(basePath, "NetApi.xml");
                //SwaggerUI添加Header请求
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
                    Description = "请输入Token，格式为Bearer [token]",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.IncludeXmlComments(xmlPath); //配置注释指向的xml文件
            });
            #endregion

            #region 添加JWT
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

            //注册Jwt工具
            services.AddSingleton<IJwtAuthManager>(m => new JwtAuthManager(jwtTokenConfig));
            #endregion

            #region 添加websocket管理服务
            services.AddSingleton<IWebsocketManager, WebsocketManager>();
            #endregion

            #region 添加cache Redis配置
            var redisConfig = Configuration.GetSection("RedisConfig");
            services.AddSingleton(new RedisHelper(redisConfig["ConnectionString"], redisConfig["InstanceName"], Convert.ToInt32(redisConfig["DefaultDB"])));
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

            //启用swagger
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NetApi v1"));

            //http重定向到https协议
            //app.UseHttpsRedirection();

            //启用路由
            app.UseRouting();

            //授权验证
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            #region websocket配置
            app.UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
            });
            app.UseMiddleware<MyWebSocketMiddleware>();
            #endregion

            #region 建立必要的文件夹
            try
            {
                List<string> DirectorList = new List<string> {
                    Configuration["Appsettings:UploadFilesLocation"]//上传文件目录
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
            catch
            {
            }
            #endregion
        }
    }
}
