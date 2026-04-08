using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Infrastructure.MongoDb.Persistence.Attributes
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Class)]
    public class BsonCollectionAttribute : Attribute
    {
        public BsonCollectionAttribute(string collectionName)
        {
            CollectionName = collectionName;
        }

        public string CollectionName { get; }
    }
}