using Microsoft.EntityFrameworkCore;
using WebApplicationtodolist.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Listen on port 8080 (Render.com friendly)
builder.WebHost.UseUrls("http://*:8080");

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<WebApplicationtodolist.Data.AppDbContext>();

// Ensure the application cookie uses the Identity login path
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();




// Configure the HTTP request pipeline.
// Temporarily disable custom exception handler to see detailed errors (enable later)
// app.UseExceptionHandler("/Home/Error");
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages();
app.MapDefaultControllerRoute();

// Apply pending migrations and seed roles/admin at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<WebApplicationtodolist.Data.AppDbContext>();
        db.Database.Migrate();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roles = new[] { "Admin", "Teacher", "Student" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var myUser = await userManager.FindByEmailAsync("abdlmobdy868@gmail.com");
        if (myUser != null && !await userManager.IsInRoleAsync(myUser, "Admin"))
        {
            await userManager.AddToRoleAsync(myUser, "Admin");
        }
    }
    catch
    {
        // ignore migration/seeding failures here; they should be addressed in deployment logs
    }
}

app.Run();
