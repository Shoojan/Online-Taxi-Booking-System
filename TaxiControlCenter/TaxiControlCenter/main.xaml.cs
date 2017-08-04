using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TaxiControlCenter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class main : Page
    {
        public main()
        {
            this.InitializeComponent();
        }

        private async void loginbutton_Click(object sender, RoutedEventArgs e)
        {
            if (username.Text == "admin" && password.Password == "admin")
                this.Frame.Navigate(typeof(MainPage));
            else
            { 
                MessageDialog message = new MessageDialog("Invalid Username/Password","Access Denied!");
            //Right click and select resolve and choose the first option to use necessary Name Space (Headerfile)

            await message.ShowAsync();
            }
        }
    }
}
