using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

            var phonenumbers = $@"
            IF  NOT EXISTS (SELECT * FROM sys.objects 
            WHERE object_id = OBJECT_ID(N'[dbo].[GuestPhonenumbers]') AND type in (N'U'))
            BEGIN
            CREATE TABLE [dbo].[GuestPhonenumbers](
	            [Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	            [PhoneNumber] [varchar](11) NOT NULL,
	            )
            END";
            
            var guests = $@"
            IF  NOT EXISTS (SELECT * FROM sys.objects 
            WHERE object_id = OBJECT_ID(N'[dbo].[Guests]') AND type in (N'U'))
            BEGIN
            CREATE TABLE [dbo].[Guests](
	            [Id] [varchar](38)  NOT NULL,
	            [TelephoneId] [int]  NOT NULL,
                [FirstName] [varchar](21) NOT NULL,
                [LastName] [varchar](21) NOT NULL,
	            [Email] [varchar](21) NOT NULL,

                PRIMARY KEY (Id),
                CONSTRAINT FK_GuestsGuestsTelephoneId FOREIGN KEY (TelephoneId)
                REFERENCES GuestPhoneNumbers(Id)
                )
            END";

            var reservations = $@"
            IF  NOT EXISTS (SELECT * FROM sys.objects 
            WHERE object_id = OBJECT_ID(N'[dbo].[Reservations]') AND type in (N'U'))
            BEGIN
            CREATE TABLE [dbo].[Reservations](
	            [Id] [varchar](38)  NOT NULL,
	            [GuestId] [varchar](38)  NOT NULL,
                [PaymentMethodId] [int] NOT NULL,
                
                PRIMARY KEY (Id),
                CONSTRAINT FK_ReservationsGuests FOREIGN KEY (GuestId)
                REFERENCES Guests(Id),
                CONSTRAINT FK_ReservationsPaymentMethods FOREIGN KEY (PaymentmethodId)
                REFERENCES PaymentMethods(Id),
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
                CONSTRAINT FK_ReservationsRooms FOREIGN KEY (ReservationId)
                REFERENCES Reservations(Id)
                
                )
            END";
            
            string insertPaymethods = "INSERT INTO PaymentMethods (PaymentTypeName)  VALUES ('Kort'),('Swish'),('Kontant')";
            string insertRoomTypes = "INSERT INTO RoomTypes (RoomTypeName,Price) VALUES ('Enkelrum','600'),('Enkelrum','600'),('Dubbelrum','1100'),('Dubbelrum','1100'),('Svit','2100'),('Svit','2100')";

            using (dbcon)
            {
                dbcon.Open();
                string[] sqls = new string[] { paymentMethods, roomTypes, phonenumbers,guests, reservations,rooms,insertPaymethods,insertRoomTypes };
                
                for (int i = 0;i< sqls.Length; i++)
                {
                    using var cmd = new SqlCommand(sqls[i], dbcon);
                    cmd.ExecuteNonQuery();
                }
                
              

            }


        }


    }
}
