CREATE VIEW [dbo].[rawHotels]
	AS 	SELECT  v.HotelId, v.HotelName, v.HotelType,v.PostalCode as HotelPostalCode,  CityCode AS HotelCityCode,
	            --(SELECT SUM (Floors * RoomsPerFloor) FROM [dbo].[Hotel]) AS HotelCapacity,
				-- Need to add these columns
	            v.RowVersion AS HotelRowVersion
	FROM        [dbo].[Hotel] as v
GO
