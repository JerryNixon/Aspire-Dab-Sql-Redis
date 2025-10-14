using Microsoft.SqlServer.Dac.Deployment;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder
    .AddSqlServerInternal(name: "sql",
        password: "P@ssw0rd!",
        port: 1234,
        database: "db");

var sqlproj = builder
    .AddSqlProject<Projects.Database>(name: "sqlproj")
    .WithReference(sql);

var sqlcmd = builder
    .AddProject<Projects.SqlCommander>("sql-commander")
    .WithUrls(e =>
    {
        e.Urls.First().DisplayText = "SQL Commander";
        e.Urls.RemoveAt(1);
    })
    .WithParentRelationship(sql)
    .WithReference(sql);

var redis = builder
    .AddRedis("redis")
    .WithRedisCommander()
    .WithExplicitStart();

var dab = builder
    .AddDataApiBuilderInternal(name: "dab",
        configPath: "../api/dab-config.json")
    .WithReference(redis)
    .WithReference(sql);

builder.AddProject<Projects.Web>(name: "web")
    .WithParentRelationship(dab)
    .WithReference(dab.GetEndpoint("http"))
    .WaitFor(dab);
