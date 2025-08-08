var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.AuthService_Web>("web");

builder.Build().Run();
