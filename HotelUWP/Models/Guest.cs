using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelUWP.Models
{
    public class Guest
    {
        public Guest(List<GuestPhonenumber> list)
        {
            this.GuestPhonenumbers = list;
        }
        public String Id { get; set; }
        public int TelephoneId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }

        public List<GuestPhonenumber> GuestPhonenumbers { get; set; }
    }
}
