var builder = DistributedApplication.CreateBuilder(args);

var sql = builder
    .AddSqlServerInternal(name: "sql",
        password: "P@ssw0rd!",
        port: 1234,
        database: "db");

var sqlproj = builder
    .AddSqlProject<Projects.Database>(name: "sqlproj")
    .WithReference(sql);

var dab = builder
    .AddDataApiBuilderInternal(name: "dab",
        configPath: "../api/dab-config.json")
    .WithReference(sql)
    .WaitForCompletion(sqlproj);

var web = builder.AddProject<Projects.Web>(name: "web")
    .WithParentRelationship(dab)
    .WithReference(dab.GetEndpoint("http"))
    .WaitFor(dab);

builder.Build().Run();