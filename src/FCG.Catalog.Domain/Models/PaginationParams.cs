using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.Domain.Models
{
    [ExcludeFromCodeCoverage]
    public class PaginationParams
    {
        [Range(1, int.MaxValue, ErrorMessage = "PageNumber deve ser maior que 0")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 50, ErrorMessage = "PageSize deve estar entre 1 e 50")]
        public int PageSize { get; set; } = 10;
    }
}
