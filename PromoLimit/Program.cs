using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MlSuite.EntityFramework.EntityFramework;
using Npgsql;
using PromoLimit.Contexts;
using PromoLimit.Models.Local;
using PromoLimit.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opts =>
{
    opts.IdleTimeout = TimeSpan.FromMinutes(20);
    opts.Cookie.HttpOnly = true;
    opts.Cookie.IsEssential = true;
});
NpgsqlConnectionStringBuilder csb = new()
{
	Database = "promolimit",
	Port = 5351,
    Username = "promolimitDBA",
    Password = "gN810BEkbbRI",
    //#if DEBUG
    Host = "192.168.10.215"
    //#else
    //                Host = "tinformatica.dyndns.org"
    //#endif

};

NpgsqlConnectionStringBuilder mlEspelhoCsb = new NpgsqlConnectionStringBuilder
{
    Database = "meliEspelho",
    Port = 5351,
    Username = "meliDBA",
    Password = builder.Configuration.GetSection("SuperSecretSettings")["NpgPassword"],
    //#if DEBUG
    Host = "192.168.10.215"
    //#else
    //                Host = "tinformatica.dyndns.org"
    //#endif
};

builder.Services.AddDbContext<PromoLimitDbContext>(c =>
{
    c.UseNpgsql(csb.ConnectionString);
    c.EnableSensitiveDataLogging();
    c.EnableDetailedErrors();
});

builder.Services.AddDbContext<TrilhaDbContext>(c =>
{
    c.UseNpgsql(mlEspelhoCsb.ConnectionString);
    c.EnableSensitiveDataLogging();
    c.EnableDetailedErrors();
});

builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<UserDataService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddSingleton<ProdutoDataService>();
builder.Services.AddScoped<JsonWebToken>();
builder.Services.AddSingleton<MlInfoDataService>();
builder.Services.AddScoped<MlApiService>();
builder.Services.AddScoped<TinyApi>();
builder.Services.AddSingleton<LoggingDataService>();
builder.Services.AddSingleton<PostgresNotificationService>();
builder.Services.AddTransient<CallbackService>();


var app = builder.Build();
NotificationTask = app.Services.GetRequiredService<PostgresNotificationService>().MonitorForNotification();

void OnNotificationReceived(object? o, NpgsqlNotificationEventArgs notificationArgs)
{
    var callbackService = app.Services.GetRequiredService<CallbackService>();
    //Debugger.Break();
    if (notificationArgs.Payload.Split('|')[1] == "Pedidos")
    {
        app.Services.GetRequiredService<PostgresNotificationService>().NotificationReceived -= OnNotificationReceived;
        _ = callbackService.RunCallbackChecks(Guid.Parse(notificationArgs.Payload.Split('|')[2]));
    }
}

app.Services.GetRequiredService<PostgresNotificationService>().NotificationReceived += OnNotificationReceived;

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<PromoLimitDbContext>();
    await dataContext.Database.MigrateAsync();
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

//app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
