﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PinnaFace.Admin.ViewModel;

namespace PinnaFace.Admin.Views
{
    /// <summary>
    /// Interaction logic for Agents.xaml
    /// </summary>
    public partial class ServerAgents : Window
    {
        public ServerAgents()
        {
            ServerAgentViewModel.Errors = 0;
            InitializeComponent();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) ServerAgentViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) ServerAgentViewModel.Errors -= 1;
        }

        //private void CheckBox_Checked(object sender, RoutedEventArgs e)
        //{

        //}

        //private void BtnAddNew_OnClick(object sender, RoutedEventArgs e)
        //{
        //    TxtAwajNumber.Focus();
        //}

        private void Agents_OnUnloaded(object sender, RoutedEventArgs e)
        {
            ServerAgentViewModel.CleanUp();
        }

        private void LstItemsAutoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LstItemsAutoCompleteBox.SearchText = string.Empty;
        }

        private void LstItemsAutoCompleteBox_GotFocus(object sender, RoutedEventArgs e)
        {
            LstItemsAutoCompleteBox.SearchText = string.Empty;
        }

        private void LstItemsAutoCompleteBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            LstItemsAutoCompleteBox.SearchText = string.Empty;
        }
    }
}
