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

*Optionally Add a Group or Route Prefix*

```csharp
app.MapScribblyEndpoints("/api");
```

# Endpoint Groups

Endpoint groups can be created to map several endpoints with common routing and configuration.  ``Scribbly.Stencil``
will build this routing tree for you behind the scenes.  Just create a Group and attached members.

```csharp
[EndpointGroup("/lunch", "Manage Lunch Menu")]
public partial class LunchGroup
{
}
```

This will create a routing group.  Groups can even be members of other groups.  Use the ``GroupMember<T>`` to target a group.  Your new group will be added to the group specified in the Generic Param.

```csharp
[EndpointGroup("/menu", "Menu")]
public partial class MenuGroup
{
}

[GroupMember<MenuGroup>]
[EndpointGroup("/lunch", "Manage Lunch Menu")]
public partial class LunchGroup
{
}
```

Of course you'll need to add endpoints to groups as well yielding a routable endpoint `/menu/lunch/{id}`

```csharp
public partial class LunchEndpoints
{
    [GetEndpoint("lunch/{id}")]
    [GroupMember<LunchGroup>]
    private static IResult GetLunchMenu(HttpContext context, [FromRoute] string id)
    {
        return Results.Ok(id);
    }
}
```

### Configuration

While this is great you may need to plug into the route building and override or append configuration to groups.

Simply add the `Configure` attribute to the `class declaration` and magic 🐣

![group_configure.gif](./docs/group_configure.gif)

Your type will now implement a custom interface used by the code generator.  This tells `Scribbly.Stencil` to invoke your configure method allowing you to modify and append the builder.

```csharp
[Configure]
[GroupMember<ApplicationRoot>]
[EndpointGroup("/lunch", "Manage Lunch Menu")]
public partial class LunchGroup
{
    /// <inheritdoc />
    public void Configure(IEndpointConventionBuilder lunchGroupBuilder)
    {
        applicationRootBuilder.AddEndpointFilter<MyFilter>();
        // .....
    }
}
```