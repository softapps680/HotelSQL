using System;

using Microsoft.Data.Sqlite;

using Windows.Storage;
using System.IO;

using Dapper;


namespace DataAccessLibrary
{
    public static class DataAccess
    {
       
        public async static void InitializeDatabase()
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

                string[] sqls = new string[] { paymentMethods, roomTypes, phonenumbers, guests, reservations, rooms };

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

     /*   public static void AddGuest(Guest guest)
        {

        }*/
        
    }
}
       

        
  