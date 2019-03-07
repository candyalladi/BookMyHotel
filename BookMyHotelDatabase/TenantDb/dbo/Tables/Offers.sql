/****** Object:  Table [dbo].[Offers]    Script Date: 04-Mar-19 2:28:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Offers](
	[OfferId] [int] IDENTITY(1,1) NOT NULL,
	[HotelId] [int] NOT NULL,
	[Discount] [decimal](18, 0) NULL,
	[OfferValidDate] [datetime] NOT NULL,
	[IsOfferAvailable] [bit] NULL,
	[RowVersion] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[OfferId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Offers] ADD  DEFAULT ('True') FOR [IsOfferAvailable]
GO

ALTER TABLE [dbo].[Offers]  WITH CHECK ADD  CONSTRAINT [FK_Offers_Hotel] FOREIGN KEY([HotelId])
REFERENCES [dbo].[Hotel] ([HotelId])
GO

ALTER TABLE [dbo].[Offers] CHECK CONSTRAINT [FK_Offers_Hotel]
GO


