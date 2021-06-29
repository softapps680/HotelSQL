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
            END;