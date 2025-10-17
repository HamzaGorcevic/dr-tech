using MediatR;
using drTech_backend.Infrastructure.Abstractions;
using FluentValidation;

namespace drTech_backend.Application.Common.Mediator
{
    // Queries
    public record GetAllQuery<T>() : IRequest<IReadOnlyList<T>> where T : class;
    public record GetByIdQuery<T>(Guid Id) : IRequest<T?> where T : class;

    // Commands
    public record CreateCommand<T>(T Entity) : IRequest<T> where T : class;
    public record UpdateCommand<T>(T Entity) : IRequest where T : class;
    public record DeleteCommand<T>(Guid Id) : IRequest where T : class;

    // Handlers (generic, provider-agnostic through IDatabaseService<T>)
    public class GetAllQueryHandler<T> : IRequestHandler<GetAllQuery<T>, IReadOnlyList<T>> where T : class
    {
        private readonly IUnitOfWork _uow;
        public GetAllQueryHandler(IUnitOfWork uow) { _uow = uow; }
        public async Task<IReadOnlyList<T>> Handle(GetAllQuery<T> request, CancellationToken cancellationToken)
        {
            var repo = _uow.GetRepository<T>();
            var items = await repo.GetAllAsync(cancellationToken);
            return items;
        }
    }

    public class GetByIdQueryHandler<T> : IRequestHandler<GetByIdQuery<T>, T?> where T : class
    {
        private readonly IUnitOfWork _uow;
        public GetByIdQueryHandler(IUnitOfWork uow) { _uow = uow; }
        public async Task<T?> Handle(GetByIdQuery<T> request, CancellationToken cancellationToken)
        {
            var repo = _uow.GetRepository<T>();
            return await repo.GetByIdAsync(request.Id, cancellationToken);
        }
    }

    public class CreateCommandHandler<T> : IRequestHandler<CreateCommand<T>, T> where T : class
    {
        private readonly IUnitOfWork _uow;
        public CreateCommandHandler(IUnitOfWork uow) { _uow = uow; }
        public async Task<T> Handle(CreateCommand<T> request, CancellationToken cancellationToken)
        {
            var repo = _uow.GetRepository<T>();
            await repo.AddAsync(request.Entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return request.Entity;
        }
    }

    public class UpdateCommandHandler<T> : IRequestHandler<UpdateCommand<T>, Unit> where T : class
    {
        private readonly IUnitOfWork _uow;
        public UpdateCommandHandler(IUnitOfWork uow) { _uow = uow; }
        public async Task<Unit> Handle(UpdateCommand<T> request, CancellationToken cancellationToken)
        {
            var repo = _uow.GetRepository<T>();
            await repo.UpdateAsync(request.Entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }

    public class DeleteCommandHandler<T> : IRequestHandler<DeleteCommand<T>, Unit> where T : class
    {
        private readonly IUnitOfWork _uow;
        public DeleteCommandHandler(IUnitOfWork uow) { _uow = uow; }
        public async Task<Unit> Handle(DeleteCommand<T> request, CancellationToken cancellationToken)
        {
            var repo = _uow.GetRepository<T>();
            await repo.DeleteAsync(request.Id, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}


