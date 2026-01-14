using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Messages;

namespace FCG.Catalog.Domain.Catalog.ValueObjects
{
    public sealed record Price
    {
        public decimal Value { get; }

        private Price(decimal value)
        {
            if (value < 0)
            {
                throw new DomainException(ResourceMessages.PriceCannotBeNegative);
            }

            if (value == 0)
            {
                throw new DomainException(ResourceMessages.GamePriceMustBeGreaterThanZero);
            }

            Value = value;
        }

        public static Price Create(decimal value)
        {
            return new Price(value);
        }

        public static implicit operator decimal(Price price) => price.Value;
        public static implicit operator Price(decimal value) => Create(value);

        public override string ToString() => Value.ToString("F2");
    }
}
