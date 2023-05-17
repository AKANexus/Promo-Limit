using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PromoLimit.DbContext;
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
	Port = 5432,
    Username = "sysdba",
    Password = "!qwer2846!",
    Host = "promolimit.postgres.database.azure.com"

};
Action<DbContextOptionsBuilder> configureDbContext = c =>
{
	c.UseNpgsql(csb.ConnectionString);
	c.EnableSensitiveDataLogging(true);
	c.EnableDetailedErrors(true);
};

builder.Services.AddDbContext<PromoLimitDbContext>(configureDbContext);

builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<UserDataService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddSingleton<ProdutoDataService>();
builder.Services.AddScoped<JsonWebToken>();
builder.Services.AddSingleton<MlInfoDataService>();
builder.Services.AddScoped<MlApiService>();
builder.Services.AddSingleton<LoggingDataService>();

var app = builder.Build();



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
