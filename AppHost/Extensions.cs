public static class Extensions
{
    public static IResourceBuilder<ContainerResource> AddDataApiBuilderInternal(this IDistributedApplicationBuilder builder, string name, string configPath)
    {
        var config = new FileInfo(configPath);

        return builder
        .AddContainer(name, "azure-databases/data-api-builder")
        .WithImageTag("1.6.67")
        .WithImageRegistry("mcr.microsoft.com")
        // expose redis connection string to the DAB container via environment variable
        .WithEnvironment("Redis__Connection", "redis:6379")
        .WithHttpEndpoint(
            port: null,
            targetPort: 5000,
            name: "http")
        .WithBindMount(config.FullName, $"/App/dab-config.json", true)
        .WithOtlpExporter()
        .WithUrls(e =>
        {
            var endpoint = e.GetEndpoint("http");
            e.Urls.Add(new() { Url = "/swagger", DisplayText = "/Swagger", Endpoint = endpoint });
            e.Urls.Add(new() { Url = "/graphql", DisplayText = "/Nitro", Endpoint = endpoint });
            e.Urls.Add(new() { Url = "/health", DisplayText = "/Health", Endpoint = endpoint });
        })
        .WithHttpHealthCheck("/health");
    }

    public static IResourceBuilder<SqlServerDatabaseResource> AddSqlServerInternal(this IDistributedApplicationBuilder builder, string name, string password, int port = 1234, string database = "db")
    {
        var sqlPassword = builder.AddParameter("sql-password", password);

        return builder
        .AddSqlServer(
            name: name,
            port: port,
            password: sqlPassword)
        .WithLifetime(ContainerLifetime.Persistent)
        .WithDataVolume("sql-data-volume")
        .AddDatabase(database);
    }
}