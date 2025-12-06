namespace FCG.Catalog.Domain.Catalog.ValueObjects
{
    public record Title
    {
        public string Value { get; }

        private Title(string value)
        {
            Value = value;
        }

        public static Title Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new("Vai ser implementado");

            if (value.Length < 2)
                throw new("Vai ser implementado");

            if (value.Length > 255)
                throw new ("Vai ser implementado");

            return new Title(value);
        }

        public static implicit operator Title(string value) => Create(value);
        public static implicit operator string(Title name) => name.Value;

        public override string ToString() => Value;
    }
}
