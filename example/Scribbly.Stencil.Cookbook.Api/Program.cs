
using Scribbly.Stencil;
using Scribbly.Stencil.Cookbook.ApiService;
using Scribbly.Stencil.Cookbook.ApiService.Endpoints.Lunch;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

WebApplication app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapDefaultEndpoints();

// app.MapLunchGroup();

app.MapScribblyEndpoints();

app.Run();

