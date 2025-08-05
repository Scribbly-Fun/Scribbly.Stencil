
using Scalar.AspNetCore;
using Scribbly.Stencil;
using Scribbly.Stencil.Cookbook.ApiService;
using Scribbly.Stencil.Cookbook.ApiService.Endpoints.Menu;
using Scribbly.Stencil.Cookbook.ApiService.Endpoints.Menu.Breakfast;
using Scribbly.Stencil.Cookbook.ApiService.Endpoints.Menu.Dinner;
using Scribbly.Stencil.Cookbook.ApiService.Endpoints.Menu.Lunch;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Layout = ScalarLayout.Modern;
        options.Theme = ScalarTheme.Purple;
    });
}

app.MapStencilApp();

app.Run();

