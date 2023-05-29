﻿using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ZmitaCart.Application.Interfaces;
using ZmitaCart.Domain.Common;

namespace ZmitaCart.Infrastructure.Persistence;

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly ApplicationDbContext _dbContext;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public DatabaseSeeder(ApplicationDbContext dbContext, RoleManager<IdentityRole<int>> roleManager)
    {
        _dbContext = dbContext;
        _roleManager = roleManager;
    }

    public async Task Seed()
    {
        if (!await _dbContext.Database.CanConnectAsync())
        {
            return;
        }

        if (!Equals(_dbContext.Roles.Select(r => r.Name), Role.SupportedRoles))
        {
            await SeedRoles();
        }
        
        if(!await _dbContext.Categories.AnyAsync())
        {
            await SeedData();
        }
    }

    private async Task SeedRoles()
    {
        foreach (var supportedRole in Role.SupportedRoles)
        {
            var role = new IdentityRole<int>(supportedRole);
            
            if (!_dbContext.Roles.Contains(role))
            {
                await _roleManager.CreateAsync(role);
            }
        }
    }
    
    private async Task SeedData()
    {
        
    }
}