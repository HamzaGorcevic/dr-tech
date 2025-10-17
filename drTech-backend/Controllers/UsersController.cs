using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using drTech_backend.Application.Common.Mediator;
using AutoMapper;
using drTech_backend.Application.Common.DTOs;

namespace drTech_backend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class UsersController : ControllerBase
	{
		private readonly IMediator _mediator;
		private readonly IMapper _mapper;
		public UsersController(IMediator mediator, IMapper mapper) { _mediator = mediator; _mapper = mapper; }

		[HttpGet]
		[Authorize(Roles = "HospitalAdmin,InsuranceAgency,Doctor,InsuredUser")]
		public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
		{
			var items = await _mediator.Send(new GetAllQuery<Domain.Entities.User>(), cancellationToken);
			return Ok(items);
		}
	}
}



