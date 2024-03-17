using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Core.Models;
using Core.Services;
using Core.Services.EmailService;
using DataBase;
using DataBase.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Net.Mime;
using System.Text;
using Web2PnK.Helpers;
using Web2PnK.Controllers;
using Core.Helpers;
using Microsoft.AspNetCore.Diagnostics;
using Core.Enums;
using Core.ResourcesFiles;
using Web2PnK.Models;
using Microsoft.Win32;

var builder = WebApplication.CreateBuilder(args);

var appConfig = builder.Configuration
        .GetSection(nameof(AppConfig))
        .Get<AppConfig>();
builder.Services.AddSingleton(appConfig);
builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddControllersWithViews().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var result = new ValidationFailedResult(context.ModelState);

        result.ContentTypes.Add(MediaTypeNames.Application.Json);
        result.ContentTypes.Add(MediaTypeNames.Application.Xml);

        return result;
    };
});


builder.Services.AddDbContext<PnkDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PagesMovieContext") ?? throw new InvalidOperationException("Connection string 'PagesMovieContext' not found.")));


builder.Services.AddIdentity<PnKUser, IdentityRole<Guid>>(options =>
{
    options.Tokens.PasswordResetTokenProvider = "ResetPassword";
    options.SignIn.RequireConfirmedEmail = true;
    options.User.RequireUniqueEmail = true;
    options.Password.RequireNonAlphanumeric = false;
})
    .AddEntityFrameworkStores<PnkDbContext>()
    .AddDefaultTokenProviders()
    .AddTokenProvider<CustomPasswordResetTokenProvider<PnKUser>>("ResetPassword");

builder.Services.Configure<DataProtectionTokenProviderOptions>(opts => opts.TokenLifespan = appConfig.TokenLifeSpan);
builder.Services.Configure<CustomPasswordResetTokenProviderOptions>(opts => opts.TokenLifespan = appConfig.ResetPasswordTokenLifeTime);

builder.Services.AddTransient<IPnkUserRepository, PnkUserRepository>();
builder.Services.AddTransient<IPnkUserService, PnkUserService>();
builder.Services.AddTransient<IUserPhotosService, UserPhotosService>();
builder.Services.AddTransient<IUserPhotosRepository, UserPhotosRepository>();
builder.Services.AddTransient<IAssignmentRepository, AssignmentRepository>();
builder.Services.AddTransient<IAssignmentService, AssignmentService>();
builder.Services.AddTransient<IGlobalSettingsRepository, GlobalSettingsRepository>();
builder.Services.AddTransient<IGlobalSettingsService, GlobalSettingsService>();
builder.Services.AddTransient<ILogService, LogService>();
builder.Services.AddTransient<ILogRepository, LogRepository>();
builder.Services.AddTransient<ICalculateService, CalculateService>();
builder.Services.AddTransient<ICalculateRepository, CalculateRepository>();
builder.Services.AddHostedService<BackgroundWorkerService>();


// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero,
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKeyManager.Instance.GetSecretKey()))
        };
    });

var app = builder.Build();
Logger.Configure(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetService<PnkDbContext>().Database.Migrate();
}

app.UseExceptionHandler(c => c.Run(async context =>
{
    var exception = context.Features?.Get<IExceptionHandlerPathFeature>()?.Error;
    if (exception != null)
    {
        _ = Logger.Instance.LogErrorAsync(Constants.Arrow, exception);
    }

    var response = new Response { Status = nameof(ResponseType.Error), Message = DefaultLanguage.SomethingWrong };
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    
    await context.Response.WriteAsJsonAsync(response);
}));



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("index.html"); ;

app.Run();
