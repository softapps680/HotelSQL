using HotelConsoleSqlClient.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;

namespace HotelConsoleSqlClient
{
    
    public class Connecting
    {
         public  SqlConnection dbcon { get; set; } = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\paula\OneDrive\Dokument\thehotel.mdf;Integrated Security=True;Connect Timeout=30;MultipleActiveResultSets=true");
    }
    
    
    public class Program
    {
    
        static void Main(string[] args)
        {
            Connecting c = new Connecting();
             
            DbHelper db = new DbHelper(c.dbcon);
            db.CreateTables();
            
            Console.WriteLine("Welcome!");
            Console.WriteLine("1. Skapa bokning");
            Console.WriteLine("2. Lista bokningar");
            Console.WriteLine("3. Lista alla rum");
            Console.WriteLine("4. Lista lediga rum");

            var selection = Console.ReadLine();

            if (selection == "1")
            {
                InsertCustomer();
            }
            if (selection == "2")
            {
                ReservationsList();
            }
            if (selection == "3")
            {
                RoomsList();
            }
            if (selection == "4")
            {
                FreeRoomsList();
            }
        }
       
        public static void InsertCustomer()
        {
            Connecting c = new Connecting();

            using (c.dbcon)
            {
                c.dbcon.Open();
                
                var guest = new Guest();
                var phone = new PhoneNumber();
                var phone2 = new PhoneNumber();
                var reservation = new Reservation();
                

                Console.Write("Ange Förnamn: ");
                guest.FirstName = Console.ReadLine();

                Console.Write("Ange Efternamn: ");
                guest.LastName = Console.ReadLine();

                Console.Write("Ange E-postadress: ");
                guest.Email = Console.ReadLine();
               
                Console.Write("Ange Telefonnummer: ");
                phone.Telnumber = Console.ReadLine();
                
                Console.Write("Ange Alternativt telenr: ");
                phone2.Telnumber = Console.ReadLine();

                //1. Insert guest//Skapa guestId
                
                Guid g = Guid.NewGuid();
                guest.Id = g.ToString();
                string sqlquery2 = "INSERT INTO Guests (Id,FirstName,LastName,Email) OUTPUT Inserted.Id VALUES (@Id,@FirstName,@LastName,@Email)";
                using var cmd2 = new SqlCommand(sqlquery2, c.dbcon);
                cmd2.Parameters.AddWithValue("@Id", guest.Id);
                cmd2.Parameters.AddWithValue("@FirstName", guest.FirstName);
                cmd2.Parameters.AddWithValue("@LastName", guest.LastName);
                cmd2.Parameters.AddWithValue("@Email", guest.Email);
                
                guest.Id = (string)cmd2.ExecuteScalar();

                using var cmd_phone = new SqlCommand("INSERT INTO GuestPhoneNumbers (PhoneNumber,GuestId)  OUTPUT Inserted.Id VALUES (@PhoneNumber,@GuestId)", c.dbcon);
                using var cmd_altphone = new SqlCommand("INSERT INTO GuestPhoneNumbers (PhoneNumber,GuestId)  OUTPUT Inserted.Id VALUES (@PhoneNumber2,@GuestId)", c.dbcon);
                
                cmd_phone.Parameters.AddWithValue("@PhoneNumber", phone.Telnumber);
                cmd_phone.Parameters.AddWithValue("@GuestId", guest.Id);

                cmd_altphone.Parameters.AddWithValue("@PhoneNumber2", phone2.Telnumber);
                cmd_altphone.Parameters.AddWithValue("@GuestId", guest.Id);

                cmd_phone.ExecuteNonQuery();
                cmd_altphone.ExecuteNonQuery();


                Console.WriteLine("Välj betalmetod ");

                

                using var cmd_paymentmethod = new SqlCommand("SELECT * FROM PaymentMethods", c.dbcon);
                var result = cmd_paymentmethod.ExecuteReader();
                while (result.Read())
                {
                    Console.WriteLine($"{result.GetValue(0)} {result.GetValue(1)}");
                }


                int paymentMethodId = int.Parse(Console.ReadLine());
                reservation.Id = Guid.NewGuid().ToString();
               

                
                SelectRoom(reservation.Id,paymentMethodId,guest.Id);
               
            }
        }

        public static void SelectRoom(string resid,int paymentMethodId,string guestid)
        {
            var room = new Room();
            Connecting c = new Connecting();
            using (c.dbcon)
            {
               
                c.dbcon.Open();
                Console.WriteLine("Obokade rum idag");
                //Kolla datum på reservationsID
                using var cmd = new SqlCommand("SELECT Rooms.Id, RoomTypes.RoomTypeName FROM Rooms INNER JOIN RoomTypes ON Rooms.RoomTypeId = RoomTypes.Id WHERE ReservationId is NULL", c.dbcon);

                var result = cmd.ExecuteReader();
                
               
                while (result.Read())
                {
                    Console.WriteLine($"{result.GetValue(0)} {result.GetValue(1)}");
                }

                
                var roomId = int.Parse(Console.ReadLine());
                Console.WriteLine("Check in date  ");
                var checkin = Console.ReadLine();
                Console.WriteLine("Check out date  ");
                var checkout = Console.ReadLine();
                //"update tblposts set title=@ptitle, pdate=@pd, 
               // content = @pcontent where pid = @p"
                //create reservation
                using var insertReservationCmd = new SqlCommand("INSERT INTO Reservations (Id,GuestId,RoomId,PaymentMethodId,CheckInDate,CheckOutDate) VALUES (@Id,@GuestId,@RoomId,@PaymentMethodId,@CheckInDate,@CheckOutDate)", c.dbcon);
                insertReservationCmd.Parameters.AddWithValue("@Id", resid);
                insertReservationCmd.Parameters.AddWithValue("@GuestId", guestid);
                insertReservationCmd.Parameters.AddWithValue("@RoomId", roomId);
                insertReservationCmd.Parameters.AddWithValue("@PaymentMethodId", paymentMethodId);
                insertReservationCmd.Parameters.AddWithValue("@CheckInDate", checkin);
                insertReservationCmd.Parameters.AddWithValue("@CheckOutDate", checkout);
                insertReservationCmd.ExecuteNonQuery();
                
                using var updateRooms = new SqlCommand("UPDATE Rooms SET ReservationId = @ReservationId WHERE Id = @RoomId", c.dbcon);
                updateRooms.Parameters.AddWithValue("@RoomId", roomId);
                updateRooms.Parameters.AddWithValue("@ReservationId", resid);
                
                updateRooms.ExecuteNonQuery();
            }
        }
        public static void ReservationsList()
        {
            Connecting c = new Connecting();
            using (c.dbcon)
            {
                c.dbcon.Open();

                
                string sql = "SELECT  Rooms.Id, RoomTypes.RoomTypeName, Guests.FirstName, Guests.LastName, CheckInDate, CheckOutDate, Id FROM Reservations"
                +" INNER JOIN Rooms ON Rooms.Id = Reservations.RoomId"
                +" INNER JOIN Guests ON  Guests.Id = Reservations.GuestId"
                +" INNER JOIN RoomTypes ON Rooms.RoomTypeId = RoomTypes.Id";
                
              

                var result = new SqlCommand(sql, c.dbcon).ExecuteReader();

                Console.WriteLine("Rum \t Rumstyp \t Förnamn \t Efternamn \t Incheckning \t Utcheckning \t");
               
                while (result.Read())
                {
                    Console.WriteLine($"{result.GetValue(0)} \t {result.GetValue(1)} \t {result.GetValue(2)} \t {result.GetValue(3)} \t {result.GetValue(4)} \t { result.GetValue(5)} \t { result.GetValue(6)} ");
                }

                Console.WriteLine("Ange Rumsnummer för att checka ut gästen");
                int RoomNumber = int.Parse(Console.ReadLine());
                //hämta resid
               
                
                using var updateRooms = new SqlCommand("UPDATE Rooms SET ReservationId = NULL WHERE Id = @RoomId", c.dbcon);
                updateRooms.Parameters.AddWithValue("@RoomId", RoomNumber);
                updateRooms.Parameters.AddWithValue("@ReservationId", null);

                updateRooms.ExecuteNonQuery();

            }
        }
        public static void RoomsList()
        {
            Connecting c = new Connecting();
            using (c.dbcon)
            {
                c.dbcon.Open();


                string sql = "SELECT  Rooms.Id, RoomTypes.RoomTypeName  FROM Rooms"

                            + " INNER JOIN RoomTypes ON Rooms.RoomTypeId = RoomTypes.Id";



                var result = new SqlCommand(sql, c.dbcon).ExecuteReader();

                Console.WriteLine("Rum \t Rumstyp \t ");

                while (result.Read())
                {
                    Console.WriteLine($"{result.GetValue(0)} \t {result.GetValue(1)} ");
                }
            }
        }
        public static void FreeRoomsList()
        {
            Connecting c = new Connecting();
  Console.WriteLine("Önskad incheckning");
           
           

            var dateStr = Console.ReadLine();
            
            DateTime dt = DateTime.ParseExact(dateStr, "yyyyMMdd", CultureInfo.InvariantCulture);
            string arrival = dt.ToString("yyyy-MM-dd");
           
            Console.WriteLine("Önskad utcheckning");
            string leave = DateTime.ParseExact(Console.ReadLine(), "yyyyMMdd", CultureInfo.InvariantCulture).ToString();

            //rum utan reservationId mellan två datum

            string sql = "SELECT  Rooms.Id, RoomTypes.RoomTypeName FROM Rooms"
            + " INNER JOIN RoomTypes ON Rooms.RoomTypeId = RoomTypes.Id"
            + " EXCEPT"
            + " SELECT  Rooms.Id, RoomTypes.RoomTypeName FROM Rooms"
            + " INNER JOIN RoomTypes ON Rooms.RoomTypeId = RoomTypes.Id"
            + " INNER JOIN Reservations ON Rooms.Id = Reservations.RoomId"
            + " WHERE CheckInDate >= '" + arrival + "' AND CheckOutDate <= '" + leave + "'";
           
            using (c.dbcon)
            {

                c.dbcon.Open();

                var result = new SqlCommand(sql, c.dbcon).ExecuteReader();
                


                while (result.Read())
                {
                    Console.WriteLine($"{result.GetValue(0)} {result.GetValue(1)}");
                }
            }
        }
    }
}
