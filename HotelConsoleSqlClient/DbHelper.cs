using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelConsoleSqlClient
{
    class DbHelper
    {

        public static SqlConnection dbcon;
       
       
        public DbHelper(SqlConnection conn)
        {
        dbcon = conn;
        }

        public void CreateTables()
        {

            var paymentMethods = $@"
            IF  NOT EXISTS (SELECT * FROM sys.objects 
            WHERE object_id = OBJECT_ID(N'[dbo].[PaymentMethods]') AND type in (N'U'))
           BEGIN
           CREATE TABLE [dbo].[PaymentMethods](
	            [Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	            [PaymentTypeName] [varchar](21)
	            )
            END";

            var roomTypes = $@"
            IF  NOT EXISTS (SELECT * FROM sys.objects 
            WHERE object_id = OBJECT_ID(N'[dbo].[RoomTypes]') AND type in (N'U'))
            BEGIN
            CREATE TABLE [dbo].[RoomTypes](
	            [Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	            [RoomTypeName] [varchar](21) NULL,
	            [Price] [decimal] NOT NULL
                )
            END";
            var guests = $@"
            IF  NOT EXISTS (SELECT * FROM sys.objects 
            WHERE object_id = OBJECT_ID(N'[dbo].[Guests]') AND type in (N'U'))
            BEGIN
            CREATE TABLE [dbo].[Guests](
	            [Id] [varchar](38) PRIMARY KEY NOT NULL,
	            [FirstName] [varchar](21) NOT NULL,
                [LastName] [varchar](21) NOT NULL,
	            [Email] [varchar](21) NOT NULL
            )
            END";

            var phonenumbers = $@"
            IF  NOT EXISTS (SELECT * FROM sys.objects 
            WHERE object_id = OBJECT_ID(N'[dbo].[GuestPhonenumbers]') AND type in (N'U'))
            BEGIN
            CREATE TABLE [dbo].[GuestPhonenumbers](
	            [Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	            [PhoneNumber] [varchar](11) NOT NULL,
                [GuestId] [varchar](38) NOT NULL,
                
                
                CONSTRAINT FK_GuestsGuestsPhoneNumber FOREIGN KEY (GuestId)
                REFERENCES Guests(Id)
	            )
            END";


            var rooms = $@"
            IF  NOT EXISTS (SELECT * FROM sys.objects 
            WHERE object_id = OBJECT_ID(N'[dbo].[Rooms]') AND type in (N'U'))
            BEGIN
            CREATE TABLE [dbo].[Rooms](
	            [Id] [int] IDENTITY(1,1) NOT NULL,
	            [RoomTypeId] [int] NOT NULL,
                [ReservationId] [varchar](38) ,
                [CheckInDate] [date] ,
                [CheckOutDate] [date] ,
                
                PRIMARY KEY (Id),
                CONSTRAINT FK_ReservationsRoomTypes FOREIGN KEY (RoomTypeId)
                REFERENCES RoomTypes(Id),
                
                
                )
            END";
            //CONSTRAINT FK_ReservationsRooms FOREIGN KEY (ReservationId)
            //REFERENCES Reservations(Id)
            var reservations = $@"
            IF  NOT EXISTS (SELECT * FROM sys.objects 
            WHERE object_id = OBJECT_ID(N'[dbo].[Reservations]') AND type in (N'U'))
            BEGIN
            CREATE TABLE [dbo].[Reservations](
	            [Id] [varchar](38)  NOT NULL,
	            [GuestId] [varchar](38)  NOT NULL,
                [RoomId] [int], 
                [PaymentMethodId] [int] NOT NULL,
                
                PRIMARY KEY (Id),
                CONSTRAINT FK_ReservationsGuests FOREIGN KEY (GuestId)
                REFERENCES Guests(Id),
                CONSTRAINT FK_ReservationsPaymentMethods FOREIGN KEY (PaymentmethodId)
                REFERENCES PaymentMethods(Id),
                )
            END";

            

            dbcon.Open();
            
           
            
            using (dbcon)
            {
               
                string[] sqls = new string[] { paymentMethods, roomTypes,guests, phonenumbers, rooms,reservations };
                
                for (int i = 0;i< sqls.Length; i++)
                {
                    if (sqls[i] != "") { 
                    using var cmd = new SqlCommand(sqls[i], dbcon);
                    cmd.ExecuteNonQuery();
                    }
                }

                
                string checkPaymethods = "SELECT COUNT (*) AS antal FROM PaymentMethods",
                        checkRoomTypes = "SELECT COUNT (*) AS antal FROM RoomTypes",
                        checkRooms = "SELECT COUNT (*) AS antal FROM Rooms";
                string[] checks = new string[] { checkPaymethods, checkRoomTypes, checkRooms };
                
               
               for (int i = 0; i < checks.Length; i++)
                {
                    var cmdcheck = new SqlCommand(checks[i], dbcon);
                    int count = (int)cmdcheck.ExecuteScalar();
                    if (count == 0)
                    {
                        if (i == 0)
                        {
                            cmdcheck = new SqlCommand("INSERT INTO PaymentMethods (PaymentTypeName)  VALUES ('Kort'),('Swish'),('Kontant')", dbcon);
                            cmdcheck.ExecuteNonQuery();
                        }
                        if (i == 1)
                        {
                            cmdcheck = new SqlCommand("INSERT INTO RoomTypes (RoomTypeName,Price) VALUES ('Enkelrum','600'),('Dubbelrum','1100'),('Svit','2100')", dbcon);
                            cmdcheck.ExecuteNonQuery();
                        }
                        if (i == 2)
                        {
                            cmdcheck = new SqlCommand("INSERT INTO Rooms (RoomTypeId) VALUES ('1'),('1'),('2'),('2'),('3'),('3')", dbcon);
                            cmdcheck.ExecuteNonQuery();
                        }
                    }
                }
             }  

                 
}
            


        


    }
}
