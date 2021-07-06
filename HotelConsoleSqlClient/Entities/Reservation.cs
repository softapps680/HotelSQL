using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelConsoleSqlClient.Entities
{
    class Reservation
    {
        public string Id { get; set; }
        public string GuestId { get; set; }
        public int RoomId { get; set; }
        public int PaymentMethodId { get; set; }
    }
}
