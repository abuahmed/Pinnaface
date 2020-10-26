using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Models;
using PinnaFace.WPF.ViewModel;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for Employees.xaml
    /// </summary>
    public partial class Employees : UserControl
    {
        public Employees()
        {
            EmployeeViewModel.Errors = 0;
            InitializeComponent();
        }
        public Employees(SearchEmployee employeeId)
        {
            InitializeComponent();
            Messenger.Default.Send<SearchEmployee>(employeeId);
            Messenger.Reset();
        }
        private void OnRadGridViewFilterOperatorsLoading(object sender, Telerik.Windows.Controls.GridView.FilterOperatorsLoadingEventArgs e)
        {
            //e.AvailableOperators.Remove(Telerik.Windows.Data.FilterOperator.IsEqualTo);
            e.AvailableOperators.Remove(Telerik.Windows.Data.FilterOperator.IsNotEqualTo);

            e.AvailableOperators.Remove(Telerik.Windows.Data.FilterOperator.IsGreaterThanOrEqualTo);
            e.AvailableOperators.Remove(Telerik.Windows.Data.FilterOperator.IsLessThanOrEqualTo);

            e.AvailableOperators.Remove(Telerik.Windows.Data.FilterOperator.IsNull);
            e.AvailableOperators.Remove(Telerik.Windows.Data.FilterOperator.IsNotNull);

            e.AvailableOperators.Remove(Telerik.Windows.Data.FilterOperator.DoesNotContain);
            e.AvailableOperators.Remove(Telerik.Windows.Data.FilterOperator.EndsWith);

            e.AvailableOperators.Remove(Telerik.Windows.Data.FilterOperator.IsContainedIn);
            e.AvailableOperators.Remove(Telerik.Windows.Data.FilterOperator.IsEmpty);
            e.AvailableOperators.Remove(Telerik.Windows.Data.FilterOperator.IsNotEmpty);

            e.AvailableOperators.Remove(Telerik.Windows.Data.FilterOperator.IsGreaterThan);
            e.AvailableOperators.Remove(Telerik.Windows.Data.FilterOperator.IsLessThan);

            e.AvailableOperators.Remove(Telerik.Windows.Data.FilterOperator.IsNotContainedIn);
            e.AvailableOperators.Remove(Telerik.Windows.Data.FilterOperator.StartsWith);

            e.DefaultOperator1 = Telerik.Windows.Data.FilterOperator.Contains;
            if (e.Column.UniqueName != "SubmitDate")
            {
                e.AvailableOperators.Remove(Telerik.Windows.Data.FilterOperator.IsEqualTo);
            }
        } 

        private void Employees_OnLoaded(object sender, RoutedEventArgs e)
        {
            //TxtFirstName.Focus();
        }

        private void EmployeesGridView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var originalSender = e.OriginalSource as FrameworkElement;
            if (originalSender != null)
            {
                //var cell = originalSender.ParentOfType<GridViewCell>();
                //if (cell != null)
                //{
                //    MessageBox.Show("The double-clicked cell is " + cell.Value);
                //}

                var row = originalSender.ParentOfType<GridViewRow>();
                //if (row != null)
                //{
                //    MessageBox.Show("The double-clicked row is " + ((EmployeeDTO)row.DataContext).FullName);
                //}
                if (row != null)
                {
                    var empId = ((EmployeeDTO) row.DataContext).Id;
                    Messenger.Default.Send<SearchEmployee>(new SearchEmployee { EmpId = empId });  
                    //new EmployeeDetail(empId).ShowDialog();
                }
                   
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;

            if (checkBox != null)
            {
                if (checkBox.IsChecked != null)// && (bool) checkBox.IsChecked)
                {
                    var grid = checkBox.ParentOfType<RadGridView>();
                    var cells =  grid.ChildrenOfType<GridViewCell>().Where(c=>c.Column.DisplayIndex==0).ToList();
                    foreach (var gridViewCell in cells)
                    {
                        //var checkBx=(GridViewCheckBoxColumn)gridViewCell;
                        if ((bool)checkBox.IsChecked)
                        {
                            gridViewCell.Value = true;
                        }else
                        gridViewCell.Value = false;
                    }
                    //MessageBox.Show("Checked");
                }
            }
    
           //playersGrid.SelectAll()
        
            //playersGrid.UnselectAll()


        }
    }
}
