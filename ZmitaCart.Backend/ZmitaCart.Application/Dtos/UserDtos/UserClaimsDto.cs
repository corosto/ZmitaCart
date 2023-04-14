﻿namespace ZmitaCart.Application.Dtos.UserDtos;

public record UserClaimsDto
{
	public int Id { get; set; }
	public string Email { get; set; } = null!;
	public string FirstName { get; set; } = null!;
	public string Role { get; set; } = null!;
}