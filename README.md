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

![hero_vid.gif](./docs/hero_vid.gif)

## Table of Contents
1. [🎁 Packages](#packages)
2. [🎯 Endpoints](#Endpoints)
3. [🛒 Groups](#Groups)
4. [🥣 Cookbook](#Cookbook)

## Example

Below is a brief snip of code to get you started before reading more.

1. Add a reference to the `Scribbly.Stencil` package and annotate a static method on a partial class.

```csharp
[EndpointGroup("/lunch")]
public partial class LunchGroup
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
public static IEndpointRouteBuilder MapLunchGroupGetLunchMenu(this IEndpointRouteBuilder builder)
{
    builder.MapPost("/{id}", PostLunchMenu);
    return builder;
}
```

3. And an Endpoint Registry for All Endpoints Mapping endpoints to Groups

```csharp
public static IEndpointRouteBuilder MapScribblyEndpoints(this IEndpointRouteBuilder builder)
{
    builder.MapLunchGroupGetLunchMenu();
    builder.MapLunchGroupPostLunchMenu();
    return builder;
} 

```

4. Simply Map Your Application

```csharp
app.MapStencilApp();
```

*Optionally Add a Group or Route Prefix*

```csharp
app.MapStencilApp("/api");
```

# 🎯 Endpoints

With `Scribbly.Stencil` endpoints are declared as a static method and automatically mapped to an HTTP request. 
The generator will accept several type configurations however the enclosing class **MUST** be a ``public partial class``.
The method signature **MAY** be private or public, static or an instance member.

```csharp
public partial class BreakfastEndpoints
{
    [PutEndpoint("/breakfast/{id}", "Edit Breakfast", "Edits a Breakfast Item")]
    private object PutBreakfastMenu(HttpContext context, [FromRoute] string id)
    {
        return new { id = id };
    }
}
```

Any method signature supported by a Minimal API Endpoints is support by `Scribbly.Stencil`

```csharp
public partial class BreakfastEndpoints
{
    [PutEndpoint("/breakfast/{id}", "Edit Breakfast", "Edits a Breakfast Item")]
    private static Task<IResult> PutBreakfastMenu([FromQuery] string id)
    {
    }
    
    [PutEndpoint("/breakfast/{id}", "Edit Breakfast", "Edits a Breakfast Item")]
    private static Task<string> PutBreakfastMenu([FromServices] IService service)
    {
    }
}
```

The code generator will kick into action when it finds a method annotated with any of the `EndpointAttributes`

![Static Badge](https://img.shields.io/badge/GetEndpoint-blue)

![Static Badge](https://img.shields.io/badge/PutEndpoint-blue)

![Static Badge](https://img.shields.io/badge/PostEndpoint-green)

![Static Badge](https://img.shields.io/badge/DeleteEndpoint-green)

*once our APIs are stable support for more HTTP verbs will be added.*

Types can declare multiple endpoints

```csharp
public partial class BreakfastEndpoints
{
    [GetEndpoint("/breakfast/{id}", "Edit Breakfast", "Edits a Breakfast Item")]
    private static object GetBreakfastMenu(HttpContext context, [FromRoute] string id)
    {
    }
    
    [PutEndpoint("/breakfast/{id}", "Edit Breakfast", "Edits a Breakfast Item")]
    private static object PutBreakfastMenu(HttpContext context, [FromRoute] string id)
    {
    }
}
```

Endpoints can also be grouped together with a parent ``IEndpointRouteBuilder`` **see groups below for details**

```csharp

[EndpointGroup("/breakfast", "Manage Breakfast Menu")]
public partial class BreakfastGroup
{
}

[GroupMember<BreakfastGroup>]
public partial class BreakfastEndpoints
{
    [GetEndpoint("/{id}", "Edit Breakfast", "Edits a Breakfast Item")]
    private static object GetBreakfastMenu(HttpContext context, [FromRoute] string id)
    {
    }
    
    [PutEndpoint("/{id}", "Edit Breakfast", "Edits a Breakfast Item")]
    private static object PutBreakfastMenu(HttpContext context, [FromRoute] string id)
    {
    }
}
```

Group membership is Type Safe as the Generic Parameter can **ONLY** accept a ``Scribbly.Stencil.IGroup`` interface.
Membership can be added to endpoints at either the `method declaration` or the `class declartion`.

```csharp

[EndpointGroup("/breakfast", "Manage Breakfast Menu")]
public partial class BreakfastGroup
{
}

[EndpointGroup("/menu", "Manage Breakfast Menu")]
public partial class MenuGroup
{
}

[GroupMember<BreakfastGroup>]
public partial class BreakfastEndpoints
{
    [GetEndpoint("/{id}", "Edit Breakfast", "Edits a Breakfast Item")]
    private static object GetBreakfastMenu(HttpContext context, [FromRoute] string id)
    {
    }
    
    [PutEndpoint("/{id}", "Edit Breakfast", "Edits a Breakfast Item")]
    [GroupMember<MenuGroup>]    
    private static object PutBreakfastMenu(HttpContext context, [FromRoute] string id)
    {
    }
}
```
*in the example above the PutBreakfastMenu would be added the `MenuGroup` with a route /menu NOT /breakfast*

### Configuration

`Stencil` allows you to easily plug into the configuration for each endpoint generated by your Methods.
Simply add the `Configure` attribute to the `method declaration` or `class declaration` and magic 🐣

![endpoint_configure.gif](./docs/endpoint_configure.gif)

Your type will now implement a custom interface used by the code generator.  This tells `Scribbly.Stencil` to invoke your configure method allowing you to modify and append the builder.  This interface forces the compiler to guid you.

```csharp
public partial class BreakfastEndpoints
{
    [PutEndpoint("/{id}", "Edit Breakfast", "Edits a Breakfast Item")]
    [Configure]
    private static object PutBreakfastMenu(HttpContext context, [FromRoute] string id)
    {
        return new { id = id };
    }

    /// <inheritdoc />
    public void ConfigurePutBreakfastMenu(IEndpointConventionBuilder putBreakfastMenuBuilder)
    {
        putBreakfastMenuBuilder.ProducesProblem(404);
    }
}
```

# 🛒 Groups

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

Using `Scribbly.Stencil` you can also contain endpoints inside of groups.  Each endpoint will inherit the behavior of the encapsulating class.

Since the `DinnerEndpoints` in the following example and annotated with the `EndpointGroup` and the `GroupMember<MenuGroup>`
the endpoints will yield routes ``/menu/dinner/{id}`` 

```csharp
[EndpointGroup("/dinner", "Dinner time endpoints wrapped in a Group.")]
[GroupMember<MenuGroup>]
public partial class DinnerEndpoints
{
    [GetEndpoint("/{id}", "Gets Dinner", "Queries a new Dinner Item")]
    private IResult GetDinnerMenu(HttpContext context, [FromRoute] string id)
    {
        return Results.Ok(id);
    }
    
    [PostEndpoint("/{id}", "Create Dinner", "Creates a new Dinner Item")]
    private object PostDinnerMenu(HttpContext context, [FromRoute] string id)
    {
        return Results.Ok(id);
    }
    
    [PutEndpoint("/{id}", "Edit Dinner", "Edits a Dinner Item")]
    private object PutDinnerMenu(HttpContext context, [FromRoute] string id)
    {
        return new { id = id };
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