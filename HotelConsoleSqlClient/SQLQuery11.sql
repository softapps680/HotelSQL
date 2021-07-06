SELECT  Rooms.Id, RoomTypes.RoomTypeName, Guests.FirstName, Guests.LastName
FROM Rooms

INNER JOIN Reservations ON Rooms.ReservationId=Reservations.Id
INNER JOIN Guests ON Guests.Id=Reservations.GuestId
INNER JOIN RoomTypes ON Rooms.RoomTypeId=RoomTypes.Id;

