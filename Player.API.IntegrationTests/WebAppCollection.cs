namespace Player.API.IntegrationTests;

[CollectionDefinition(nameof(WebAppCollection))]
public class WebAppCollection : IClassFixture<CustomWebApplicationFactory>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}