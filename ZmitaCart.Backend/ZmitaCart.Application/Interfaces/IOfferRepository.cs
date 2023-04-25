﻿using ZmitaCart.Application.Dtos.OfferDtos;

namespace ZmitaCart.Application.Interfaces;

public interface IOfferRepository
{
	public Task<int> CreateAsync(CreateOfferDto offerDto);
	public Task<int> UpdateAsync(UpdateOfferDto offerDto);
	public Task DeleteAsync(int userId, int offerId);
}
