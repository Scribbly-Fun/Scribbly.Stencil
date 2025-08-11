
using Scalar.AspNetCore;
using Scribbly.Stencil;
using Scribbly.Stencil.Cookbook.ApiService;
using Scribbly.Stencil.Cookbook.ApiService.Endpoints.Menu;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.Configure<MyOptions>(
    builder.Configuration.GetSection(nameof(MyOptions)));

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

builder.Services.AddStencil(options =>
{
    options.ServicesScope = ServiceLifetime.Transient;
});

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