using MediatR;
using drTech_backend.Infrastructure.Abstractions;

namespace drTech_backend.Application.Common.Behaviors
{
	public class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
		where TRequest : IRequest<TResponse>
	{
		private readonly IGenericUnitOfWork _unitOfWork;

		public UnitOfWorkBehavior(IGenericUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
		{
			var isTransactional = request is Application.Common.Mediator.ITransactionalRequest;
			if (!isTransactional)
			{
				return await next();
			}

			try
			{
				await _unitOfWork.BeginTransactionAsync(cancellationToken);
				var response = await next();
				await _unitOfWork.SaveChangesAsync(cancellationToken);
				await _unitOfWork.CommitTransactionAsync(cancellationToken);
				return response;
			}
			catch
			{
				await _unitOfWork.RollbackTransactionAsync(cancellationToken);
				throw;
			}
		}
	}
}


