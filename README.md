![scribbly_banner.png](./docs/scribbly_banner.png)

![GitHub](https://img.shields.io/github/license/Scribbly-Fun/Scribbly.Stencil) 
![GitHub all releases](https://img.shields.io/github/downloads/Scribbly-Fun/Scribbly.Stencil/total) 
![Nuget](https://img.shields.io/nuget/dt/Scribbly.Stencil)
[![GitHub issues](https://img.shields.io/github/issues/Scribbly-Fun/Scribbly.Stencil)](https://github.com/Scribbly-Fun/Scribbly.Stencil/issues)
![GitHub Repo stars](https://img.shields.io/github/stars/Scribbly-Fun/Scribbly.Stencil?style=social)
![GitHub forks](https://img.shields.io/github/forks/Scribbly-Fun/Scribbly.Stencil?style=social)
![GitHub last commit (branch)](https://img.shields.io/github/last-commit/Scribbly-Fun/Scribbly.Stencil/main)

# Scribbly Stencil

A framework for organizing and generating minimal API endpoints.

![Static Badge](https://img.shields.io/badge/ROUTE-blue)
![Static Badge](https://img.shields.io/badge/REQUEST-green)
![Static Badge](https://img.shields.io/badge/RESPOND-blue)

## Table of Contents
1. [🎁 Packages](#packages)
2. [💪 Groups](#Groups)
3. [🛒 Endpoints](#Endpoints)
4. [🥣 Cookbook](#Cookbook)

## Example

Below is a brief snip of code to get you started before reading more.

1. Add a reference to the `Scribbly.Stencil` package and annotate a method on a partial static class.

```csharp
[EndpointGroup("/lunch")]
public static partial class LunchGroup
{
    // Annotate any method signature with the VerbEndpoint attribute
    [GetEndpoint("/{id}", "Get Lunch", "Fetches the current lunch for the provided restaurant ID.")]
    private static IResult GetLunchMenu(HttpContext context, string id)
    {
        return Results.Ok(id);
    }
    
    [PostEndpoint("/{id}", "Get Lunch", "Fetches the current lunch for the provided restaurant ID.")]
    private static IResult PostLunchMenu(HttpContext context, string id, LunchItem request)
    {
        return Results.Ok(request);
    }
}
```
*Scribbly.Stencil will generate a few things for you behind the Scenes*

2. An HTTP Endpoint Registration Method

``` csharp
public static IEndpointRouteBuilder MapDinnerGroupPostLunchMenuEndpoint(this IEndpointRouteBuilder builder)
{
    builder.MapPost("/{id}", PostLunchMenu);
    return builder;
}
```

3. And an Endpoint Registry for All Endpoints Mapping endpoints to Groups

```csharp
public static IEndpointRouteBuilder MapScribblyEndpoints(this IEndpointRouteBuilder builder)
{
    builder.MapLunchGroupGetLunchMenuEndpoint();
    builder.MapLunchGroupPostLunchMenuEndpoint();
    return builder;
} 

```

4. Simply Map Your Application

```csharp
app.MapScribblyEndpoints();
```

*Optionally Add a Group or Route Prefix

```csharp
app.MapScribblyEndpoints("/api");
```
