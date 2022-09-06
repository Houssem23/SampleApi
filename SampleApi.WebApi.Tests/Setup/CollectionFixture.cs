using Xunit;

namespace SampleApi.WebApi.Tests.Setup
{
    [CollectionDefinition("api")]
    public class CollectionFixture:ICollectionFixture<TestContext>
    {
    }
}
