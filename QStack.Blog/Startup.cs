using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QStack.Framework.Persistent.EFCore;
using QStack.Framework.Basic.Services.Auth;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.Model.Auth;
using AutoMapper;
using AspectCore.Extensions.DependencyInjection;
using QStack.Framework.Basic.ViewModel.Auth;
using System.Reflection;
using QStack.Framework.Persistent.EFCore.External;
using QStack.Framework.Core.Persistent;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using QStack.Blog.Comon;
using System.Linq;
using QStack.Framework.Core.Cache;
using QStack.Blog.Middleware;
using QStack.Framework.Util;
using QStack.Framework.Core;
using System.IO;
using QStack.Blog.Filters;
using QStack.Framework.SearchEngine.Extensions;
using QStack.Framework.SearchEngine.Imps;
using QStack.Framework.Basic.Model;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Mvc.Controllers;
using QStack.Framework.AspNetCore.Plugin.Extensions;
using QStack.Framework.AspNetCore.Plugin.ViewModels;
using QStack.Framework.HangFire;
using Savorboard.CAP.InMemoryMessageQueue;
using QStack.Framework.Core.MessageQueue;
using QStack.Framework.AspNetCore.Plugin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;


//using System.Net.Http;

namespace QStack.Blog
{
    public class Startup
    {
        //public static readonly ILoggerFactory MyLoggerFactory
        //    = LoggerFactory.Create(builder => { builder.AddConsole(); });
        //public static readonly ILoggerFactory EFCoreLoggerFactory
        //       = LoggerFactory.Create(builder => { builder.AddProvider(new EFLoggerProvider()); });
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
         
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSession();
            services.AddCors(
               option => option.AddPolicy("cors",
               policy => policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
               //地址最后不要加斜杠“/”(((φ(◎ロ◎;)φ)))
               .WithOrigins(Configuration.GetSection("SiteSetting:CorsOrigin").Get<string[]>())));
            //services.AddControllersWithViews().AddControllersAsServices()
            //    .AddRazorRuntimeCompilation();
            services.AddMvc(options => options.Filters.Add<TitleFilter>())
                .AddRazorRuntimeCompilation()
#if DEBUG
                //.AddApplicationPart(typeof(Docker.Crawler.CrawlerOptions).Assembly)
#endif
                .AddControllersAsServices()
                .AddJsonOptions(options =>
                {
                    //options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                    options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                    options.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());
                    options.JsonSerializerOptions.Converters.Add(new DateTimeOffsetConverter());
                });

            var defaultActivator = services.FirstOrDefault(c => c.ServiceType == typeof(IControllerActivator));
            if (defaultActivator != null)
            {
                services.Remove(defaultActivator);
                services.AddSingleton<IControllerActivator, CustomServiceBasedControllerActivator>();
            }
           

            services.Configure<FileManagerOptions>(options => Configuration.GetSection("FileOptions").Bind(options));

            services.AddAutoMapper(
                configAction =>
                {
                    configAction.AddAutoMaperConfig(typeof(UserDto).Assembly, typeof(PluginInfoDto).Assembly);

                },
                new Assembly[] { });

            services.AddEFCore(Configuration, option =>
            {
                switch (option.FactoryName)
                {
                    case "sfdb":
                        {
                            option.EntityAssemblies.Add(typeof(User).Assembly);
                            option.EntityAssemblies.Add(typeof(PluginInfo).Assembly);
                            break;
                        }
                }
            });

#if DEBUG
            //services.AddServices(new Assembly[] { typeof(Docker.Crawler.CrawlerOptions).Assembly });
#endif
            services.AddServices(new Assembly[] { typeof(UserService).Assembly });


            JWTTokenOptions jwtTokenOptions = new JWTTokenOptions();
            services.AddSingleton<JWTTokenOptions>(provider => jwtTokenOptions);
            //cookies登陆
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o =>
                {
                    o.Cookie.Name = "qystack.top";
                    o.Cookie.HttpOnly = false;

                    //o.LoginPath = new PathString("/Home/Index");
                    //o.LogoutPath = new PathString("/Account/Login");
                    //开启跨域cookie
                    o.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
                })
                .AddJwtBearer(options =>
            {


                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // 是否验证颁发者
                    ValidateIssuer = true,
                    // 是否验证访问群体
                    ValidateAudience = true,
                    // 是否验证生存期
                    ValidateLifetime = true,
                    // 验证Token的时间偏移量
                    ClockSkew = TimeSpan.FromSeconds(30),
                    // 是否验证安全密钥
                    ValidateIssuerSigningKey = true,
                    // 访问群体
                    ValidAudience = jwtTokenOptions.Audience,
                    // 颁发者
                    ValidIssuer = jwtTokenOptions.Issuer,
                    // 安全密钥
                    IssuerSigningKey = jwtTokenOptions.Key,
                };

            });
            //.AddQQ(options =>
            //{
            //    options.ClientId = Configuration["OAuths:QQ:ClientId"];
            //    options.ClientSecret = Configuration["OAuths:QQ:ClientSecret"];
            //}); 

            services.AddCache(config => config.UseInMemory()); ;
            services.AddScoped<IEnviromentContext, EnviromentContext>();
            services.AddAutoMigration(options =>
            {
                options.MigrationPath = Path.Combine("app_data", "Migrations");
                options.BackupBasePath = Path.Combine("app_data", "MigrationsBackup");
#if DEBUG
                options.PgDumpPath = @"E:\Program Files\PostgreSQL\10\bin";
#endif
            });
            services.AddSearchEngine(new LuceneIndexerOptions()
            {
                Path = Path.Combine("app_data", "lucene_index")
            });
            services.AddSingleton<HtmlEncoder>(
                HtmlEncoder.Create(allowedRanges: new[] {
                    UnicodeRanges.All
                }));
            services.ConfigureDynamicProxy(
                config => config.AddInterceptor(Configuration)
            );

          
            services.PluginSetup(Configuration);
            services.AddHangFire(Configuration);
            services.AddCapWithRabbitMQ(Configuration, options => { options.UseDashboard(); });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddIpRateLimitings(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseIpRateLimitings();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            else
            {
                //app.UseMiddleware<ExceptionHandlerMiddleware>();
                //此处使用内部的requetDelegate流程
                app.UseExceptionHandler(newApp => newApp.UseMiddleware<ExceptionHandlerMiddleware>());
                //app.UseExceptionHandler();
                //app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

                app.UseStatusCodePages(context =>
                {
                    //StatusCodePage中间件默认只处理responsebody为空白的状态码
                    //此处通过抛出异常的方式让ExceptionHandler中间件去处理404
                    if (context.HttpContext.Response.StatusCode == 404)
                        throw new HttpException(context.HttpContext.Response.StatusCode);
                    return System.Threading.Tasks.Task.CompletedTask;
                });
            }

            app.UsePlugin();
            app.UseMiddleware<ResponseMeasurementMiddleware>();

            app.UseCors("cors");
            //app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {

                //静态文件跨域 支持tinymce图片编辑
                OnPrepareResponse = context =>
                 {
                     context.Context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                    //context.Context.Response.Headers.Remove("Content-Length");
                }
            }
            );
            app.UseSession();

            app.UseRouting();
            app.UseAuthentication();
            app.UseMiddleware<SlideCaptchaMiddleware>();
            app.UseMiddleware<AuthorizationMiddleware>();
            app.UseMiddleware<SerilogContextMiddleware>();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
               
                //endpoints.MapControllerRoute(
                // name: "tag",
                // pattern: "/tag/{id}.html"
                // );
                //endpoints.MapControllerRoute(
                // name: "article",
                // pattern: "/article/{id}.html"
                // );
                //endpoints.MapAreaControllerRoute(
                //  name: "areas", "api",
                //  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "areas",
               pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
               );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");



            });
            app.UserHangFire();

            app.UsePluginRoute();

            if (!Configuration.GetValue<bool>("HadMigration"))
            {
                //自动migration
                using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var autoMigration = scope.ServiceProvider.GetRequiredService<AutoMigration>();
                    autoMigration.GenerateMigrations();

                    var dbcontext = scope.ServiceProvider.GetServices<IDaoFactory>().FirstOrDefault(f => f.FactoryName == "sfdb")?.CreateDao();

                    if (!dbcontext?.DbSet<User>().Any() == true)
                    {
                        dbcontext?.AddOrUpdate<Group, int>(new Group { Name = "系统管理组", Code = "101" });
                        dbcontext?.AddOrUpdate<Role, int>(new Role { Name = "admin", Code = "101" });
                        dbcontext?.AddOrUpdate<User, int>(new User { Name = "admin", State = QStack.Framework.Basic.Enum.UserState.Active, PassWord = "++111111" });
                        dbcontext?.AddOrUpdate<UserRole, int>(new UserRole { UserId = 1, RoleId = 1 });

                        dbcontext?.Flush();
                    }


                }

                SettingsHelpers.AddOrUpdateAppSetting("HadMigration", true, Path.Combine("app_data", "config", $"appsettings.{ env.EnvironmentName}.json"));
            }
        }
    }
}
