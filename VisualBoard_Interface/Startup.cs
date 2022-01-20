using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MySql.Data.MySqlClient;
using Rokin.Common.MongoDB;
using Rokin.Common.RabbitMQ;
using Rokin.Common.Tools;
using Rokin.Dapper;
using Rokin.EFCore.VIPBI.VIP_BI;
using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using VisualBoard.Business.Interface;
using VisualBoard.Business.Service;
using VisualBoard.Models.Request;
using VisualBoard_Interface.Common;
using Rokin.Common.RabbitMQ.Interface;
using Rokin.Common.RabbitMQ.Business;

namespace VisualBoard_Interface
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //���ÿ�����
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policyBuilder =>
                {
                    policyBuilder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                    .AllowAnyOrigin();
                });
            });

            /*       services.Configure<KestrelServerOptions>(options =>
                   {
                       options.AllowSynchronousIO = true;
                   });*/
            //services.AddHostedService<MQCustomer>();
            services.AddControllers();

            services.AddDbContext<_WMS_VisualboardContext>();
            services.AddDbContext<_VIP_BIContext>();
            services.AddTransient<IB2CStatementBL, B2CStatementBL>();
            services.AddTransient<IUserBL, UserBL>();
            services.AddTransient<IOrganizationBL, OrganizationBL>();
            services.AddTransient<ICustomerBL, CustomerBL>();
            services.AddTransient<IOrderBL, OrderBL>();
            services.AddTransient<ILoginBL, LoginBL>();
            services.AddTransient<IWarehouseBL, WarehouseBL>();
            services.AddTransient<IPubBL, PubBL>();
            services.AddTransient<IDbConnection, MySqlConnection>(context => { return new MySqlConnection(DapperHelp.GetConnectionString(RokinConn_Enum.WMS_Visualboard)); });
            services.AddTransient<HttpHelperAsync>();
            services.AddTransient<IExpressrBL, ExpressrBL>();
            //services.AddSingleton<IMongoHelper, MongoHelper>(ops => { return new MongoHelper("WMSVisual", "OperInfo"); });
            //services.AddSingleton<PublishTools>(ops => { return new PublishTools(VirtualHost: "WMS"); });
            //services.AddSingleton<IRabbitMQHelper, RabbitMQHelper>(ops => { return new RabbitMQHelper(VirtualHost: "WMS"); });
            #region Jwt����
            //��appsettings.json�е�JwtSettings�����ļ���ȡ��JwtSettings�У����Ǹ������ط��õ�
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));

            //���ڳ�ʼ����ʱ�����Ǿ���Ҫ�ã�����ʹ��Bind�ķ�ʽ��ȡ����
            //�����ð󶨵�JwtSettingsʵ����
            var jwtSettings = new JwtSettings();
            Configuration.Bind("JwtSettings", jwtSettings);

            //��������֤
            services.AddAuthentication(options =>
            {
                //��֤middleware����
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                //jwt token��������
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = JwtClaimTypes.Name,
                    RoleClaimType = JwtClaimTypes.Role,
                    //Token�䷢����
                    ValidIssuer = jwtSettings.Issuer,
                    //�䷢��˭
                    ValidAudience = jwtSettings.Audience,
                    //�����keyҪ���м���
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

                    /***********************************TokenValidationParameters�Ĳ���Ĭ��ֵ***********************************/
                    // RequireSignedTokens = true,
                    // SaveSigninToken = false,
                    // ValidateActor = false,
                    // ������������������Ϊfalse�����Բ���֤Issuer��Audience�����ǲ�������������
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    // ValidateIssuerSigningKey = false,
                    // �Ƿ�Ҫ��Token��Claims�б������Expires
                    // RequireExpirationTime = true,
                    // ����ķ�����ʱ��ƫ����
                    ClockSkew = TimeSpan.FromSeconds(10),
                    // �Ƿ���֤Token��Ч�ڣ�ʹ�õ�ǰʱ����Token��Claims�е�NotBefore��Expires�Ա�
                    ValidateLifetime = true
                };
            });
            #endregion

            #region Swagger����
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���) ֱ�����¿�������Bearer {token}��ע������֮����һ���ո�\"",
                    Name = "Authorization",//jwtĬ�ϵĲ�������
                    In = ParameterLocation.Header,//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
                    Type = SecuritySchemeType.ApiKey
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement {

                { new OpenApiSecurityScheme
                {
                Reference = new OpenApiReference()
                {
                Id = "Bearer",
                Type = ReferenceType.SecurityScheme
                }
                }, Array.Empty<string>() }
                });

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "MsSystem API"
                });

                //Determine base path for the application.  
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                //Set the comments path for the swagger json and ui.  
                options.IncludeXmlComments(Path.Combine(basePath, "VisualBoard_Interface.xml"));
            });
            services.AddControllersWithViews(
                t =>
                {
                    t.Filters.Add<AuthorizationFilter>();
                    t.Filters.Add<ExceptionFilter>();
                })
                .AddJsonOptions(option =>
                {
                    option.JsonSerializerOptions.Converters.Add(new CoreDateTimeConverter());
                    option.JsonSerializerOptions.Converters.Add(new CoreIntConverter());
                    option.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                    option.JsonSerializerOptions.PropertyNamingPolicy = null;
                    //option.JsonSerializerOptions.Encoder = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                    // option.SerializerSettings.ContractResolver = new DefaultContractResolver();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use(next => new RequestDelegate(
          async context =>
          {
              context.Request.EnableBuffering();
              await next(context);
          }
      ));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MsSystem API V1");
            });
        }
    }
}
