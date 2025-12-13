using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Messages;

namespace FCG.Catalog.Domain.Catalog.ValueObjects
{
    public sealed record Title
    {
        public string Value { get; }

        private Title(string value)
        {
            Value = value;
        }

        public static Title Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException(ResourceMessages.GameNameIsRequired);

            if (value.Length < 3)
                throw new DomainException(ResourceMessages.GameTitleMinLength);

            if (value.Length > 255)
                throw new DomainException(ResourceMessages.GameTitleMaxLength);

            return new Title(value);
        }

        public static implicit operator Title(string value) => Create(value);
        public static implicit operator string(Title name) => name.Value;

        public override string ToString() => Value;
    }
}
