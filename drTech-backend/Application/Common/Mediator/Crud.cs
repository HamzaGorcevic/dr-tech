using MediatR;
using drTech_backend.Infrastructure.Abstractions;
using FluentValidation;

namespace drTech_backend.Application.Common.Mediator
{
    // Marker to indicate a request should be executed transactionally
    public interface ITransactionalRequest {}

    // Queries
    public record GetAllQuery<T>() : IRequest<IReadOnlyList<T>> where T : class;
    public record GetByIdQuery<T>(Guid Id) : IRequest<T?> where T : class;

    // Commands
    public record CreateCommand<T>(T Entity) : IRequest<T>, ITransactionalRequest where T : class;
    public record UpdateCommand<T>(T Entity) : IRequest, ITransactionalRequest where T : class;
    public record DeleteCommand<T>(Guid Id) : IRequest, ITransactionalRequest where T : class;
}


