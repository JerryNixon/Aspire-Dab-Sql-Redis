var builder = DistributedApplication.CreateBuilder(args);

var sql = builder
    .AddSqlServerInternal(
        name: "sql",
        password: "P@ssw0rd!",
        port: 1234,
        database: "db");

var sqlproj = builder
    .AddSqlProject<Projects.Database>(
        name: "sqlproj")
    .WithReference(sql);

var api = builder
    .AddDataApiBuilderInternal(
        name: "dab",
        path: "./api/dab-config.json")
    .WaitForCompletion(sqlproj)
    .WithReference(sql);

builder.AddProject<Projects.Web>(
        name: "web")
    .WithReference(api.GetEndpoint("http"));

builder.Build().Run();
