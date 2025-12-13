using FCG.Catalog.Domain.Abstractions;
using FluentAssertions;

namespace FCG.Catalog.UnitTests.Domain.Catalog
{
    public class BaseEntityTests
    {
        public class ConcreteEntity : BaseEntity
        {
            public ConcreteEntity() : base() { }
        }

        [Fact]
        public void Given_NewEntity_When_Created_Then_IdAndDatesAreSetAndIsActiveIsTrue()
        {
            var entity = new ConcreteEntity();

            entity.Should().NotBeNull();
            entity.Id.Should().NotBe(Guid.Empty);
            entity.IsActive.Should().BeTrue();
            entity.CreatedAt.Should().BeOnOrBefore(DateTime.UtcNow);
            entity.UpdatedAt.Should().BeOnOrBefore(DateTime.UtcNow);
        }

        [Fact]
        public void Given_Entity_When_DeactivateIsCalled_Then_IsActiveIsFalseAndUpdateTimeIsSet()
        {
            var entity = new ConcreteEntity();

            entity.Deactivate();

            entity.IsActive.Should().BeFalse();
            entity.UpdatedAt.Should().BeOnOrBefore(DateTime.UtcNow);
        }

        [Fact]
        public void Given_InactiveEntity_When_ActivateIsCalled_Then_IsActiveIsTrueAndUpdateTimeIsSet()
        {

            var entity = new ConcreteEntity();
            entity.Deactivate();

            entity.Activate();

            entity.IsActive.Should().BeTrue();
            entity.UpdatedAt.Should().BeOnOrBefore(DateTime.UtcNow);
        }
    }
}
