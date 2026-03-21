using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Messages;

namespace FCG.Catalog.Domain.Catalog.ValueObjects
{
    public sealed class Discount
    {
        public decimal Value { get; private set; }

        private Discount(decimal value)
        {
            if (value < 0 || value > 100)
                throw new DomainException(ResourceMessages.DiscountMustBeBetweenZeroAndHundred);

            Value = value;
        }

        internal void ChangeValue(decimal newValue)
        {
            if (newValue < 0 || newValue > 100)
                throw new DomainException(ResourceMessages.DiscountMustBeBetweenZeroAndHundred);

            Value = newValue;
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
