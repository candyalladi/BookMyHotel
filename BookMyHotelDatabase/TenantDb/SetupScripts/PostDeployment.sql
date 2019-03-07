/*

Post-Deployment Script Template                            
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.        
 Use SQLCMD syntax to include a file in the post-deployment script.            
 Example:      :r .\myfile.sql                                
 Use SQLCMD syntax to reference a variable in the post-deployment script.        
 Example:      :setvar TableName MyTable                            
               SELECT * FROM [$(TableName)]                    
--------------------------------------------------------------------------------------
*/

-- Country code is based on ISO 3166-1 alpha-3 codes.  See https://en.wikipedia.org/wiki/ISO_3166-1_alpha-3

INSERT INTO [dbo].[Cities]
    ([CityCode],[CityName])
VALUES
    ('HYD', 'Hyderabad'),
	('BLR', 'Bengaluru'),
	('MUM', 'Mumbai'),
	('DEL', 'Delhi'),
	('CHN', 'Chennai'),
	('VJY', 'Vijayawada')
GO

INSERT INTO [dbo].[HotelTypes]
    ([HotelType],[HotelTypeName],[RoomTypeName],[RoomTypeShortName],[RoomTypeShortNamePlural])
VALUES
     ('verysmall','Very Small Hotel','SingleRoom', 'Double Room','Twin Sharing'),
    ('small','Small Hotel','Twin Sharing','Single Room','Double Room'),
    ('medium','Medium Hotel','Double Room','Single Room','Twin Sharing'),
    ('Large','Large Hotel','Twin Double Room','Double Room','Delux Room'),
    ('B&B','Bread & Breakfast','Transit', 'Trans','Transits'),
    ('suitehotels','Living Room','Suite Bedroom', 'Hall','Halls'),
    ('3star', '3 Star', 'Double Bedroom', 'Double', 'Doubles'),
    ('4star', '4 Star', 'Furnished', 'Furn','Fully'),
    ('5star','5 Star','King Suite','KSuite', 'KSuits'),
    ('airporthotel','Air Port Hotel','Presidential Suite','PSuite','PSuits'), 
    ('motel','Motel','Transit Guest', 'Transit', 'Transits')   
GO

-- Ids pre-computed as md5 hash of UTF8 encoding of the normalized tenant name, converted to Int 
-- These are the id values that will be used by the client application and PowerShell scripts to 
-- retrieve tenant-specific data.   
DECLARE @HotelId INT = -1209177223

INSERT INTO [dbo].Hotel
    ([HotelId], [HotelName],[HotelType],[AdminEmail],[CityCode],[Lock])         
VALUES
    (@HotelId,'Hotel Name','B&B','admin@email.com','HYD','X'),
	(@HotelId,'Hotel Name','3Star','admin@email.com','BLR','X'),
	(@HotelId,'Hotel Name','Large','admin@email.com','CHN','X'),
	(@HotelId,'Hotel Name','SuiteHotel','admin@email.com','MUM','X'),
	(@HotelId,'Hotel Name','5Star','admin@email.com','DEL','X'),
	(@HotelId,'Hotel Name','Motel','admin@email.com','VJY','X')
GO


-- Sections
SET IDENTITY_INSERT [dbo].[Rooms] ON;

INSERT INTO [dbo].[Rooms]
    ([RoomId],[RoomName],[RoomType],[HotelId])
VALUES
    (501,'A501','Luxury',@HotelId),
    (502,'A502','Delux',@HotelId),
	(503,'A503','Standard',@HotelId),
	(504,'A504','Suite',@HotelId),
	(505,'A505','Presidential Suite',@HotelId),
	(506,'A506','Double Bed',@HotelId),
	(507,'A507','Twin Sharing',@HotelId),
	(508,'A508','King Suite',@HotelId),
	(509,'A509','Hollywood Twin',@HotelId),
	(510,'A510','Standard',@HotelId),
	(511,'A511','Luxury',@HotelId)

SET IDENTITY_INSERT [dbo].[Rooms] OFF
GO

-- Events
SET IDENTITY_INSERT [dbo].[Bookings] ON;

INSERT INTO [dbo].[Bookings]
    ([BookingId],[GuestName],[HotelName],[Checkin_Date],[Checkout_Date],[RoomId],[RoomName],[Payment],[BookingPurchaseId])     
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

SET IDENTITY_INSERT [dbo].[Rooms] OFF
GO

-- Event Sections
INSERT INTO [dbo].[RoomPrices]
   ([RoomId],[HotelId],[Price])
VALUES
    (1,1,4000.00),
    (1,2,2000.00),
    (2,1,4000.00),
    (2,2,2000.00),    
    (3,1,4000.00),
    (3,2,2000.00),
    (4,1,4000.00),
    (4,2,2000.00),
    (5,1,4000.00),
    (5,2,2000.00);
GO
