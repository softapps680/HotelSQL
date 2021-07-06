﻿using HotelConsoleSqlClient.Entities;
using System;
using System.Data;
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
                Console.WriteLine("Unbooked rooms today ");
                
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

                //create reservation
                using var insertReservationCmd = new SqlCommand("INSERT INTO Reservations (Id,GuestId,RoomId,PaymentMethodId) VALUES (@Id,@GuestId,@RoomId,@PaymentMethodId)", c.dbcon);
                insertReservationCmd.Parameters.AddWithValue("@Id", resid);
                insertReservationCmd.Parameters.AddWithValue("@GuestId", guestid);
                insertReservationCmd.Parameters.AddWithValue("@RoomId", roomId);
                insertReservationCmd.Parameters.AddWithValue("@PaymentMethodId", paymentMethodId);
                insertReservationCmd.ExecuteNonQuery();
                //update the room with dates and  reservationsid


                string reserveSQL = "UPDATE Rooms SET ReservationId = @ReservationId ,CheckInDate=@CheckInDate ,CheckOutDate=@CheckOutDate WHERE Id=@ID" ;
                
            
       
                SqlCommand command = new SqlCommand(reserveSQL, c.dbcon);
                    
                command.Parameters.Add("@ID", SqlDbType.Int);
                command.Parameters["@ID"].Value = roomId;

                command.Parameters.AddWithValue("@ReservationId", resid);
                command.Parameters.AddWithValue("@CheckInDate", checkin);
                command.Parameters.AddWithValue("@CheckOutDate", checkout);
                command.ExecuteNonQuery();
            }
        }
        public static void ReservationsList()
        {
            Connecting c = new Connecting();
            using (c.dbcon)
            {
                c.dbcon.Open();

                
                string sql = "SELECT  Rooms.Id, RoomTypes.RoomTypeName, Guests.FirstName, Guests.LastName FROM Rooms"
                +" INNER JOIN Reservations ON Rooms.ReservationId = Reservations.Id"
                +" INNER JOIN Guests ON Guests.Id = Reservations.GuestId"
                +" INNER JOIN RoomTypes ON Rooms.RoomTypeId = RoomTypes.Id";
                
              

                var result = new SqlCommand(sql, c.dbcon).ExecuteReader();

                Console.WriteLine("Rum \t Rumstyp \t Förnamn \t Efternamn");
                
                while (result.Read())
                {
                    Console.WriteLine($"{result.GetValue(0)} \t {result.GetValue(1)} \t {result.GetValue(2)} \t {result.GetValue(3)} ");
                }

                Console.WriteLine("Ange Rumsnummer för att checka ute gästen");
            }
        }
    }
}
