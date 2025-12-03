using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Domain.Catalog.ValueObjects
{
    public sealed record Price
    {
        [ExcludeFromCodeCoverage]

        public decimal Value { get; }

        private Price(decimal value)
        {
            if (value < 0)
            {
                throw new("Vai ser implementado");
            }

            if (value == 0)
            {
                throw new("Vai ser implementado");
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
