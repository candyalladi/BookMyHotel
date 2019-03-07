CREATE VIEW [dbo].[HotelOffers]
	AS SELECT Discount, OfferValidDate,IsOfferAvailable from Offers o
	Inner join Hotel h
	on o.HotelId = h.HotelId