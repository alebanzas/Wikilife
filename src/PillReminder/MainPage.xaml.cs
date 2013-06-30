using System;
using System.Windows;
using Microsoft.Phone.Controls;

namespace PillReminder
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Account/Register.xaml", UriKind.Relative));
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Account/Login.xaml", UriKind.Relative));
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            //TODO: logout
        }
    }
}