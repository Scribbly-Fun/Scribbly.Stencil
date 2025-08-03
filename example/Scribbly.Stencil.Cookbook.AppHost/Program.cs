var builder = DistributedApplication.CreateBuilder(args);

var stencil = builder.AddProject<Projects.Scribbly_Stencil_Cookbook_Api>("scrb-stencil");

builder.Build().Run();
