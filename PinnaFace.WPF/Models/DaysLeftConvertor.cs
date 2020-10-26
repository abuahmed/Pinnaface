using System;
using System.Windows.Data;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Models;

namespace PinnaFace.WPF.Models
{
    public class DaysLeftConvertor : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var emp = value as EmployeeDTO;

            if (emp != null)
            {
                var status = emp.CurrentStatus;

                switch (status)
                {
                    case ProcessStatusTypes.New:
                        return "Gray";
                    case ProcessStatusTypes.VisaAssigned:
                        return "LightGray";
                    case ProcessStatusTypes.OnProcess:
                    case ProcessStatusTypes.LabourProcess:
                    case ProcessStatusTypes.EmbassyProcess:
                        return "Yellow";
                    case ProcessStatusTypes.FlightProcess:
                    case ProcessStatusTypes.BookedDepartured:
                        return "Orange";
                    case ProcessStatusTypes.OnGoodCondition:
                        return "Green";
                    case ProcessStatusTypes.WithComplain:
                    case ProcessStatusTypes.Returned:
                    case ProcessStatusTypes.Lost:
                    case ProcessStatusTypes.Discontinued:
                    case ProcessStatusTypes.Canceled:
                        return "Red";
                }
            }

            return "Yellow";
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class ComplainStatusConvertor : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var emp = value as ComplainDTO;

            if (emp != null)
            {
                var status = emp.Status;

                switch (status)
                {
                    case ComplainStatusTypes.Opened:
                        return "Red";
                    case ComplainStatusTypes.OnProcess:
                        return "Yellow";
                    case ComplainStatusTypes.Closed:
                        return "Green";
                }

                //    var ldays = room.LastRentalPaymentTemp.DaysLeft;
                //    if (room.LastServicePaymentTemp.Contrat.Room != null)
                //    {
                //        var servDays = room.LastServicePaymentTemp.DaysLeft;
                //        if (servDays < ldays)
                //            ldays = servDays;
                //    }

                //    if (ldays > 10 && ldays <=366)
                //        return "Green";
                //    else if (ldays <= 10 && ldays >= 0)
                //        return "Yellow";
                //    else if (ldays < 0)
                //        return "Red";
                //    else return "White";
            }

            return "Green";
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}