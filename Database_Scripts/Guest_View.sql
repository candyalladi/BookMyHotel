CREATE VIEW [dbo].[rawGuests]
AS 	
SELECT  (SELECT TOP 1 HotelId FROM Hotels) AS HotelId, 
(select top 1 HotelName from Hotels) as HotelName,
(select top 1 RoomId from Rooms) as RoomId,
Convert(int, HASHBYTES('md5',c.Email)) AS GuestEmailId, 
            c.PostalCode AS GuestPostalCode, c.CityCode AS CustomerCityCode,
	            c.RowVersion AS CustomerRowVersion
FROM        [dbo].[Guests]  as c
GO 


