var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var app = builder.Build();

// Serve static files from wwwroot (CSS, JS, images)
app.UseStaticFiles();

app.MapRazorPages();

app.Run();
