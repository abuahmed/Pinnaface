using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core;
using PinnaFace.WPF.Models;
using Telerik.Windows.Controls;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for Duration.xaml
    /// </summary>
    public partial class Calendar : Window
    {
        public Calendar()
        {
            //StyleManager.SetTheme(DtSelectedDate, new TransparentTheme()); 
            InitializeComponent();
        }

        public Calendar(DateTime calDate)
        {
            //StyleManager.SetTheme(DtSelectedDate, new TransparentTheme()); 
            InitializeComponent();
            //Messenger.Default.Send<DateTime>(calDate);
            //Messenger.Reset();
            DtSelectedDate.SelectedDate = calDate;
            TxtSelectedDateMonth.Text = CalendarUtil.GetEthCalendar(calDate, true);

        }
        public Calendar(CalendarModel calModel)
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

        private void TxtSelectedDateMonth_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            var calDate = (RadCalendar)sender;
            if (calDate.SelectedDate != null)
                TxtSelectedDateMonth.Text = CalendarUtil.GetEthCalendar((DateTime) calDate.SelectedDate, true);
        }
    }
}
