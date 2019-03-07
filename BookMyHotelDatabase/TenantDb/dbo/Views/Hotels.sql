
CREATE VIEW [dbo].[Hotels] AS
SELECT HotelId, HotelName, HotelType, AdminEmail, PostalCode, CityCode, @@ServerName as Server, DB_NAME() AS [DatabaseName]
FROM [Hotel]