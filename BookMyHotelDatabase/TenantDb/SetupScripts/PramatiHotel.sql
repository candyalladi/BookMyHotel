-- Pramati Hotel Initialization
--
-- All existing data will be deleted (excluding reference tables)
--
-----------------------------------------------------------------

-- Delete all current data
DELETE FROM [dbo].[HotelOffers]
DELETE FROM [dbo].[BookingPurchases]
DELETE FROM [dbo].[Guests]
DELETE FROM [dbo].[RoomPrices]
DELETE FROM [dbo].[Rooms]
DELETE FROM [dbo].[Hotel]
DELETE FROM [dbo].[Offers]
DELETE FROM [dbo].[HotelTypes]
DELETE FROM [dbo].[Cities]
DELETE FROM [dbo].[Bookings]

-- Ids pre-computed as md5 hash of UTF8 encoding of the normalized tenant name, converted to Int 
-- These are the id values that will be used by the client application and PowerShell scripts to 
-- retrieve tenant-specific data.   
DECLARE @HotelId INT = 1976168774

-- Venue
INSERT INTO [dbo].[Hotel]
   ([HotelId],[HotelName],[HotelType],[AdminEmail],[AdminPassword],[PostalCode],[CityCode],[Lock])
     VALUES
           (@HotelId,'5Star','5 Star','admin@pramati.com',NULL,'500001','HYD','X')
GO

-- Sections
SET IDENTITY_INSERT [dbo].[Rooms] ON
INSERT INTO [dbo].[Rooms]
    ([RoomId],[RoomName],[RoomType],[HotelId],[StandardPrice])
    VALUES
    (1,'King Suite','Luxury',(Select Top 1 HotelId from Hotel),14000.00),
    (2,'Delux Suite','Delux',(Select top 1 HotelId from Hotel),12000.00),
    (3,'Presidential Suite','Presidential',(Select top 1  HotelId from Hotel),60000.00),
    (4,'Lanai','Sea Scape view',(Select top 1 HotelId from Hotel),30000.00)
;
SET IDENTITY_INSERT [dbo].[Rooms] OFF

-- Events
SET IDENTITY_INSERT [dbo].[Bookings] ON
INSERT INTO [dbo].[Bookings]
    ([BookingId],[GuestId],[GuestName],[RoomId],[RoomName],[Checkin_Date],[Checkout_Date],[BookingPurchaseId],[Payment])
    VALUES
    (1,501,'Chandrapaul',501,'A501','2019-03-05 12:45:00','2019-03-07 12:45:00',1501,1),
	(2,502,'Karthik',502,'A502','2019-03-05 12:45:00','2019-03-07 12:45:00',1502,1),
	(3,503,'Sumeet',503,'A503','2019-03-05 12:45:00','2019-03-07 12:45:00',1503,0),
	(4,504,'Ravi',504,'A504','2019-03-05 12:45:00','2019-03-07 12:45:00',1504,0),
	(5,505,'Ajay',505,'A505','2019-03-05 12:45:00','2019-03-07 12:45:00',1505,1),
	(6,506,'Raj Sekhar',506,'A506','2019-03-05 12:45:00','2019-03-07 12:45:00',1506,1),
	(7,507,'Srinivas',507,'A507','2019-03-05 12:45:00','2019-03-07 12:45:00',1507,1),
	(8,508,'Sri Ram',508,'A508','2019-03-05 12:45:00','2019-03-07 12:45:00',1508,0),
	(9,501,'Anirudh',509,'A509','2019-03-05 12:45:00','2019-03-07 12:45:00',1509,0),
	(10,501,'Kamal',510,'A510','2019-03-05 12:45:00','2019-03-07 12:45:00',1510,1),
	(11,501,'Kalyan',511,'A511','2019-03-05 12:45:00','2019-03-07 12:45:00',1511,1)    
;
SET IDENTITY_INSERT [dbo].[Bookings] OFF
GO

-- Event Sections
INSERT INTO [dbo].[RoomPrices]
    ([RoomId],[HotelId],[Price])
    VALUES
    (1,1,10000.00),
    (1,2,8000.00),
    (1,3,6000.00),
    (1,4,4000.00),
    (2,1,10000.00),
    (2,2,8000.00),
    (2,3,6000.00),
    (2,4,4000.00),
    (3,1,10000.00),
    (3,2,8000.00),
    (3,3,6000.00),
    (3,4,4000.00),   
    (4,1,10000.00),
    (4,2,8000.00),
    (4,3,6000.00),
    (4,4,4000.00),
    (5,1,10000.00),
    (5,2,8000.00),
    (5,3,6000.00),
    (5,4,4000.00),
    (6,1,10000.00),
    (6,2,8000.00),
    (6,3,6000.00),
    (6,4,4000.00),
    (7,1,10000.00),
    (7,2,8000.00),
    (7,3,6000.00),
    (7,4,4000.00),
    (8,1,10000.00),
    (8,2,8000.00),
    (8,3,6000.00),
    (8,4,4000.00),
    (9,1,10000.00),
    (9,2,8000.00),
    (9,3,6000.00),
    (9,4,4000.00),
    (10,1,10000.00),
    (10,2,8000.00),
    (10,3,6000.00),
    (10,4,4000.00),
    (11,1,15000.00),
    (11,2,10000.00),
    (11,3,9000.00),
    (11,4,6000.00)
;

