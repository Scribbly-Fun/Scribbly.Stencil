namespace Scribbly.Stencil.IntegrationTests;

public static class Collections
{
    public const string Api = "Scribbly.Api";
}

[CollectionDefinition(Collections.Api)]
public class ApiTestCollection : ICollectionFixture<ApplicationFactory>
{
}