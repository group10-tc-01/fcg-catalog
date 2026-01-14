using Bogus;
using FCG.Catalog.Domain.Services;

namespace FCG.Catalog.CommomTestUtilities.Builders.Users
{
    public class LoggedUserInfoBuilder
    {
        public LoggedUserInfo Build()
        {
            var faker = new Faker();
            return new LoggedUserInfo
            {
                Id = faker.Random.Guid(),
                Role = faker.PickRandom("Admin", "User", "Premium")
            };
        }

        public LoggedUserInfo BuildWithId(Guid userId)
        {
            var faker = new Faker();
            return new LoggedUserInfo
            {
                Id = userId,
                Role = faker.PickRandom("Admin", "User", "Premium")
            };
        }

        public LoggedUserInfo BuildWithRole(string role)
        {
            var faker = new Faker();
            return new LoggedUserInfo
            {
                Id = faker.Random.Guid(),
                Role = role
            };
        }

        public LoggedUserInfo BuildWithParameters(Guid userId, string role)
        {
            return new LoggedUserInfo
            {
                Id = userId,
                Role = role
            };
        }
    }
}