/****** Object:  Table [dbo].[Rooms]    Script Date: 04-Mar-19 1:13:59 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Rooms](
	[RoomId] [int] IDENTITY(1,1) NOT NULL,
	[RoomName] [nvarchar](30) NOT NULL,
	[RoomType] [nvarchar](30) NULL,
	[HotelId] [int] NOT NULL,
	[StandardPrice] [money] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoomId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Rooms] ADD  DEFAULT ('Standard') FOR [RoomType]
GO

ALTER TABLE [dbo].[Rooms] ADD  DEFAULT ((10)) FOR [StandardPrice]
GO

ALTER TABLE [dbo].[Rooms]  WITH CHECK ADD  CONSTRAINT [FK_Rooms_Hotel] FOREIGN KEY([HotelId])
REFERENCES [dbo].[Hotel] ([HotelId])
GO

ALTER TABLE [dbo].[Rooms] CHECK CONSTRAINT [FK_Rooms_Hotel]
GO

ALTER TABLE [dbo].[Rooms]  WITH CHECK ADD  CONSTRAINT [CK_Sections_StandardPrice] CHECK  (([StandardPrice]<=(100000)))
GO

ALTER TABLE [dbo].[Rooms] CHECK CONSTRAINT [CK_Sections_StandardPrice]
GO


