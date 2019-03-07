CREATE TABLE [dbo].[RoomPrices] (
    [RoomId]           INT   NOT NULL,
    [HotelId]         INT   NOT NULL,
    [Price]             MONEY NOT NULL,
    [RowVersion]        ROWVERSION NOT NULL, 
    PRIMARY KEY CLUSTERED ([RoomId], [HotelId] ASC), 
    CONSTRAINT [FK_RoomPrices_Prices] FOREIGN KEY ([RoomId]) REFERENCES [Rooms]([RoomId]) ON DELETE CASCADE, 
    CONSTRAINT [FK_RoomPirces_Rooms] FOREIGN KEY ([RoomId]) REFERENCES [Rooms]([RoomId])
);

