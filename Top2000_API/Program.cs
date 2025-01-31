using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Top2000_API.Data;

var builder = WebApplication.CreateBuilder(args);

// Set up the database context and connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Set up Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add controllers and other services
builder.Services.AddControllers();

var app = builder.Build();

// Middleware setup
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
