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
    .AddProject<Projects.SqlCommander>("sqlcmd")
    .WithParentRelationship(sql)
    .WaitForCompletion(sqlproj)
    .WithReference(sql);

var redis = builder
    .AddRedis("redis")
    .WithRedisCommander();

var api = builder
    .AddDataApiBuilderInternal(name: "dab",
        configPath: "../api/dab-config.json")
    .WaitForCompletion(sqlproj)
    .WithReference(redis)
    .WithReference(sql);

builder.AddProject<Projects.Web>(name: "web")
    .WithReference(api.GetEndpoint("http"))
    .WaitFor(sqlproj);

builder.Build().Run();
