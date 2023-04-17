﻿using ZmitaCart.Application.Dtos.UserDtos;

namespace ZmitaCart.Application.Interfaces;

public interface IUserRepository
{
	public Task RegisterAsync(RegisterUserDto registerUserDto);
	public Task<string> LoginAsync(LoginUserDto loginUserDto);
	public Task LogoutAsync();
	public Task AddRoleAsync(string userEmail, string role);
}