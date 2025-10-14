using MediatR;
using drTech_backend.Infrastructure.Abstractions;

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
        private readonly IDatabaseService<T> _db;
        public GetAllQueryHandler(IDatabaseService<T> db) { _db = db; }
        public async Task<IReadOnlyList<T>> Handle(GetAllQuery<T> request, CancellationToken cancellationToken)
        {
            var items = await _db.GetAllAsync(cancellationToken);
            return items;
        }
    }

    public class GetByIdQueryHandler<T> : IRequestHandler<GetByIdQuery<T>, T?> where T : class
    {
        private readonly IDatabaseService<T> _db;
        public GetByIdQueryHandler(IDatabaseService<T> db) { _db = db; }
        public async Task<T?> Handle(GetByIdQuery<T> request, CancellationToken cancellationToken)
        {
            return await _db.GetByIdAsync(request.Id, cancellationToken);
        }
    }

    public class CreateCommandHandler<T> : IRequestHandler<CreateCommand<T>, T> where T : class
    {
        private readonly IDatabaseService<T> _db;
        public CreateCommandHandler(IDatabaseService<T> db) { _db = db; }
        public async Task<T> Handle(CreateCommand<T> request, CancellationToken cancellationToken)
        {
            await _db.AddAsync(request.Entity, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            return request.Entity;
        }
    }

    public class UpdateCommandHandler<T> : IRequestHandler<UpdateCommand<T>, Unit> where T : class
    {
        private readonly IDatabaseService<T> _db;
        public UpdateCommandHandler(IDatabaseService<T> db) { _db = db; }
        public async Task<Unit> Handle(UpdateCommand<T> request, CancellationToken cancellationToken)
        {
            await _db.UpdateAsync(request.Entity, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }

    public class DeleteCommandHandler<T> : IRequestHandler<DeleteCommand<T>, Unit> where T : class
    {
        private readonly IDatabaseService<T> _db;
        public DeleteCommandHandler(IDatabaseService<T> db) { _db = db; }
        public async Task<Unit> Handle(DeleteCommand<T> request, CancellationToken cancellationToken)
        {
            await _db.DeleteAsync(request.Id, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}


