using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Top2000_API.Models;
using Microsoft.OpenApi.Models;
using Top2000_API.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Database configuratie
builder.Services.AddDbContext<Top2000_API.Data.ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

// Set up Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 🔹 Controllers en Swagger toevoegen
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Top2000 API", Version = "v1" });
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// 🔹 Middleware setup
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Top2000 API v1"));
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

// 🔹 Nodige middleware toevoegen
app.UseHttpsRedirection();
app.UseRouting();  // ✅ BELANGRIJK: Routing inschakelen!
app.UseAuthorization();
app.MapControllers();  // ✅ BELANGRIJK: Controllers correct mappen

app.Run();
