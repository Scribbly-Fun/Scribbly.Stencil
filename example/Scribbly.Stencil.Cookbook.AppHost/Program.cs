var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Scribbly_Stencil_Cookbook_ApiService>("scrb-stencil");

builder.Build().Run();
