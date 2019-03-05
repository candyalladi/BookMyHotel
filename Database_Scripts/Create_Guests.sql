/****** Object:  Table [dbo].[Guests]    Script Date: 04-Mar-19 1:09:50 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Guests](
	[GuestId] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[Email] [varchar](128) NOT NULL,
	[Password] [nvarchar](30) NULL,
	[PostalCode] [nvarchar](20) NULL,
	[CityCode] [char](3) NOT NULL,
	[RoomId] [int] NULL,
	[HotelName] [nvarchar](50) NULL,
	[RowVersion] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[GuestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_Email] UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Guests]  WITH CHECK ADD  CONSTRAINT [FK_Guests_Cities] FOREIGN KEY([CityCode])
REFERENCES [dbo].[Cities] ([CityCode])
GO

ALTER TABLE [dbo].[Guests] CHECK CONSTRAINT [FK_Guests_Cities]
GO


