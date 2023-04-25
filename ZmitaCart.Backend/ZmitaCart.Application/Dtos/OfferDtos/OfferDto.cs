using ZmitaCart.Application.Dtos.UserDtos;
using ZmitaCart.Domain.Entities;
using ZmitaCart.Domain.ValueObjects;

namespace ZmitaCart.Application.Dtos.OfferDtos;

public class OfferDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public bool IsAvailable { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string Condition { get; set; } = null!;
    public virtual UserDto User { get; set; } = null!;
    public Address Address { get; set; } = null!;
    public virtual ICollection<string>? PicturesUrls { get; set; }
}