using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for BackupRestore.xaml
    /// </summary>
    public partial class BackupRestore : Window
    {
        public BackupRestore()
        {
            InitializeComponent();
        }
        public BackupRestore(object obj)
        {            
            InitializeComponent();
            Messenger.Default.Send<object>(obj);
            Messenger.Reset();
        }
    }
}
