using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.Storage;
using System.Data.SqlClient;
using System.Data;

namespace HotelUWP.Models
{
    public static class DbHelper
    {
        public static SqliteConnection conn { get; set; }
        public static string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "hoteltest.db");




        public  static async void InitializeDatabase()
        {
            //C:\Users\paula\AppData\Local\Packages\16d5d14d-7b72-4154-88db-cc4d04f5eaaf_tkrspepzy0bwm\LocalState

             StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            
            await localFolder.CreateFileAsync("hoteltest.db", CreationCollisionOption.OpenIfExists);
               string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "hoteltest.db");


             using (SqliteConnection conn = new SqliteConnection($"Filename={dbpath}"))
            
            {
                conn.Open();


                string paymentMethods = "CREATE TABLE IF  NOT EXISTS PaymentMethods(Id INTEGER PRIMARY KEY,PaymentTypeName VARCHAR(21))";
                string roomTypes = "CREATE TABLE IF  NOT EXISTS RoomTypes (Id INTEGER PRIMARY KEY, RoomTypeName VARCHAR(21),Price REAL)";
                string guests = "CREATE TABLE IF  NOT EXISTS Guests (Id VARCHAR(38) PRIMARY KEY, FirstName VARCHAR(21), LastName VARCHAR(21), Email VARCHAR(21))";

                string phonenumbers = "CREATE TABLE IF  NOT EXISTS GuestPhonenumbers(Id INTEGER PRIMARY KEY,PhoneNumber VARCHAR(11), GuestId VARCHAR(38), FOREIGN KEY (GuestId) REFERENCES Guests(Id))";
                string reservations = "CREATE TABLE IF  NOT EXISTS Reservations (Id VARCHAR(38) PRIMARY KEY, GuestId VARCHAR(38),RoomId INTEGER, PaymentMethodId INTEGER NOT NULL, FOREIGN KEY(GuestId) REFERENCES Guests(Id), FOREIGN KEY(PaymentMethodId) REFERENCES PaymentMethods(Id))";
                string rooms = "CREATE TABLE IF  NOT EXISTS Rooms (Id INTEGER PRIMARY KEY, RoomTypeId INTEGER, ReservationId VARCHAR(38), CheckInDate DATE, CheckOutDate DATE, FOREIGN KEY (RoomTypeId) REFERENCES RoomTypes(Id), FOREIGN KEY (ReservationId) REFERENCES Reservations(Id))";

                string[] sqls = new string[] { paymentMethods, roomTypes, guests, phonenumbers, reservations, rooms };
               

                for (int i = 0; i < sqls.Length; i++)
                {
                    if (sqls[i] != "")
                    {
                        conn.Execute(sqls[i]);
                    }
                }


                string checkPaymethods = "SELECT COUNT (*) AS antal FROM PaymentMethods",
                        checkRoomTypes = "SELECT COUNT (*) AS antal FROM RoomTypes",
                        checkRooms = "SELECT COUNT (*) AS antal FROM Rooms";
                string[] checks = new string[] { checkPaymethods, checkRoomTypes, checkRooms };

                for (int i = 0; i < checks.Length; i++)
                {
                    var res = conn.ExecuteScalar<int>(checks[i]);

                    if (res == 0)
                    {
                        if (i == 0)
                        {
                            conn.Execute("INSERT INTO PaymentMethods (PaymentTypeName)  VALUES ('Kort'),('Swish'),('Kontant')");

                        }
                        if (i == 1)
                        {
                            conn.Execute("INSERT INTO RoomTypes (RoomTypeName,Price) VALUES ('Enkelrum','600'),('Dubbelrum','1100'),('Svit','2100')");

                        }
                        if (i == 2)
                        {
                            conn.Execute("INSERT INTO Rooms (RoomTypeId) VALUES ('1'),('1'),('2'),('2'),('3'),('3')");

                        }
                    }
                }
            }
        }

        public static List<PaymentMethod> GetPaymentMethods()
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "hoteltest.db");
            
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbpath}"))
            {
                var paymentMethods = conn.Query<PaymentMethod>("SELECT * FROM PaymentMethods").ToList();
                return paymentMethods;
            }  
        }
        public static List<RoomType> GetRoomTypes()
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "hoteltest.db");

            using (SqliteConnection conn = new SqliteConnection($"Filename={dbpath}"))
            {
                var roomTypes = conn.Query<RoomType>("SELECT * FROM RoomTypes").ToList();
                return roomTypes;
            }
        }
        public static ObservableCollection<Room> GetFreeRooms(string arrival, string leave)
        {
            string sql = "SELECT  Rooms.Id, RoomTypes.RoomTypeName FROM Rooms"
               + " INNER JOIN RoomTypes ON Rooms.RoomTypeId = RoomTypes.Id"
               + " EXCEPT"
               + " SELECT  Rooms.Id, RoomTypes.RoomTypeName FROM Rooms"
               + " INNER JOIN RoomTypes ON Rooms.RoomTypeId = RoomTypes.Id"
               + " INNER JOIN Reservations ON Rooms.Id = Reservations.RoomId"
               + " WHERE CheckInDate >= '" + arrival + "' AND CheckOutDate <= '" + leave + "'";
          
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbpath}"))
            {
                conn.Open();
               
                var rooms = conn.Query<Room, RoomType, Room>(
             sql,
             (room, roomtype) =>
             {
                 room.RoomType = roomtype;
                 return room;
             },
             splitOn:"RoomTypeName")
            .Distinct() .ToList();

            ObservableCollection<Room> freeRooms = new ObservableCollection<Room>();

                foreach (var item in rooms) { 
                    freeRooms.Add(item);
                }
               
                return freeRooms;
            }
        }




        public static void  SaveGuest(Guest guest)
        {
           
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbpath}")) { 

                conn.Execute("INSERT INTO Guests (Id, FirstName, LastName, Email)  VALUES(@Id, @FirstName, @LastName, @Email)", guest);
                
                foreach (var item in guest.GuestPhonenumbers)
                {
                  conn.ExecuteScalar("INSERT INTO GuestPhonenumbers (PhoneNumber, GuestId)  VALUES(@PhoneNumber, @GuestId);", new { PhoneNumber = item.PhoneNumber, GuestId = item.GuestId });
                }
            }
        }
        
        public static void SaveReservation(Reservation reservation, Room room)
        {
            
            string insert = "INSERT INTO Reservations (Id, GuestId, RoomId, PaymentMethodId) Values (@Id, @GuestId, @RoomId, @PaymentMethodId);";
            string update = "UPDATE Rooms SET ReservationId = @ReservationId, CheckInDate = @CheckInDate, CheckOutDate = @CheckOutDate WHERE Id = @Id";
           
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbpath}"))
            {
                conn.Execute(insert, reservation );
                conn.Execute(update, room);
            }
        }
    }
}
