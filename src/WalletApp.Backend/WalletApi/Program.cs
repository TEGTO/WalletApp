using Authentication;
using Authentication.Token;
using AuthEntities.Data;
using AuthEntities.Domain.Entities;
using DatabaseControl;
using ExceptionHandling;
using Microsoft.AspNetCore.Identity;
using Shared;
using WalletApi;
using WalletApi.Features.AuthFeature.Services;
using WalletEntities.Data;
using WalletEntities.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<AuthIdentityDbContext>(
    builder.Configuration.GetConnectionString(Configuration.AUTH_DATABASE_CONNECTION_STRING)!, "WalletApi");

builder.Services.AddDbContextFactory<WalletDbContext>(
    builder.Configuration.GetConnectionString(Configuration.WALLET_DATABASE_CONNECTION_STRING)!, "WalletApi");

#region Cors

bool.TryParse(builder.Configuration[Configuration.USE_CORS], out bool useCors);
var myAllowSpecificOrigins = "_myAllowSpecificOrigins";

if (useCors)
{
    builder.Services.AddApplicationCors(builder.Configuration, myAllowSpecificOrigins, builder.Environment.IsDevelopment());
}

#endregion

#region Identity & Authentication

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AuthIdentityDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureIdentityServices(builder.Configuration);
builder.Services.AddScoped<ITokenHandler, JwtHandler>();

#endregion

#region Project Services 

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddSingleton<IAuthorizedUserRepository, AuthorizedUserRepository>();
builder.Services.AddSingleton<ICardRepository, CardRepository>();
builder.Services.AddSingleton<ITransactionRepository, TransactionRepository>();
builder.Services.AddRepositoryPatternWithResilience<AuthIdentityDbContext>(builder.Configuration);
builder.Services.AddRepositoryPatternWithResilience<WalletDbContext>(builder.Configuration);

#endregion

builder.Services.AddSharedFluentValidation(typeof(Program));
builder.Services.ConfigureCustomInvalidModelStateResponseControllers();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddMediatR(conf =>
{
    conf.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwagger("Wallet API");
}

var app = builder.Build();

if (useCors)
{
    app.UseCors(myAllowSpecificOrigins);
}

if (app.Configuration[Configuration.EF_CREATE_DATABASE] == "true")
{
    await app.ConfigureDatabaseAsync<AuthIdentityDbContext>(CancellationToken.None);
    await app.ConfigureDatabaseAsync<WalletDbContext>(CancellationToken.None);
}

app.UseExceptionMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger("Wallet API V1");
}

app.UseIdentity();

//app.UseHttpsRedirection();

app.MapControllers();

app.Run();

public partial class Program { }