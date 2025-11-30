var builder = DistributedApplication.CreateBuilder(args);

var stencil = builder
    .AddProject<Projects.Scribbly_Stencil_Cookbook_Api>("scrb-stencil")
    .WithUrl("/scalar", "📄 API Reference");

builder.Build().Run();
