using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ZmitaCart.Application.Interfaces;
using ZmitaCart.Domain.Entities;
using ZmitaCart.Infrastructure.Persistence;

namespace ZmitaCart.Infrastructure.Repositories;

public class PictureRepository : IPictureRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PictureRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(int userId, int offerId, IEnumerable<IFormFile> images)
    {
        var offer = await _dbContext.Offers.FirstOrDefaultAsync(o => o.Id == offerId)
                    ?? throw new InvalidDataException("Offer does not exist");

        if (offer.UserId != userId) throw new UnauthorizedAccessException("User does not have access to this offer");

        foreach (var image in images)
        {
            if (image.Length <= 0) return;

            var creationTime = DateTimeOffset.Now;
            var imageName =
                $"{offerId}_{creationTime:ddMMyyyyhhmmssfff}_{new Random().Next(0, 10000000)}.{image.FileName.Split('.').Last()}";
            var filePath = Path.Combine(Path.GetFullPath("wwwroot"), imageName);

            await using (var stream = new FileStream(filePath, FileMode.Append))
            {
                await image.CopyToAsync(stream);
            }

            var picture = new Picture
            {
                OfferId = offerId,
                Offer = offer,
                Name = imageName,
                PictureUrl = filePath,
                CreationTime = creationTime
            };

            await _dbContext.Pictures.AddAsync(picture);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveAsync(int userId, int offerId, IEnumerable<int> imagesIds)
    {
        var offer = await _dbContext.Offers.FirstOrDefaultAsync(o => o.Id == offerId)
                    ?? throw new InvalidDataException("Offer does not exist");

        if (offer.UserId != userId) throw new UnauthorizedAccessException("User does not have access to this offer");

        var pictures = await _dbContext.Pictures
            .Where(p => imagesIds.Contains(p.Id))
            .ToListAsync();

        foreach (var filePath in pictures.Select(picture => Path.Combine(Path.GetFullPath("wwwroot"), picture.Name)).Where(File.Exists))
        {
            File.Delete(filePath);
        }

        _dbContext.Pictures.RemoveRange(pictures);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveAllAsync(int userId, int offerId)
    {
        var offer = await _dbContext.Offers
                        .Include(o => o.Pictures)
                        .FirstOrDefaultAsync(o => o.Id == offerId)
                    ?? throw new InvalidDataException("Offer does not exist");

        if (offer.UserId != userId) throw new UnauthorizedAccessException("User does not have access to this offer");

        if (offer.Pictures is null) return;

        foreach (var filePath in offer.Pictures.Select(picture => Path.Combine(Path.GetFullPath("wwwroot"), picture.Name))
                     .Where(File.Exists))
        {
            File.Delete(filePath);
        }

        _dbContext.Pictures.RemoveRange(offer.Pictures);
        await _dbContext.SaveChangesAsync();
    }
}