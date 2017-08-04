using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TaxiControlCenter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    public sealed partial class TaxiDriver : Page
    {

        public class DriverInfo
        {
            public string Id { get; set; }
            public string Drivername { get; set; }
            public string Driverphone { get; set; }
            public string Taxiid { get; set; }
        }


        private MobileServiceCollection<DriverInfo, DriverInfo> items;
        private IMobileServiceTable<DriverInfo> itemsTable = App.MobileService.GetTable<DriverInfo>();



        public TaxiDriver()
        {
            this.InitializeComponent();
            RefreshTodoItems();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private async void refresh_Click(object sender, RoutedEventArgs e)
        {
            ringg.IsActive = true;
            await RefreshTodoItems();
            ringg.IsActive = false;
        }


        private async Task InsertTodoItem(DriverInfo taxidept)
        {
            // This code inserts a new TodoItem into the database. When the operation completes
            // and Mobile App backend has assigned an Id, the item is added to the CollectionView.
            try
            {
                await itemsTable.InsertAsync(taxidept);
                //MessageBox("You are now Registered.", "Congratulations!!!");
                //await new MessageDialog("You are now Registered").ShowAsync();

                var messagedialog = new MessageDialog("Driver Information Updated!", "Registration Successful!");
                drivername.Text = "";
                phone.Text = "";
                tid.Text = "";
                //First command starts
                // Second command starts here
                await messagedialog.ShowAsync();
                await RefreshTodoItems();
                
            }
            catch (Exception ex)
            {
                var messagedialog = new MessageDialog("Please check your internet connection.", "Something is wrong!");
                //First command starts

                await messagedialog.ShowAsync();
            }
        }


        private async void enter_Click(object sender, RoutedEventArgs e)
        {
            var item = new DriverInfo { Drivername = drivername.Text, Driverphone = phone.Text, Taxiid = tid.Text };
            await InsertTodoItem(item);
        }


        private async Task RefreshTodoItems()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                // This code refreshes the entries in the list view by querying the TodoItems table.
                // The query excludes completed TodoItems.
                items = await
                itemsTable.ToCollectionAsync();
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }
            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loading items").ShowAsync();
            }
            else
            {
                ListItems.ItemsSource = items;
            }
        }
    }
}

