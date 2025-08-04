
using Scalar.AspNetCore;
using Scribbly.Stencil;
using Scribbly.Stencil.Cookbook.ApiService;
using Scribbly.Stencil.Cookbook.ApiService.Endpoints.Menu;
using Scribbly.Stencil.Cookbook.ApiService.Endpoints.Menu.Breakfast;

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

app.MapScribblyApp();

// var scribbly_stencil_cookbook_apiservice_endpoints_menu_menugroup = app.MapMenuGroup();
//
// scribbly_stencil_cookbook_apiservice_endpoints_menu_menugroup.MapGetMenuRequestsGetMenusEndpoint();
// scribbly_stencil_cookbook_apiservice_endpoints_menu_menugroup.MapGetMenuRequestsGetMenuEndpoint();
//
// var scribbly_stencil_cookbook_apiservice_endpoints_menu_breakfast_breakfastgroup = scribbly_stencil_cookbook_apiservice_endpoints_menu_menugroup.MapBreakfastGroup();
//
// scribbly_stencil_cookbook_apiservice_endpoints_menu_breakfast_breakfastgroup.MapBreakfastEndpointsGetBreakfastMenuEndpoint();
// scribbly_stencil_cookbook_apiservice_endpoints_menu_breakfast_breakfastgroup.MapBreakfastEndpointsPostBreakfastMenuEndpoint();
// scribbly_stencil_cookbook_apiservice_endpoints_menu_breakfast_breakfastgroup.MapBreakfastEndpointsPutBreakfastMenuEndpoint();
//

app.Run();

