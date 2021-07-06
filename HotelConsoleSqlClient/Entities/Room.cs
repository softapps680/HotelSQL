﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelConsoleSqlClient.Entities
{
    class Room
    {
        public int Id { get; set; }
        public int RoomTypeId { get; set; }
        public string ReservationId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }


    }
}
