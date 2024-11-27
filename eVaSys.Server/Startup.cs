//using eValorplastWebApp.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace eValorplastWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            StaticConfig = configuration;
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public static IConfiguration StaticConfig { get; private set; }
        private IWebHostEnvironment CurrentEnvironment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Session
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(120);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore)
                .AddNewtonsoftJson(options => options.SerializerSettings.Formatting = Formatting.Indented)
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
                ;
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist/browser";
            });
            // Add EntityFramework
            services.AddEntityFrameworkSqlServer();

            //// Add ApplicationDbContext.
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"])
            //);
            //AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            // Add Authentication with JWT Tokens
            services.AddAuthentication(opts =>
            {
                opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    // standard configuration
                    ValidIssuer = Configuration["Auth:Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["Auth:Jwt:Key"])),
                    ValidAudience = Configuration["Auth:Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero,

                    // security switches
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true
                };
                cfg.IncludeErrorDetails = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                // Handles exceptions and generates a custom response body
                //app.UseExceptionHandler("/error/500");
                //// Handles non-success status codes with empty body
                //app.UseStatusCodePagesWithReExecute("/error/{0}");
            }

            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    OnPrepareResponse = (context) =>
            //    {
            //        // Disable caching for all static files. 
            //        context.Context.Response.Headers["Cache-Control"] =
            //            Configuration["StaticFiles:Headers:Cache-Control"];
            //        context.Context.Response.Headers["Pragma"] =
            //            Configuration["StaticFiles:Headers:Pragma"];
            //        context.Context.Response.Headers["Expires"] =
            //            Configuration["StaticFiles:Headers:Expires"];
            //    }
            //});
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                        Path.Combine(Directory.GetCurrentDirectory(), "Assets/Docs")),
                RequestPath = "/Assets/Docs"
            });
            //if (!env.IsDevelopment())
            //{
            //    app.UseSpaStaticFiles(new StaticFileOptions()
            //    {
            //        OnPrepareResponse = (context) =>
            //        {
            //            // Disable caching for all static files. 
            //            context.Context.Response.Headers["Cache-Control"] =
            //                    Configuration["StaticFiles:Headers:Cache-Control"];
            //            context.Context.Response.Headers["Pragma"] =
            //                Configuration["StaticFiles:Headers:Pragma"];
            //            context.Context.Response.Headers["Expires"] =
            //                Configuration["StaticFiles:Headers:Expires"];
            //        }
            //    });
            //}

            //App started event
            lifetime.ApplicationStarted.Register(OnAppStarted);

            //Session
            app.UseRouting();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");

            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    //spa.UseAngularCliServer(npmScript: "start");
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                }
            });
        }
        public void OnAppStarted()
        {
            //var connectionstring = Configuration["Data:DefaultConnection:ConnectionString"];

            //var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            //optionsBuilder.UseSqlServer(connectionstring);
            //using (ApplicationDbContext dbContext = new(optionsBuilder.Options))
            //{
            //    Utils.Utils.UnlockAll(dbContext);
            //    //Apply access security rules 
            //    var securite = dbContext.Securites.FirstOrDefault();
            //    if (securite != null && securite.DelaiAvantDesactivationUtilisateur > 0)
            //    {
            //        var idSet = dbContext.Logs.Where(x => x.DLogin>DateTime.Now.AddDays(-securite.DelaiAvantDesactivationUtilisateur))
            //            .Select(i=>i.RefUtilisateur)
            //            .Distinct().ToHashSet();
            //        var notFoundItems = dbContext.Utilisateurs.Where(item => 
            //        !idSet.Contains(item.RefUtilisateur) 
            //        && item.Actif
            //        && item.DCreation < DateTime.Now.AddDays(-securite.DelaiAvantDesactivationUtilisateur)).ToList(); 
            //        foreach (Utilisateur u in notFoundItems)
            //        {
            //            u.Actif = false;
            //            u.RefUtilisateurCourant = 1;
            //        }
            //        dbContext.SaveChanges();
            //    }
            //}
        }
    }
}
