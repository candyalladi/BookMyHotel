    CREATE VIEW [dbo].[HotelBookingPurchases] AS
    SELECT (SELECT TOP 1 HotelId FROM Hotels) AS HotelId, BookingPurchaseId, BookedDate, TotalPrice, GuestId 
	FROM [BookingPurchases]