﻿using FluentValidation;

namespace ZmitaCart.Application.Commands.UserCommands.AddRoleForUser;

public class AddRoleForUserValidator : AbstractValidator<AddRoleForUserCommand>
{
	public AddRoleForUserValidator()
	{
		RuleFor(x => x.Role)
			.NotNull().WithMessage("Enter role");

		RuleFor(x => x.UserEmail)
			.NotNull().WithMessage("Enter email");
	}
}