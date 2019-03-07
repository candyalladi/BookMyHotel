/****** Object:  View [dbo].[rawBookings]    Script Date: 04-Mar-19 3:24:20 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[rawBookings]
	AS 	 SELECT      v.HotelId, Convert(int, HASHBYTES('md5',c.Email)) AS GuestEmailId,
	            tp.BookingPurchaseId, tp.BookedDate, tp.TotalPrice, tp.RowVersion AS BookingPurchaseRowVersion,
	            e.RoomId, t.GuestId, t.HotelName
	FROM        [dbo].[BookingPurchases] AS tp 
	INNER JOIN [dbo].[Bookings] AS t ON t.BookingPurchaseId = tp.BookingPurchaseId 
	INNER JOIN [dbo].[Rooms] AS e ON t.RoomId = e.RoomId 
	INNER JOIN [dbo].[Guests] AS c ON tp.GuestId = c.GuestId
	INNER join [dbo].[Hotel] as v on 1=1

GO


