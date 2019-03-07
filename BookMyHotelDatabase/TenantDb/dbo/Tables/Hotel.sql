/****** Object:  Table [dbo].[Hotel]    Script Date: 04-Mar-19 1:14:23 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Hotel](
	[HotelId] [int] NOT NULL,
	[HotelName] [nvarchar](50) NOT NULL,
	[HotelType] [nvarchar](30) NOT NULL,
	[AdminEmail] [nvarchar](128) NOT NULL,
	[AdminPassword] [nvarchar](30) NULL,
	[PostalCode] [nvarchar](20) NULL,
	[CityCode] [char](3) NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
	[Lock] [char](1) NOT NULL,
 CONSTRAINT [PK_Hotel] PRIMARY KEY CLUSTERED 
(
	[HotelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Hotel] ADD  DEFAULT ('X') FOR [Lock]
GO

ALTER TABLE [dbo].[Hotel]  WITH CHECK ADD  CONSTRAINT [FK_Hotels_Cities] FOREIGN KEY([CityCode])
REFERENCES [dbo].[Cities] ([CityCode])
GO

ALTER TABLE [dbo].[Hotel] CHECK CONSTRAINT [FK_Hotels_Cities]
GO

ALTER TABLE [dbo].[Hotel]  WITH CHECK ADD  CONSTRAINT [FK_Hotels_HotelTypes] FOREIGN KEY([HotelType])
REFERENCES [dbo].[HotelTypes] ([HotelType])
GO

ALTER TABLE [dbo].[Hotel] CHECK CONSTRAINT [FK_Hotels_HotelTypes]
GO

ALTER TABLE [dbo].[Hotel]  WITH CHECK ADD  CONSTRAINT [CK_Hotel_Singleton] CHECK  (([Lock]='X'))
GO

ALTER TABLE [dbo].[Hotel] CHECK CONSTRAINT [CK_Hotel_Singleton]
GO


