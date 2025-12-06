using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Catalog.Application.UseCases.Games.Register
{
    public class RegisterGameOutput
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}
