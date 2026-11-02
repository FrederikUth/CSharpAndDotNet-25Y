var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();
var database = postgres.AddDatabase("appdb");

var api = builder.AddProject<Projects.SubTrack_Api>("apiservice")
    .WithReference(database);

builder.AddProject<Projects.Server>("webfrontend")
    .WithReference(api);

builder.Build().Run();
