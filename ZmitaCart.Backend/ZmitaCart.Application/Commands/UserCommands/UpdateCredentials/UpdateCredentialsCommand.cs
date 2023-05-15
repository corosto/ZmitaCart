﻿using MediatR;
using ZmitaCart.Domain.ValueObjects;

namespace ZmitaCart.Application.Commands.UserCommands.UpdateCredentials;

public record UpdateCredentialsCommand : IRequest
{
	public string? PhoneNumber { get; init; }
	public Address? Address { get; init; }
}