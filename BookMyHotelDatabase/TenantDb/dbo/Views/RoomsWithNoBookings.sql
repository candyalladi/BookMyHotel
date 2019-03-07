CREATE VIEW [dbo].[RoomsWithNoBookings]
    AS SELECT RoomId, RoomName, 
	RoomType, (select Top 1 HotelId from Hotels) as HotelId from dbo.Rooms as e
    WHERE (SELECT Count(*) FROM dbo.Bookings AS t WHERE t.RoomId=e.RoomId) = 0


