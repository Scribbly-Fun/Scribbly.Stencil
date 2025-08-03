var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Scribbly_Stencil_Cookbook_Api>("scrb-stencil");

builder.Build().Run();
