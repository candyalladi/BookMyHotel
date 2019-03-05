CREATE PROCEDURE [dbo].[sp_ResetBookingDates]
    @StartHour int = 19,
    @StartMinute int = 00
AS
    SET NOCOUNT ON

    DECLARE @RoomId int
    DECLARE @Index int = 1
    DECLARE @Offset int = ROUND(((-3 - (-5) - 1) * RAND() + (-5)), 0)    -- offset of the first event in days from current date   
    DECLARE @Interval int = ROUND(((5 - 2 - 1) * RAND() + 2), 0)   -- interval between each event
    DECLARE @Old_Check_in_Date datetime
	DECLARE @Old_Check_out_Date datetime
    DECLARE @New_Check_in_Date datetime
	DECLARE @New_Check_out_Date datetime
    DECLARE @Diff int
    DECLARE @BaseDate datetime = DATETIMEFROMPARTS(YEAR(CURRENT_TIMESTAMP),MONTH(CURRENT_TIMESTAMP),DAY(CURRENT_TIMESTAMP),@StartHour,@StartMinute,00,000)
    DECLARE EventCursor CURSOR FOR SELECT BookingId FROM [dbo].[Bookings] 
    
    OPEN EventCursor
    FETCH NEXT FROM EventCursor INTO @RoomId
    --
    WHILE @@Fetch_Status = 0
    BEGIN
        SET @Old_Check_in_Date = (SELECT top 1 [Checkin_Date] from [Bookings] WHERE RoomId=@RoomId)
		SET @Old_Check_out_Date = (SELECT top 1 [Checkout_Date] from [Bookings] WHERE RoomId=@RoomId)		
        SET @New_Check_in_Date = DATEADD(Day,@Offset,@BaseDate)
		SET @New_Check_out_Date = DATEADD(Day,@Offset,@BaseDate)
        
        UPDATE [Bookings] SET [Checkin_Date] = @New_Check_in_Date WHERE RoomId=@RoomId
		UPDATE [Bookings] SET [Checkout_Date] = @New_Check_out_Date WHERE RoomId=@RoomId 
        
        UPDATE BookingPurchases SET BookedDate = DATEADD(day,Diff,@New_Check_in_Date)
        FROM BookingPurchases AS tp
        INNER JOIN (SELECT tp2.BookingPurchaseId, DATEDIFF(day,@Old_Check_in_Date,tp2.BookedDate) AS Diff 
                        FROM BookingPurchases AS tp2
                        INNER JOIN [Bookings] AS t ON t.BookingPurchaseId = tp2.BookingPurchaseId
                        INNER JOIN [Rooms] AS e ON t.RoomId = e.RoomId
                    WHERE e.RoomId = @RoomId) AS etp ON etp.BookingPurchaseId = tp.BookingPurchaseId

        SET @Offset = @Offset + @Interval
        SET @Index = @Index + 1
	    FETCH NEXT FROM EventCursor INTO @RoomId
    END
    
    CLOSE EventCursor
    DEALLOCATE EventCursor
    
    RETURN 0