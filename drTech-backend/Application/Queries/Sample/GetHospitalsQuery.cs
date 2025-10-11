using MediatR;

namespace drTech_backend.Application.Queries.Sample
{
    public record GetHospitalsQuery() : IRequest<IReadOnlyList<Domain.Entities.Hospital>>;

    public class GetHospitalsQueryHandler : IRequestHandler<GetHospitalsQuery, IReadOnlyList<Domain.Entities.Hospital>>
    {
        private readonly Infrastructure.Repositories.IRepository<Domain.Entities.Hospital> _repo;

        public GetHospitalsQueryHandler(Infrastructure.Repositories.IRepository<Domain.Entities.Hospital> repo)
        {
            _repo = repo;
        }

        public Task<IReadOnlyList<Domain.Entities.Hospital>> Handle(GetHospitalsQuery request, CancellationToken cancellationToken)
        {
            var list = _repo.Query().ToList();
            return Task.FromResult((IReadOnlyList<Domain.Entities.Hospital>)list);
        }
    }
}


