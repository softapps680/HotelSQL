SELECT  Rooms.Id, RoomTypes.RoomTypeName, Guests.FirstName, Guests.LastName, GuestPhonenumbers.PhoneNumber
FROM Rooms

INNER JOIN Reservations ON Rooms.ReservationId=Reservations.Id
INNER JOIN Guests ON Guests.Id=Reservations.GuestId
INNER JOIN RoomTypes ON Rooms.RoomTypeId=RoomTypes.Id
INNER JOIN GuestPhonenumbers ON Guests.TelephoneId=GuestPhonenumbers.Id;

/*WHERE Guests.Id IS NOT NULL
 OR Reservations.Id IS NOT NULL*/