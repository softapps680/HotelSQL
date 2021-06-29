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

        public void CreateTables()
        {
           
          var createPaymentMethods = $@"
            IF  NOT EXISTS (SELECT * FROM sys.objects 
            WHERE object_id = OBJECT_ID(N'[dbo].[PaymentMethods]') AND type in (N'U'))

            BEGIN
            CREATE TABLE [dbo].[PaymentMethods](
	            [Id] [int] IDENTITY(1,1) NOT NULL,
	            [PaymentTypeName] [varchar](21) NULL,
	            
             CONSTRAINT [PK_PaymentMethods] PRIMARY KEY CLUSTERED 
            (
	            [Id] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, 
                IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            )
            END";
           
            var createRoomTypes = $@"
            IF  NOT EXISTS (SELECT * FROM sys.objects 
            WHERE object_id = OBJECT_ID(N'[dbo].[RoomTypes]') AND type in (N'U'))

            BEGIN
            CREATE TABLE [dbo].[RoomTypes](
	            [Id] [int] IDENTITY(1,1) NOT NULL,
	            [RoomTypeName] [varchar](21) NULL,
	            
             CONSTRAINT [PK_RoomTypes] PRIMARY KEY CLUSTERED 
            (
	            [Id] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, 
                IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) 
            END";

            var guestPhonenumbers = $@"
            IF  NOT EXISTS (SELECT * FROM sys.objects 
            WHERE object_id = OBJECT_ID(N'[dbo].[GuestPhonenumbers]') AND type in (N'U'))

            BEGIN
            CREATE TABLE [dbo].[GuestPhonenumbers](
	            [Id] [int] IDENTITY(1,1) NOT NULL,
	            [RoomTypeName] [varchar](11) NOT NULL,
	            
             CONSTRAINT [PK_GuestPhonenumbers] PRIMARY KEY CLUSTERED 
            (
	            [Id] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, 
                IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
            // FOREIGN KEY ([TelephoneId]) REFERENCES 
            //seed
            using (var conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\paula\OneDrive\Dokument\thehotel.mdf;Integrated Security=True;Connect Timeout=30"))
            {

                using var cmd_paymethods = new SqlCommand(createPaymentMethods, conn);
                using var cmd_roomtypes = new SqlCommand(createRoomTypes, conn);
                using var cmd_phonenumbers = new SqlCommand(guestPhonenumbers, conn);
                using var cmd_guests = new SqlCommand(guests, conn);
                conn.Open();
                cmd_paymethods.ExecuteNonQuery();
                cmd_roomtypes.ExecuteNonQuery();
                cmd_phonenumbers.ExecuteNonQuery();
                cmd_guests.ExecuteNonQuery();

                string insertPaymethods = "INSERT INTO PaymentMethods (PaymentTypeName)  VALUES ('Kort'),('Swish'),('Kontant')";
                string insertRoomTypes = "INSERT INTO RoomTypes (RoomTypeName) VALUES ('Enkelrum'),('Enkelrum'),('Dubbelrum'),('Dubbelrum'),('Svit'),('Svit')";

                using var cmd1 = new SqlCommand(insertPaymethods, conn);
                using var cmd2 = new SqlCommand(insertRoomTypes, conn);
                cmd1.ExecuteNonQuery();
                cmd2.ExecuteNonQuery();

            }


        }


    }
}
