using System;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Application.UseCases.Games.Update
{
    [ExcludeFromCodeCoverage]
    public class UpdateGameOutput
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public decimal Price { get; private set; }
        public string Description { get; private set; }
        public string Category { get; private set; }
        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

        public UpdateGameOutput(Guid id, string title, decimal price, string description, string category, DateTime updatedAt)
        {
            Id = id;
            Title = title;
            Price = price;
            Description = description;
            Category = category;
            UpdatedAt = updatedAt;
        }
    }
}