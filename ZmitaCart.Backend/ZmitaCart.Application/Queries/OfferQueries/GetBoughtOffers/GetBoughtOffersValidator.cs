﻿using FluentValidation;

namespace ZmitaCart.Application.Queries.OfferQueries.GetBoughtOffers;

public class GetBoughtOffersValidator : AbstractValidator<GetBoughtOffersQuery>
{
	public GetBoughtOffersValidator()
	{
		RuleFor(x => x.PageNumber)
			.GreaterThanOrEqualTo(1).WithMessage("PageNumber must be greater than or equal to 1")
			.Must(x => x.HasValue).When(x => x.PageSize.HasValue).WithMessage("PageNumber is required");


		RuleFor(x => x.PageSize)
			.GreaterThanOrEqualTo(1).WithMessage("PageSize must be greater than or equal to 1")
			.Must(x => x.HasValue).When(x => x.PageNumber.HasValue).WithMessage("PageSize is required");
	}
}