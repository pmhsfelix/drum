drum
====

[![Build status](https://ci.appveyor.com/api/projects/status/bdf3iyaua0qky38a)](https://ci.appveyor.com/project/pmhsfelix/drum)

Drum - Direct Route URI Maker for ASP.NET Web API

Drum is a little library for building URIs to ASP.NET Web API actions, using direct routes and lambda expressions.
It provides an alternative to the [UrlHelper](http://msdn.microsoft.com/en-us/library/system.web.http.routing.urlhelper(v=vs.118).aspx) class. 
Instead of requiring a route name and a set of name-value pairs, Drum allows the creation of URIs using actions invocations.

```csharp
// using UrlHelper 
var uri1 = _urlHelper.Link("GetPaged", new { page = 0, count = 10 });

// using UriMaker
var uri2 = _uriMaker.UriFor(c => c.GetPaged(0, 10));
```

where `GetPaged` is a Web API controller action

```
[RoutePrefix("api/UriMakerTests/resources")]
public class ResourceController : ApiController
{
    [Route("", Name="GetPaged")]
    public HttpResponseMessage GetPaged(int page, int count) {...}

    ...
}
```

The idea of using lambda expressions to generated URIs is not a new one.

* In the following post, [José Romanielo](http://joseoncode.com/) creates a _typed resource linker_ for a WCF Web API preview: [http://joseoncode.com/2011/03/18/wcf-web-api-strongly-typed-resource-linker](http://joseoncode.com/2011/03/18/wcf-web-api-strongly-typed-resource-linker/)

* The [Hyprlinkr](https://github.com/ploeh/Hyprlinkr) library provides a `RouteLinker` class for creating URIs based on lambda expressions.
By default, it assumes that there is only a single configured route named "API Default". 
Different behaviors require the injection of a new route dispatcher implementation.

On the other hand, Drum was created to work with _direct routes_, which were introduced on Web API 2, with the name of _attribute routes_.
This type of routing was generalized on Web API 2.2, allowing for alternative route definition mechanisms. 
For an example called `Strathweb.TypedRouting`, see the following post by [Filip W.](https://twitter.com/filip_woj):[http://www.strathweb.com/2014/07/building-strongly-typed-route-provider-asp-net-web-api/](http://www.strathweb.com/2014/07/building-strongly-typed-route-provider-asp-net-web-api/).

Drum aims to work with _any_ routing mechanism based on direct routes.
Drum also works with unnamed routes.

## Usage

### Configuration

By default, attribute-based direct routes are enabled by calling the [`MapHttpAttributeRoutes`](http://msdn.microsoft.com/en-us/library/dn479134(v=vs.118).aspx) extension method over the HttpConfiguration.
To use Drum, use the  `MapHttpAttributeRoutesAndUseUriMaker` method intead.

```csharp
config.MapHttpAttributeRoutesAndUseUriMaker();
```

If using a custom direct route mechanism, such as `Strathweb.TypedRouting`, just pass the custom `IDirectRouteProvider` as a parameter to the `MapHttpAttributeRoutesAndUseUriMaker` method (see [https://github.com/pmhsfelix/drum/blob/master/src/Drum.Tests/StrathWebTypedRouteTests.cs](https://github.com/pmhsfelix/drum/blob/master/src/Drum.Tests/StrathWebTypedRouteTests.cs) for an example).

```csharp
config.MapHttpAttributeRoutesAndUseUriMaker(new TypedDirectRouteProvider());
```

### `UriMaker` 
The Drum library  provides an `UriMaker<TController>` class, containing the `UriFor` method.
This method receives an `Expression<Action<TController>>` and returns an URI pointing to the action invoked on the given lambda expression.

### Obtaining `UriMaker` instances

`UriMaker` instances can be obtained by one of two methods
* Based on the current HTTP request.
* Using dependency injection.

#### Creating `UriMaker` from the current request

When you register Drum by calling the `MapHttpAttributeRoutesAndUseUriMaker` method, `UriMakerContext` instance is stored in the `Properties` of the `HttpConfiguration` and available for every incoming HTTP request. When needed, use the `TryGetUriMakerFor<TController>` to obtain a `UriMaker<TController>` instance, for instance inside a controller.

```csharp
var uriMaker = Request.TryGetUriMakerFor<TypedRoutesController>();
```

#### Using dependency injection

Alternatively, a dependency injection container can also be used to provide the `UriMaker` instances.
For that, the context returned by `MapHttpAttributeRoutesAndUseUriMaker` must be configured on the container as a singleton instance.

The following excerpt shows an example using the [Autofac](http://autofac.org) container.

```csharp
// Autofac
var builder = new ContainerBuilder();
builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

// Web API routes
var uriMakerContext = config.MapHttpAttributeRoutesAndUseUriMaker();
builder.RegisterInstance(uriMakerContext).ExternallyOwned();
builder.RegisterHttpRequestMessage(config);
builder.RegisterGeneric(typeof(UriMaker<>)).AsSelf().InstancePerRequest();
```
## Nuget

Drum is available as a nuget package at: [https://www.nuget.org/packages/Drum/](https://www.nuget.org/packages/Drum/)