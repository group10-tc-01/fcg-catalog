using System.Diagnostics.CodeAnalysis;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Messages;

namespace FCG.Catalog.Domain.Catalog.ValueObjects
{
    public sealed record Discount
    {
        public decimal Value { get; }

        private Discount(decimal value)
        {
            if (value < 0 || value > 100)
               throw new DomainException(ResourceMessages.DiscountMustBeBetweenZeroAndHundred);

            Value = value;
        }

        public static Discount Create(decimal value)
        {

            return new Discount(value);
        }

        public static implicit operator decimal(Discount discount) => discount.Value;
        public static implicit operator Discount(decimal value) => Create(value);

        public override string ToString() => Value.ToString("F2");
    }
}
