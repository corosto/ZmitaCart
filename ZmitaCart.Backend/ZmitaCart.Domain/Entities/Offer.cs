﻿using ZmitaCart.Domain.Common.Models;

namespace ZmitaCart.Domain.Entities;

public class Offer : AggregateRoot<int>
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public bool IsAvailable { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string Condition { get; set; } = null!;
    public int CategoryId { get; set; }
    public virtual Category Category { get; set; } = null!;
    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual List<UserOffer> Favorites { get; set; }  = new();
    public virtual List<Bought> Bought { get; set; } = new();
    public virtual List<Picture> Pictures { get; set; }  = new();
    public virtual List<CategoryOffer> CategoryOffers { get; set; }  = new(); //TODO remove
}