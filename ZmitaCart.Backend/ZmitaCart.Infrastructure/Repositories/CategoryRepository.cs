using FluentResults;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ZmitaCart.Application.Common.Errors;
using ZmitaCart.Application.Dtos.CategoryDtos;
using ZmitaCart.Application.Interfaces;
using ZmitaCart.Domain.Entities;
using ZmitaCart.Infrastructure.Persistence;

namespace ZmitaCart.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CategoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<int>> Create(string name, int? parentId, string? iconName)
    {
        if (await _dbContext.Categories.AnyAsync(c => c.Name == name))
        {
            return Result.Fail(new AlreadyExistsError("Category with input name already exists"));
        }

        var category = new Category { Name = name, IconName = iconName };
        if (parentId is null)
        {
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            return category.Id;
        }

        var parent = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == parentId);
        if (parent is null)
        {
            return Result.Fail(new NotFoundError("Parent category with input id doesn't exist"));
        }

        category.Parent = parent;
        category.ParentId = parentId;
        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();
        return category.Id;
    }

    public async Task<Result<int>> Update(int id, string? name, int? parentId, string? iconName)
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
        {
            return Result.Fail(new NotFoundError("Item does not exist"));
        }

        if (name is not null)
        {
            category.Name = name;
        }

        if (iconName is not null)
        {
            category.IconName = iconName;
        }

        if (parentId is not null)
        {
            var parent = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == parentId);
            if (parent is null)
            {
                return Result.Fail(new NotFoundError("Parent category with input id doesn't exist"));
            }

            var currentCategory = (await _dbContext.Categories
                .Include(c => c.Children)
                .ToListAsync()).FirstOrDefault(c => c.Id == id);

            var result = CheckChildrenId(currentCategory!, parentId.Value);
            if (result.IsFailed)
            {
                return result;
            }

            category.Parent = parent;
            category.ParentId = parentId;
        }
        else
        {
            category.ParentId = null;
            category.Parent = null;
        }

        _dbContext.Categories.Update(category);
        await _dbContext.SaveChangesAsync();
        return category.Id;
    }

    private Result CheckChildrenId(Category category, int parentId)
    {
        if (category.Id == parentId)
        {
            return Result.Fail(new ArgumentError("Category cannot have itself as a child"));
        }

        if (!category.Children.Any())
        {
            return Result.Ok();
        }

        return category.Children.Select(child => CheckChildrenId(child, parentId)).Any(result => result.IsFailed)
            ? Result.Fail(new ArgumentError("Category cannot have itself as a child"))
            : Result.Ok();
    }

    public async Task<Result> Delete(int id)
    {
        var categoryToRemove = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);

        if (categoryToRemove is null)
        {
            Result.Fail(new NotFoundError("Item does not exist"));
        }

        await _dbContext.Categories
            .Where(c => c.ParentId == id)
            .ForEachAsync(c => c.ParentId = null);

        _dbContext.Categories.Remove(categoryToRemove!);
        await _dbContext.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<IEnumerable<SuperiorCategoryDto>>> GetAllSuperiors()
    {
        return await _dbContext.Categories
            .Where(c => c.ParentId == null)
            .ProjectToType<SuperiorCategoryDto>()
            .ToListAsync();
    }

    public async Task<Result<IEnumerable<CategoryDto>>> GetSuperiorsWithFewChildren(int? childrenCount)
    {
        var categories = await _dbContext.Categories
            .Where(c => c.ParentId == null)
            .Include(c => c.Children)
            .ProjectToType<CategoryDto>()
            .ToListAsync();

        childrenCount ??= 0; //get superiors only

        foreach (var category in categories)
        {
            await FillChildren(category.Children, childrenCount.Value);
        }

        return categories;
    }


    public async Task<Result<IEnumerable<CategoryDto>>> GetCategoriesBySuperiorId(int superiorId, int? childrenCount)
    {
        if (await _dbContext.Categories.AnyAsync(c => c.Id == superiorId) is false)
        {
            return Result.Fail(new NotFoundError("Superior category with input id doesn't exist"));
        }

        var categories = await _dbContext.Categories
            .Where(c => c.Id == superiorId)
            .Include(c => c.Children)
            .ProjectToType<CategoryDto>()
            .ToListAsync();

        childrenCount ??= -1; // -1 means all children

        foreach (var category in categories)
        {
            await FillChildren(category.Children, childrenCount.Value);
        }

        return categories;
    }

    private async Task FillChildren(IEnumerable<CategoryDto?>? children, int childrenCount)
    {
        if (childrenCount == 0 || children is null) return;
        childrenCount--;

        foreach (var child in children)
        {
            if (child is null) continue;

            var temp = await _dbContext.Categories
                .Where(c => c.ParentId == child.Id)
                .ProjectToType<CategoryDto>()
                .ToListAsync();

            child.Children = temp.Any() && childrenCount != 0 ? temp : null;

            await FillChildren(child.Children, childrenCount);
        }
    }
}