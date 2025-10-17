var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var app = builder.Build();

// Serve static files from wwwroot (CSS, JS, images)
app.UseStaticFiles();

app.MapRazorPages();

// Redirect endpoint to open SQL connection in VS Code via custom scheme
app.MapGet("/connect-sql", (IConfiguration cfg) =>
{
    // Env var naming: services__sql__tcp__0 created by Aspire for the sql server tcp endpoint
    var server = Environment.GetEnvironmentVariable("services__sql__tcp__0") ?? "localhost:1234";
    var password = "P@ssw0rd!"; // If you move to secrets, inject via configuration instead.
    var url = $"vscode://ms-mssql.mssql/connect?server={server}&database=db&user=sa&password={password}&trustServerCertificate=true&authenticationType=SqlLogin";
    return Results.Redirect(url);
});

app.Run();
