using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using eVaSys.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using eVaSys.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Session Managment
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllers()
    .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore)
    .AddNewtonsoftJson(options => options.SerializerSettings.Formatting = Formatting.Indented)
    .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
;
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add ApplicationDbContext and SQL Server support
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration["Data:DefaultConnection:ConnectionString"]
        )
);

//AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Add Authentication with JWT Tokens
builder.Services.AddAuthentication(opts =>
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
        ValidIssuer = builder.Configuration["Auth:Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Auth:Jwt:Key"])),
        ValidAudience = builder.Configuration["Auth:Jwt:Audience"],
        ClockSkew = TimeSpan.Zero,

        // security switches
        RequireExpirationTime = true,
        ValidateIssuer = true,
        ValidateIssuerSigningKey = true,
        ValidateAudience = true
    };
    cfg.IncludeErrorDetails = true;
});


var app = builder.Build();
IHostApplicationLifetime lifetime = app.Lifetime;
lifetime.ApplicationStarted.Register(() =>OnAppStarted(builder.Configuration));
app.UseSession();
app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();



void OnAppStarted(ConfigurationManager Configuration)
{
    var connectionstring = Configuration["Data:DefaultConnection:ConnectionString"];

    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
    optionsBuilder.UseSqlServer(connectionstring);
    using (ApplicationDbContext dbContext = new(optionsBuilder.Options))
    {
        Utils.UnlockAll(dbContext);
        //Apply access security rules 
        var securite = dbContext.Securites.FirstOrDefault();
        if (securite != null && securite.DelaiAvantDesactivationUtilisateur > 0)
        {
            var idSet = dbContext.Logs.Where(x => x.DLogin > DateTime.Now.AddDays(-securite.DelaiAvantDesactivationUtilisateur))
                .Select(i => i.RefUtilisateur)
                .Distinct().ToHashSet();
            var notFoundItems = dbContext.Utilisateurs.Where(item =>
            !idSet.Contains(item.RefUtilisateur)
            && item.Actif
            && item.DCreation < DateTime.Now.AddDays(-securite.DelaiAvantDesactivationUtilisateur)).ToList();
            foreach (Utilisateur u in notFoundItems)
            {
                u.Actif = false;
                u.RefUtilisateurCourant = 1;
            }
            dbContext.SaveChanges();
        }
    }
}


//****************************************************************  OLD CODE  ****************************************************************

//using Microsoft.AspNetCore;
//using Microsoft.AspNetCore.Hosting;

//namespace eValorplastWebApp
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            CreateWebHostBuilder(args).Build().Run();
//        }

//        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
//            WebHost.CreateDefaultBuilder(args)
//                .UseStartup<Startup>();
//    }
//}
