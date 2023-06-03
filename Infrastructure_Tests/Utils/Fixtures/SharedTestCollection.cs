using Xunit;

namespace Infrastructure_Tests.Utils.Fixtures;

[CollectionDefinition("db-fixture")]
public sealed class SharedTestCollection : ICollectionFixture<DbContextFixture>
{
    
}