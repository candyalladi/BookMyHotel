CREATE PROCEDURE [dbo].[sp_DeleteBookings]
	@RoomId int
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @Bookings int = (SELECT Count(*) FROM dbo.Bookings WHERE RoomId = @RoomId)

    IF @Bookings > 0
    BEGIN
        RAISERROR ('Error. Cannot delete booking for which tickets have been purchased.', 11, 1)
        RETURN 1
    END

    DELETE FROM dbo.[Rooms]
    WHERE RoomId = @RoomId

    RETURN 0
END