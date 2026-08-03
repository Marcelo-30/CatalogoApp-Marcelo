using System.Reflection;
using CatalogoRopaMVC.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace CatalogoRopaMVC.Tests.Controllers;

public class ApiSecurityAttributesTests
{
    [Theory]
    [InlineData(typeof(ProductosApiController), nameof(ProductosApiController.PostProducto))]
    [InlineData(typeof(ProductosApiController), nameof(ProductosApiController.PutProducto))]
    [InlineData(typeof(ProductosApiController), nameof(ProductosApiController.DeleteProducto))]
    [InlineData(typeof(CatalogosApiController), nameof(CatalogosApiController.PostCategoria))]
    [InlineData(typeof(CatalogosApiController), nameof(CatalogosApiController.PutCategoria))]
    [InlineData(typeof(CatalogosApiController), nameof(CatalogosApiController.DeleteCategoria))]
    public void CatalogWritesRequireSellerAndAntiforgery(Type controllerType, string methodName)
    {
        var method = GetMethod(controllerType, methodName);

        var authorize = method.GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(authorize);
        Assert.Equal("Vendedor", authorize.Roles);
        Assert.NotNull(method.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>());
    }

    [Theory]
    [InlineData(nameof(AuthApiController.Login), false)]
    [InlineData(nameof(AuthApiController.Register), false)]
    [InlineData(nameof(AuthApiController.Logout), true)]
    public void AuthWritesRequireAntiforgeryAndLogoutRequiresSeller(string methodName, bool requiresSeller)
    {
        var method = GetMethod(typeof(AuthApiController), methodName);

        Assert.NotNull(method.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>());
        var authorize = method.GetCustomAttribute<AuthorizeAttribute>();
        Assert.Equal(requiresSeller, authorize != null);
        if (requiresSeller)
        {
            Assert.Equal("Vendedor", authorize!.Roles);
        }
    }

    private static MethodInfo GetMethod(Type controllerType, string methodName)
    {
        return controllerType.GetMethods()
            .Single(method => method.Name == methodName);
    }
}
