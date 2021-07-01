using HotelConsoleSqlClient.Entities;
using System;
using System.Data.SqlClient;
using System.IO;

namespace HotelConsoleSqlClient
{
    
    public class Connecting
    {
         public  SqlConnection dbcon { get; set; } = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\paula\OneDrive\Dokument\thehotel.mdf;Integrated Security=True;Connect Timeout=30;MultipleActiveResultSets=true");
    }
    
    
    public class Program
    {
    //  public  SqlConnection dbcon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\paula\OneDrive\Dokument\thehotel.mdf;Integrated Security=True;Connect Timeout=30");
      //  public string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\paula\OneDrive\Dokument\thehotel.mdf;Integrated Security=True;Connect Timeout=30";
        static void Main(string[] args)
        {
            Connecting c = new Connecting();
             
            DbHelper db = new DbHelper(c.dbcon);
            db.CreateTables();
            
            Console.WriteLine("Welcome!");
            Console.WriteLine("1. Skapa bokning");
            Console.WriteLine("2. Lista bokningar");
            
            var selection = Console.ReadLine();

            if (selection == "1")
            {
                InsertCustomer();
            }
            if (selection == "2")
            {
                ReservationsList();
            }

        }
       
        public static void InsertCustomer()
        {
            Connecting c = new Connecting();

            using (c.dbcon)
            {
                c.dbcon.Open();

                var phone = new PhoneNumber();
                var guest = new Guest();
                
                Console.Write("Ange Förnamn: ");
                guest.FirstName = Console.ReadLine();

                Console.Write("Ange Efternamn: ");
                guest.LastName = Console.ReadLine();

                Console.Write("Ange E-postadress: ");
                guest.Email = Console.ReadLine();
               
                Console.Write("Ange Telefonnummer: ");
                phone.Telnumber = Console.ReadLine();

                
               
                string sqlquery1= "INSERT INTO GuestPhoneNumbers (PhoneNumber)  OUTPUT Inserted.Id VALUES (@PhoneNumber)";
                using var cmd1 = new SqlCommand(sqlquery1, c.dbcon);
                
                cmd1.Parameters.AddWithValue("@PhoneNumber", phone.Telnumber);
                //ExecuteScalar Executes the query, and returns the first column of the first row in the result set returned by the query
                guest.TelephoneId= (int)cmd1.ExecuteScalar();
               
                //Skapa guestId
                Guid g = Guid.NewGuid();
                guest.Id = g.ToString();
                
                string sqlquery2= "INSERT INTO Guests (Id,TelephoneId,FirstName,LastName,Email) OUTPUT Inserted.Id VALUES (@Id,@TelephoneId,@FirstName,@LastName,@Email)";
                using var cmd2 = new SqlCommand( sqlquery2, c.dbcon);
                cmd2.Parameters.AddWithValue("@Id", guest.Id);
                cmd2.Parameters.AddWithValue("@TelephoneId", guest.TelephoneId);
                cmd2.Parameters.AddWithValue("@FirstName", guest.FirstName);
                cmd2.Parameters.AddWithValue("@LastName", guest.LastName);
                cmd2.Parameters.AddWithValue("@Email", guest.Email);
                //var antal=cmd2.ExecuteNonQuery();

                guest.Id = (string)cmd2.ExecuteScalar();
                Console.WriteLine(" Guest Added ID "+ guest.Id);
                Console.WriteLine("Välj betalmetod ");
                //Lista lediga rum
               
               
                using var cmd3 = new SqlCommand("SELECT * FROM PaymentMethods", c.dbcon);
                var result = cmd3.ExecuteReader();
                
                while (result.Read())
                {
                    Console.WriteLine($"{result.GetValue(0)} {result.GetValue(1)}");
                }

                var selectedPaymethod =  Console.ReadLine();
                int payid = int.Parse(selectedPaymethod);

                //Skapa guestId
                Guid i = Guid.NewGuid();
                string id = i.ToString();

                string insertReservation = "INSERT INTO Reservations (Id,GuestId,PaymentMethodId) VALUES (@Id,@GuestId,@PaymentMethodId)";
                using var cmd4 = new SqlCommand(insertReservation, c.dbcon);
                cmd4.Parameters.AddWithValue("@Id", id);
                cmd4.Parameters.AddWithValue("@GuestId", guest.Id);
                cmd4.Parameters.AddWithValue("@PaymentMethodId", payid);
                string resId = (string)cmd4.ExecuteScalar();

                Console.WriteLine("HEJ" + id);
                SelectRoom(id);
            }
        }

        public static void SelectRoom(String resid)
        {
            Console.WriteLine("HEJ" + resid);
            Connecting c = new Connecting();
            using (c.dbcon)
            {
                Console.WriteLine("HEJ"+ resid);
                c.dbcon.Open();
                //Lista lediga rum
                using var cmd = new SqlCommand("SELECT Rooms.Id, RoomTypes.RoomTypeName FROM Rooms INNER JOIN RoomTypes ON Rooms.RoomTypeId = RoomTypes.Id WHERE ReservationId is NULL", c.dbcon);

                var result = cmd.ExecuteReader();
                Console.WriteLine("Select Room ");
               
                while (result.Read())
                {
                    Console.WriteLine($"{result.GetValue(0)} {result.GetValue(1)}");
                }

                
                var selectedRoom = int.Parse(Console.ReadLine());
                Console.WriteLine("Check in date  ");
                var checkin = Console.ReadLine();
                Console.WriteLine("Check out date  ");
                var checkout = Console.ReadLine();
                //uppdatera det rummet med de uppg inkl reservationsid

                string reserveSQL = "UPDATE Rooms SET ReservationId = '"+ resid + "',CheckInDate='" + checkin + "',CheckOutDate='" + checkout + "' WHERE Id='" + selectedRoom + "'" ;

                Console.WriteLine(reserveSQL);

                using var cmd5 = new SqlCommand(reserveSQL, c.dbcon);
               
                cmd5.ExecuteNonQuery();
            }
        }
        public static void ReservationsList()
        {
            Connecting c = new Connecting();
            using (c.dbcon)
            {
                c.dbcon.Open();
                //   C: \Users\paula\source\repos\HotelSQL\HotelConsoleSqlClient\SQLQuery11.sql
                //  @"/Users/praveen/Desktop/images\November.pdf";
                //var currPath = Path.Combine(Environment.CurrentDirectory, relation, "test.txt");
                var currPath = Path.Combine(Environment.CurrentDirectory, "SQLQuery11.sql");
                string listQuery = System.IO.File.ReadAllText(currPath);
                using var listCommand = new SqlCommand(listQuery, c.dbcon);
                
                var result = listCommand.ExecuteReader();

                while (result.Read())
                {
                    Console.WriteLine($"{result.GetValue(0)} {result.GetValue(1)} {result.GetValue(2)} {result.GetValue(3)} {result.GetValue(4)}");
                }
            }
        }
    }
}
