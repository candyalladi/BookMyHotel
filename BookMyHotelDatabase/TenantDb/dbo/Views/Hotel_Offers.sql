

	CREATE VIEW [dbo].[HotelOffers]
	AS
	SELECT offer.Discount, offer.OfferValidDate, offer.IsOfferAvailable
	FROM [dbo].[Offers] as offer
	INNER JOIN [dbo].[Hotel] as hotel
	ON offer.HotelId = hotel.HotelId

