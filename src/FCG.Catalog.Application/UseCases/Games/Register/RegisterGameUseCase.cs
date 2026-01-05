using FCG.Catalog.Domain.Abstractions;
using FCG.Catalog.Domain.Catalog.ValueObjects;
using FCG.Catalog.Domain.Enum;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Repositories.Game;
using FCG.Catalog.Messages;
using MediatR;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using FCG.Catalog.Domain.Catalog.Entities.Games;

namespace FCG.Catalog.Application.UseCases.Games.Register
{
    [ExcludeFromCodeCoverage]
    public class RegisterGameUseCase : IRequestHandler<RegisterGameInput, RegisterGameOutput>
    {
        private readonly IWriteOnlyGameRepository _writeRepo;
        private readonly IReadOnlyGameRepository _readRepo;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterGameUseCase(IWriteOnlyGameRepository writeRepo, IReadOnlyGameRepository readRepo, IUnitOfWork unitOfWork)
        {
            _writeRepo = writeRepo;
            _readRepo = readRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task<RegisterGameOutput> Handle(RegisterGameInput request, CancellationToken cancellationToken)
        {
            await ValidateIfGameAlreadyExistsAsync(request.Name);

            var price = Price.Create(request.Price);

            if (!Enum.IsDefined(typeof(GameCategory), request.Category))
            {
                throw new DomainException($"Invalid category: '{request.Category}'. Available categories are: Action, Adventure, RPG...");
            }
            var game = Game.Create(request.Name, request.Description, price, request.Category);

            await _writeRepo.AddAsync(game);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RegisterGameOutput { Id = game.Id, Name = game.Title };
        }
        private async Task ValidateIfGameAlreadyExistsAsync(string name)
        {
            var game = await _readRepo.GetByNameAsync(name);

            if (game is not null)
            {
              throw new ConflictException(string.Format(ResourceMessages.GameNameAlreadyExists, name));
            }
        }
    }
}
