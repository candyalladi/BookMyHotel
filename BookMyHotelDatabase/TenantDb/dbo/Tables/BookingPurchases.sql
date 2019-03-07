CREATE TABLE [dbo].[BookingPurchases]
(
    [BookingPurchaseId]  INT         NOT NULL IDENTITY (1,1), 
    [BookedDate]      DATETIME    NOT NULL, 
    [TotalPrice]     MONEY       NOT NULL,
    [GuestId]        INT         NOT NULL,
	[OfferId]		INT NOT nULL,
    [RowVersion]        ROWVERSION, 
    PRIMARY KEY CLUSTERED ([BookingPurchaseId] ASC), 
    CONSTRAINT [FK_TicketPurchases_Customers] FOREIGN KEY ([GuestId]) REFERENCES [Guests]([GuestId])
)

GO


CREATE INDEX [IX_BookingPurchases_GuestId] ON [dbo].[BookingPurchases] (GuestId)
