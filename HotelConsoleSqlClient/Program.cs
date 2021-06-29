using HotelConsoleSqlClient.Entities;
using System;
using System.Data.SqlClient;

namespace HotelConsoleSqlClient
{
    class Program
    {
        public static SqlConnection dbcon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\paula\OneDrive\Dokument\thehotel.mdf;Integrated Security=True;Connect Timeout=30");
        static void Main(string[] args)
        {

             
            DbHelper db = new DbHelper(dbcon);
            db.CreateTables();
            
            Console.WriteLine("Welcome!");
            Console.WriteLine("1. Skapa bokning");
            Console.WriteLine("2. Lista bokningar");
            
            var selection = Console.ReadLine();

            if (selection == "1")
            {
                InsertCustomer();
            }

           
        }
       
        public static void InsertCustomer()
        {
            using (var conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\paula\OneDrive\Dokument\thehotel.mdf;Integrated Security=True;Connect Timeout=30"))
            {
                conn.Open();

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
                using var cmd1 = new SqlCommand(sqlquery1, conn);
                
                cmd1.Parameters.AddWithValue("@PhoneNumber", phone.Telnumber);
                //ExecuteScalar Executes the query, and returns the first column of the first row in the result set returned by the query
                guest.TelephoneId= (int)cmd1.ExecuteScalar();
               
                //Skapa guestId
                Guid g = Guid.NewGuid();
                guest.Id = g.ToString();
                
                string sqlquery2="INSERT INTO Guests (Id,TelephoneId,FirstName,LastName,Email) VALUES (@Id,@TelephoneId,@FirstName,@LastName,@Email)";
                using var cmd2 = new SqlCommand( sqlquery2, conn);
                cmd2.Parameters.AddWithValue("@Id", guest.Id);
                cmd2.Parameters.AddWithValue("@TelephoneId", guest.TelephoneId);
                cmd2.Parameters.AddWithValue("@FirstName", guest.FirstName);
                cmd2.Parameters.AddWithValue("@LastName", guest.LastName);
                cmd2.Parameters.AddWithValue("@Email", guest.Email);
                var antal=cmd2.ExecuteNonQuery();
                
                Console.WriteLine(antal+ " Guest Added");
                ListRoomTypes();
            }
        }

        public static void ListRoomTypes()
        {
            using (var conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\paula\OneDrive\Dokument\thehotel.mdf;Integrated Security=True;Connect Timeout=30"))
            {
                conn.Open();
                //Lista lediga rum
                using var cmd = new SqlCommand("SELECT Rooms.Id, RoomTypes.RoomTypeName FROM Rooms INNER JOIN RoomTypes ON Rooms.RoomTypeId = RoomTypes.Id WHERE ReservationId is NULL", conn);

                var result = cmd.ExecuteReader();
                Console.WriteLine("Select Room ");
               
                while (result.Read())
                {
                    Console.WriteLine($"{result.GetValue(0)} {result.GetValue(1)}");
                }
                
                var selectedRoom = Console.ReadLine();
                Console.WriteLine("Check in date  ");
                var checkin = Console.ReadLine();
                Console.WriteLine("Check out date  ");
                var checkout = Console.ReadLine();
            }
        }

    }
}
