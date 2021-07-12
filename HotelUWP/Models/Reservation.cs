using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelUWP.Models
{
    public class Reservation
    {
        public String Id { get; set; }
       
        public String GuestId{ get; set; }
        public int RoomId { get; set; }
        public int PaymentMethodId { get; set; }

        public Room Guest { get; set; }
        public Room Room{ get; set; }
        public PaymentMethod PaymentMethod { get; set; }

    }
}
