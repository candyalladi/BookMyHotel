CREATE TABLE [dbo].[HotelTypes]
(
    [HotelType]                 NVARCHAR(30) NOT NULL,
	[HotelTypeName]             NVARCHAR(30) NOT NULL,  
    [RoomTypeName]             NVARCHAR(30) NOT NULL, 
	[RoomTypeShortName]        NVARCHAR(20) NOT NULL,
	[RoomTypeShortNamePlural]  NVARCHAR(20) NOT NULL,
    PRIMARY KEY CLUSTERED ([HotelType] ASC)
)
GO
