CREATE TABLE [dbo].[Cities]
(
	[CityCode]   CHAR(3) NOT NULL,
	[CityName]   NVARCHAR(50) NOT NULL,
    PRIMARY KEY CLUSTERED ([CityCode] ASC)
)

GO