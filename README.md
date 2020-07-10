# RestEase.Serialization.Extensions
![Build](https://github.com/rsivanov/RestEase.Serialization.Extensions/workflows/Build%20&%20test%20&%20publish%20Nuget/badge.svg?branch=master)
[![NuGet](https://img.shields.io/nuget/dt/RestEase.Serialization.Extensions)](https://www.nuget.org/packages/RestEase.Serialization.Extensions) 
[![NuGet](https://img.shields.io/nuget/v/RestEase.Serialization.Extensions)](https://www.nuget.org/packages/RestEase.Serialization.Extensions)

Provides an implementation of RequestQueryParamSerializer that serializes object parameters in a format compatible with [ASP.NET Core MVC model binding for complex types](https://docs.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-3.1#complex-types).

Why?
===
It seems a bit strange, but by default [RestEase](https://github.com/canton7/RestEase) doesn't support serialization of complex type parameters, so that they could be correctly bound to controller method parameters by ASP.NET Core MVC at the server side.
Imagine that we have a following controller:
```csharp
[Route("[controller]")]
public class EmployeesController : Controller
{
    [HttpGet("bulk")]
    public Task<BulkRequest> GetEmployeeByRequest([FromQuery] BulkRequest bulkRequest) =>
        Task.FromResult(bulkRequest);
}

public class BulkRequest
{
    public string Value { get; set; }
    public int[] Ids { get; set; }
}
```
Let's suppose that we need to write an integration test for that conroller using RestEase to get a strongly-typed test code:
```csharp
[BasePath("Employees")]
[SerializationMethods(Query = QuerySerializationMethod.Serialized)]
public interface IEmployeesController
{
    [Get("bulk")]
    Task<BulkRequest> GetEmployeeByRequest([Query] BulkRequest bulkRequest);
}
```
```csharp
[Fact]
public async Task GetEmployeeByRequest_Success()
{
    var controller = RestClient.For<IEmployeesController>(_httpClient);
    var request = new BulkRequest { Ids = new[] { 1, 2, 3 }, Value = "Qwerty" };
    var response = await controller.GetEmployeeByRequest(request);
    Assert.Equal(request.Value, response.Value);
    Assert.True(request.Ids.SequenceEqual(response.Ids));
}
```
The problem is that [RestEase supports only Json serialization or ToString](https://github.com/canton7/RestEase#query-parameters) and that's not what ASP.NET Core MVC expects according to the [official documentation](https://docs.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-3.1#complex-types). 

How to use
===
You just need to set a RequestQueryParamSerializer to ComplexTypeRequestQueryParamSerializer for the Requester that you're going to use. For the sample test code to work correctly:
```csharp
[Fact]
public async Task GetEmployeeByRequest_Success()
{
    var requester = new Requester(_httpClient)
    {
        RequestQueryParamSerializer = new ComplexTypeRequestQueryParamSerializer(),
    };
    var controller = RestClient.For<IEmployeesController>(requester);
    var request = new BulkRequest { Ids = new[] { 1, 2, 3 }, Value = "Qwerty" };
    var response = await controller.GetEmployeeByRequest(request);
    Assert.Equal(request.Value, response.Value);
    Assert.True(request.Ids.SequenceEqual(response.Ids));
}
```
