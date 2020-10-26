using System;

namespace PinnaFace.WPF.Models
{
    public class CalendarModel
    {
        public CalendarModel()
        {
            SelectedDate = DateTime.Now;
        }
        public DateTime SelectedDate { get; set; }
    }
}