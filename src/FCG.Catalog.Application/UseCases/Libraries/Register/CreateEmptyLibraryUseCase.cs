using FCG.Catalog.Domain;
using FCG.Catalog.Domain.Catalog.Entity.Libraries;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Repositories.Library;
using FCG.Catalog.Messages;
using FCG.Domain.Repositories.LibraryRepository;
using MediatR;

namespace FCG.Catalog.Application.UseCases.Libraries.Register
{
    public class CreateEmptyLibraryUseCase : IRequestHandler<CreateEmptyLibraryCommand>
    {
        private readonly IReadOnlyLibraryRepository _readOnlyLibraryRepository;
        private readonly IWriteOnlyLibraryRepository _writeOnlyLibraryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateEmptyLibraryUseCase(IReadOnlyLibraryRepository readOnlyLibraryRepository, IWriteOnlyLibraryRepository writeOnlyLibraryRepository, IUnitOfWork unitOfWork)
        {
            _readOnlyLibraryRepository = readOnlyLibraryRepository;
            _writeOnlyLibraryRepository = writeOnlyLibraryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CreateEmptyLibraryCommand request, CancellationToken cancellationToken)
        {
            var existingLibrary = await _readOnlyLibraryRepository.GetByUserIdAsync(request.UserId, cancellationToken);

            if (existingLibrary != null)
                throw new ConflictException(string.Format(ResourceMessages.LibraryAlreadyExists, request.UserId)); ;

            var library = Library.Create(request.UserId);
            await _writeOnlyLibraryRepository.AddAsync(library);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
