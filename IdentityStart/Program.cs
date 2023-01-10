using Microsoft.EntityFrameworkCore;
using Data;
using Microsoft.AspNetCore.Identity;
using IdentityStart.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using IdentityStart.Helper;
using Microsoft.AspNetCore.Identity.UI.Services;
using IdentityStart.Services.UrlService;
using AspNetCore.IServiceCollection.AddIUrlHelper;

var builder = WebApplication.CreateBuilder(args);

//Connection to the DB
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("default"));
});

// Add services to the container.
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddScoped<IEmailSender, MailJetService>();
builder.Services.AddScoped<IUrlServices, UrlServices>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddUrlHelper();
//HelperPart
builder.Services.AddScoped<IHelperService, HelperService>();
//To Redirect to the login page if the user is not authenticate 
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = new PathString("/Account/Login");
    options.AccessDeniedPath = new PathString("/Account/Locked");
});
builder.Services.AddControllersWithViews();

//Identity Options
builder.Services.Configure<IdentityOptions>(options => 
{
    //What the password need
    options.Password.RequiredLength = 5;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireLowercase = true;

    //Lockout-Options

    //The time the account is going to be block
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
    //Attemps to login in.
    options.Lockout.MaxFailedAccessAttempts = 3;
});

//Facebook AUTH
builder.Services.AddAuthentication().AddFacebook
    (
        options =>
        {
            //Obtainig values from the appsettings.json
            options.AppId =builder.Configuration.GetValue<string>("facebook:app_id");
            options.AppSecret = builder.Configuration.GetValue<string>("facebook:app_secret");
        }
    );

//Google Auth
builder.Services.AddAuthentication().AddGoogle
    (   
        options =>
        {
            options.ClientId = builder.Configuration.GetValue<string>("google:client_id");
            options.ClientSecret = builder.Configuration.GetValue<string>("google:client_secret");
        }
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
//Adding the Authentication (Must be above the Authorization)
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
