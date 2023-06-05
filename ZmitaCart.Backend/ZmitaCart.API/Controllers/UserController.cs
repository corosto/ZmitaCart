﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZmitaCart.API.Common;
using ZmitaCart.Application.Commands.UserCommands.AddRole;
using ZmitaCart.Application.Commands.UserCommands.DeleteFeedback;
using ZmitaCart.Application.Commands.UserCommands.GiveFeedback;
using ZmitaCart.Application.Commands.UserCommands.LoginUser;
using ZmitaCart.Application.Commands.UserCommands.Register;
using ZmitaCart.Application.Commands.UserCommands.UpdateCredentials;
using ZmitaCart.Application.Commands.UserCommands.UpdateFeedback;
using ZmitaCart.Application.Common;
using ZmitaCart.Application.Dtos.OfferDtos;
using ZmitaCart.Application.Dtos.UserDtos;
using ZmitaCart.Application.Queries.OfferQueries.GetUserOffers;
using ZmitaCart.Application.Queries.UserQueries.GetData;
using ZmitaCart.Application.Queries.UserQueries.GetFeedback;
using ZmitaCart.Application.Queries.UserQueries.LogoutUser;
using ZmitaCart.Domain.Common;

namespace ZmitaCart.API.Controllers;

[Route("user")]
public class UserController : ApiController
{
	public UserController(IMediator mediator) : base(mediator)
	{
	}

	[HttpGet]
	public async Task<ActionResult<UserDataDto>> GetUserData()
	{
		return await mediator.Send(new GetDataQuery()).Then(
			s => Ok(s.Value),
			err => StatusCode(err.StatusCode, err.ToList()));
	}
	
	[HttpPost("register")]
	[AllowAnonymous]
	public async Task<ActionResult> Register([FromBody] RegisterUserCommand command)
	{
		return await mediator.Send(command).Then(
			s => Ok(),
			err => StatusCode(err.StatusCode, err.ToList()));
	}

	[HttpPost("login")]
	[AllowAnonymous]
	public async Task<ActionResult<TokensDto>> Login([FromBody] LoginUserCommand command)
	{
		return await mediator.Send(command).Then(
			s => Ok(s.Value),
			err => StatusCode(err.StatusCode, err.ToList()));
	}

	[HttpPost("logout")]
	public async Task<ActionResult> Logout()
	{
		return await mediator.Send(new LogoutUserQuery()).Then(
			s => Ok(),
			err => StatusCode(err.StatusCode, err.ToList()));
	}

	[HttpPost("addRole")]
	[Authorize(Roles = Role.administrator)]
	public async Task<ActionResult> AddRoleForUser([FromBody] AddRoleForUserCommand command)
	{
		return await mediator.Send(command).Then(
			s => Ok(),
			err => StatusCode(err.StatusCode, err.ToList()));
	}

	[HttpPut("updateCredentials")]
	public async Task<ActionResult> UpdateCredentials([FromBody] UpdateCredentialsCommand command)
	{
		return await mediator.Send(command).Then(
			s => Ok(),
			err => StatusCode(err.StatusCode, err.ToList()));
	}

	[HttpPost("feedback")]
	public async Task<ActionResult<int>> GiveFeedback([FromBody] GiveFeedbackCommand command)
	{
		return await mediator.Send(command).Then(
			s => Ok(s.Value),
			err => StatusCode(err.StatusCode, err.ToList()));
	}

	[HttpPut("feedback")]
	public async Task<ActionResult<int>> UpdateFeedback([FromBody] UpdateFeedbackCommand command)
	{
		return await mediator.Send(command).Then(
			s => Ok(s.Value),
			err => StatusCode(err.StatusCode, err.ToList()));
	}

	[HttpDelete("feedback/{id:int}")]
	public async Task<ActionResult> DeleteFeedback([FromRoute] int id)
	{
		return await mediator.Send(new DeleteFeedbackCommand(id)).Then(
			s => Ok(),
			err => NotFound(err.ToList()));
	}

	[HttpGet("feedback")]
	public async Task<ActionResult<PaginatedList<FeedbackDto>>> GetFeedback([FromQuery] GetFeedbackQuery query)
	{
		return await mediator.Send(query).Then(
			s => Ok(s.Value),
			err => StatusCode(err.StatusCode, err.ToList()));
	}
	
	[HttpGet("offer")]
	public async Task<ActionResult<PaginatedList<OfferInfoDto>>> GetOffers([FromQuery] GetUserOffersQuery query)
	{
		return await mediator.Send(query).Then(
			s => Ok(s.Value),
			err => StatusCode(err.StatusCode, err.ToList()));
	}
}