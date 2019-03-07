CREATE VIEW [dbo].[HotelBookings] AS 
    SELECT (SELECT TOP 1 HotelId FROM Hotels) AS HotelId,
	 (select top 1 RoomId from Rooms) as RoomId, 
	 (select top 1 GuestId from Guests) as GuestId,
	 (select Top 1 GuestName from Guests) as GuestName,
	 (select Top 1 HotelName from Hotel) as HotelName,
	 BookingId,BookingPurchaseId
	 FROM [Bookings]