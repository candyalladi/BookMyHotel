CREATE VIEW [dbo].[BookingsFacts] AS
    SELECT      v.HotelId, Convert(int, HASHBYTES('md5',c.Email)) AS GuestEmailId, c.PostalCode AS CustomerPostalCode, c.CityCode AS GuestCityCode,
	            tp.BookingPurchaseId, tp.BookedDate, tp.TotalPrice, tp.RowVersion AS BookkingPurchaseRowVersion,
	            e.RoomId, t.GuestId, t.HotelName 
	FROM        [dbo].[BookingPurchases] AS tp 
	INNER JOIN [dbo].[Bookings] AS t ON t.BookingPurchaseId = tp.BookingPurchaseId 
	INNER JOIN [dbo].[Rooms] AS e ON t.RoomId = e.RoomId 
	INNER JOIN [dbo].[Guests] AS c ON tp.GuestId = c.GuestId
	INNER Join [dbo].[Hotel] as v on 1=1