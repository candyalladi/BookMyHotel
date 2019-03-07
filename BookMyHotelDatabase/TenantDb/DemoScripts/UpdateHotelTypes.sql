-- Extend the set of VenueTypes using an idempotent MERGE script
--
MERGE INTO [dbo].[HotelTypes] AS [target]
USING (VALUES
    ('verysmall','Very Small Hotel','SingleRoom', 'Double Room','Twin Sharing'),
    ('small','Small Hotel','Twin Sharing','Single Room','Double Room'),
    ('medium','Medium Hotel','Double Room','Single Room','Twin Sharing'),
    ('Large','Large Hotel','Twin Double Room','Double Room','Delux Room'),
    ('B&B','Bread & Breakfast','Transit', 'Trans','Transits'),
    ('suitehotels','Living Room','Suite Bedroom', 'Hall','Halls'),
    ('3star', '3 Star', 'Double Bedroom', 'Double', 'Doubles'),
    ('4star', '4 Star', 'Furnished', 'Furn','Fully'),
    ('5star','5 Star','King Suite','KSuite', 'KSuits'),
    ('airporthotel','Air Port Hotel','Presidential Suite','PSuite','PSuits'), 
    ('motel','Motel','Transit Guest', 'Transit', 'Transits')
) AS source(
    HotelType,HotelTypeName,RoomTypeName,RommTypeShortName,RoomTypeShortNamePlural
)              
ON [target].HotelType = source.HotelType
-- update existing rows
WHEN MATCHED THEN
    UPDATE SET 
        HotelTypeName = source.HotelTypeName,
        RoomTypeName = source.RoomTypeName,
        HotelTypeShortName = source.HotelTypeShortName,
        HotelTypeShortNamePlural = source.HotelTypeShortNamePlural
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
    INSERT (HotelType,HotelTypeName,RoomTypeName,RoomTypeShortName,RoomTypeShortNamePlural)
    VALUES (HotelType,HotelTypeName,RoomTypeName,RoomTypeShortName,RoomTypeShortNamePlural)
;
GO
