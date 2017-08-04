using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace BookTaxiV2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login : Page
    {
        public static string userback;
        public class Customer
        {
            public string Id { get; set; }
            public string Fullname { get; set; }
            public string Dob { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }


        }


        private MobileServiceCollection<Customer, Customer> items;
        private IMobileServiceTable<Customer> itemsTable = App.MobileService.GetTable<Customer>();
        public Login()
        {
            this.InitializeComponent();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                e.Handled = true;
                Frame.GoBack();
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async Task InsertTodoItem(Customer customer)
        {
            // This code inserts a new TodoItem into the database. When the operation completes
            // and Mobile App backend has assigned an Id, the item is added to the CollectionView.
            try
            {
                await itemsTable.InsertAsync(customer);
                //MessageBox("You are now Registered.", "Congratulations!!!");
                //await new MessageDialog("You are now Registered").ShowAsync();

                var messagedialog = new MessageDialog("You are now Registered. Please use your username and password to login.","Congratulations!");
                //First command starts
                messagedialog.Commands.Add(new UICommand("Ok", (UICommandInvokeHandler) =>
                {
                    //Frame.Navigate(typeof(Login)); //closes app
                }
                    )); //First Command Ends here
                        // Second command starts here
                await messagedialog.ShowAsync();

               
            }
            catch (Exception ex)
            {
                var messagedialog = new MessageDialog("Please check your internet connection.", "Something is wrong!");
                //First command starts

                await messagedialog.ShowAsync();
            }

        }

        private async void register_Click(object sender, RoutedEventArgs e)
        {
            if(fname.Text == "" || phone.Text == "" || email.Text == "" || uname.Text == "" || pass.Password == "")
            {
                await new MessageDialog("All fields are compulsory. Please check your input.").ShowAsync();
            }

            else if (!Regex.IsMatch(uname.Text.Trim(), @"^[A-Za-z_][a-zA-Z0-9_\s]*$"))
            {
                await new MessageDialog("Invalid Username.").ShowAsync();

            }

            else if (!Regex.IsMatch(email.Text.Trim(), @"^([a-zA-Z_])([a-zA-Z0-9_\-\.]*)@(\[((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.){3}|((([a-zA-Z0-9\-]+)\.)+))([a-zA-Z]{2,}|(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\])$"))
            {
                await new MessageDialog("Invalid email").ShowAsync();
            }

            else if(pass.Password.Length<8)
            {
                await new MessageDialog("Invalid password. Minimum 8 character required.").ShowAsync();

            }

            else if(phone.Text.Length<10)
            {
                await new MessageDialog("Invalid Phone.").ShowAsync();

            }

            else
            {
                pring.IsActive = true;
                var item = new Customer

                {
                    Fullname = fname.Text,
                    Dob = dob.Date.ToString(@"dd\/M\/yyyy"),
                    Phone = phone.Text,
                    Email = email.Text,
                    Username = uname.Text,
                    Password = pass.Password
                };
                await InsertTodoItem(item);
            }

            pring.IsActive = false;

            
        }

        //------------------------------------- For LOGIN PAGE ----------------------------------------------------
        private async void loginbutton_Click(object sender, RoutedEventArgs e)
        {
            loginring.IsActive = true;
            try
            {
                userback = username.Text;
                {

                    items = await itemsTable.Where(x => x.Username == username.Text && x.Password == password.Password).ToCollectionAsync();

                    if (items.Count > 0)
                    {
                        Frame.Navigate(typeof(map));
                    }

                    else
                    {
                        await new MessageDialog("Username or Password is invalid!").ShowAsync();
                    }

                }

            }

            catch (Exception ex)
            {
                await new MessageDialog("Something went wrong. Try again!").ShowAsync();
            }
            loginring.IsActive = false;
            
        }

        private bool _triggerCompleted;

        private const double SideMenuCollapsedLeft = -375;
        private const double SideMenuExpandedLeft = 0;

        private void Sidebar_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            _triggerCompleted = true;

            double finalLeft = Canvas.GetLeft(Sidebar) + e.Delta.Translation.X;
            if (finalLeft < -375 || finalLeft > 0)
                return;

            Canvas.SetLeft(Sidebar, finalLeft);

            if (e.IsInertial && e.Velocities.Linear.X > 1)
            {
                _triggerCompleted = false;
                e.Complete();
                MoveLeft(SideMenuExpandedLeft);
            }

            if (e.IsInertial && e.Velocities.Linear.X < -1)
            {
                _triggerCompleted = false;
                e.Complete();
                MoveLeft(SideMenuCollapsedLeft);
            }

            if (e.IsInertial && Math.Abs(e.Velocities.Linear.X) <= 1)
                e.Complete();
        }

        private void Sidebar_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (_triggerCompleted == false)
                return;

            double finalLeft = Canvas.GetLeft(Sidebar);

            if (finalLeft > -187)
                MoveLeft(SideMenuExpandedLeft);
            else
                MoveLeft(SideMenuCollapsedLeft);
        }

        private void MoveLeft(double left)
        {
            double finalLeft = Canvas.GetLeft(Sidebar);

            Storyboard moveAnivation = ((Storyboard)RootCanvas.Resources["MoveAnimation"]);
            DoubleAnimation direction = ((DoubleAnimation)((Storyboard)RootCanvas.Resources["MoveAnimation"]).Children[0]);

            direction.From = finalLeft;

            moveAnivation.SkipToFill();

            direction.To = left;

            moveAnivation.Begin();
            
        }
    }
}
