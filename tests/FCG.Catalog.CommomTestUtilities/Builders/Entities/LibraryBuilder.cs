using Bogus;
using FCG.Catalog.Domain.Catalog.Entity.Libraries;

namespace FCG.CommomTestsUtilities.Builders.Entities
{
    public static class LibraryBuilder
    {
        public static Library Build(Guid? userId = null)
        {
            var id = userId ?? Guid.NewGuid();
            return new Faker<Library>().CustomInstantiator(f => Library.Create(id));
        }

    }

}