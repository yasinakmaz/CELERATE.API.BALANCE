var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CELERATE_API_API>("celerate-api-api");

builder.Build().Run();
