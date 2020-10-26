using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.WPF.Models;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for Duration.xaml
    /// </summary>
    public partial class CalendarConvertor : Window
    {
        public CalendarConvertor()
        {
            InitializeComponent();
        }

        public CalendarConvertor(DateTime calDate)
        {
            InitializeComponent();
            Messenger.Default.Send<DateTime>(calDate);
            Messenger.Reset();
        }
        public CalendarConvertor(CalendarModel calModel)
        {
            InitializeComponent();
            Messenger.Default.Send<CalendarModel>(calModel);
            Messenger.Reset();
        }

        private void DtSelectedDate_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
