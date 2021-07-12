//using DataAccessLibrary;
using HotelUWP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Data.SqlClient;
using Windows.ApplicationModel.Core;
using Windows.UI.Popups;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HotelUWP
{
   
    
    public sealed partial class MainPage : Page
    {
        
        private string arrivalDateTime, leaveDateTime, guestId;
        int paymentMethodId, roomtypeId, selectedRoom;
        private ObservableCollection<Room> rooms;
       
        public MainPage()
        {
            this.InitializeComponent();

            DbHelper.InitializeDatabase();


            SelectPayment.ItemsSource = DbHelper.GetPaymentMethods();
            SelectPayment.DisplayMemberPath = "PaymentTypeName";

            pickArrival.SelectedDate = new DateTimeOffset(DateTime.Today);
            pickLeaving.SelectedDate = new DateTimeOffset(DateTime.Today).AddDays(1);

            string dateTime = DateTime.Today.ToString();
            string dateTime2 = DateTime.Today.AddDays(1).ToString();
            arrivalDateTime = Convert.ToDateTime(dateTime).ToString("yyyy-MM-dd HH:mm:ss");
            leaveDateTime = Convert.ToDateTime(dateTime2).ToString("yyyy-MM-dd HH:mm:ss");
          

        }
        private void ListButton_Click(object sender, RoutedEventArgs e)
        {
            rooms = DbHelper.GetFreeRooms(arrivalDateTime, leaveDateTime);
            RoomsList.ItemsSource = rooms;
        }
        private void Lista_ItemClick(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (RoomsList.SelectedItems[0] != null)
                {
                    Room r = (Room)RoomsList.SelectedItems[0];
                    selectedRoom = r.Id;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }

        }
        private void ArrivalDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            arrivalDateTime = new DateTime(e.NewDate.Year, e.NewDate.Month, e.NewDate.Day).ToString("yyyy-MM-dd");
        }

        private void LeaveDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {

            leaveDateTime = new DateTime(e.NewDate.Year, e.NewDate.Month, e.NewDate.Day).ToString("yyyy-MM-dd");
        }

        private void status_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var container = sender as ComboBox;
            PaymentMethod p = (PaymentMethod)container.SelectedItem;
            paymentMethodId = p.Id;
        }
       
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            Guid g = Guid.NewGuid();
            this.guestId = g.ToString();

            List<GuestPhonenumber> phones = new List<GuestPhonenumber>();
            GuestPhonenumber gp = new GuestPhonenumber();
            GuestPhonenumber gp2 = new GuestPhonenumber();
            gp.GuestId = this.guestId;
            gp.PhoneNumber = Phone1.Text;
            gp2.GuestId = this.guestId;
            gp2.PhoneNumber = Phone2.Text;
            phones.Add(gp);
            phones.Add(gp2);

            Guest guest = new Guest(phones);
            guest.Id = this.guestId;
           
            guest.FirstName = FirstName.Text;
            guest.LastName = LastName.Text;
            guest.Email = Email.Text;
          
            DbHelper.SaveGuest(guest);

        }
        private void ReserveButton_Click(object sender, RoutedEventArgs e)
        {
            Guid g = Guid.NewGuid();
            string s = g.ToString();

            Reservation reservation = new Reservation
            {
                Id = s,
                GuestId = this.guestId,
                RoomId = this.selectedRoom,
                PaymentMethodId = this.paymentMethodId

            };
            Room reservedRoom = new Room
            {
                Id = selectedRoom,
                ReservationId = s,
                CheckInDate = DateTime.Parse(arrivalDateTime),
                CheckOutDate = DateTime.Parse(leaveDateTime)

            };
            DbHelper.SaveReservation(reservation, reservedRoom);
            rooms = DbHelper.GetFreeRooms(arrivalDateTime, leaveDateTime);
            RoomsList.ItemsSource = rooms;
        }
        
    }
}
