using Bing.Maps;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
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
    public sealed partial class MainPage : Page
    {

        int count = 1;
        
        double lattt;
        double longgg;
        Pushpin pushpin;
        MapLayer pushpinLayer;
        public string user_name;
        string[] client = new string[50];
        CheckBox[] cb = new CheckBox[50];

        public class TaxiRequest
        {
            public string Id { get; set; }
            public string Username { get; set; }
            public string Lattitude { get; set; }
            public string Longitude { get; set; }
            public string Time { get; set; }
            public string RTime { get; set; }

        }
        

        private MobileServiceCollection<TaxiRequest, TaxiRequest> items;
        private IMobileServiceTable<TaxiRequest> itemsTable = App.MobileService.GetTable<TaxiRequest>();

        public MainPage()
        {
            this.InitializeComponent();
            RefreshTodoItems();
        }

        public void mappp(double lattt, double longgg, int count)
        {
            try
            {
                Location location = new Location(lattt , longgg );
                myMap.SetView(location, 18D);

                pushpinLayer = new MapLayer();
                myMap.Children.Add(pushpinLayer);

                pushpin = new Pushpin();
                pushpin.Text = count.ToString();
                pushpin.Tapped += new TappedEventHandler(myTap);
                pushpin.DoubleTapped += new DoubleTappedEventHandler(dtap);
                MapLayer.SetPosition(pushpin, location);
                pushpin.Background = new SolidColorBrush(Colors.Blue);

                pushpinLayer.Children.Add(pushpin);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox("", "Location service is turned off!");
            }
        }

        private void dtap(object sender, DoubleTappedRoutedEventArgs e)
        {
            myMap.Children.Remove(pushpinLayer);
            pushpinLayer.Children.Remove(pushpin);
        }

        private async void myTap(object sender, TappedRoutedEventArgs e)
        {
            Pushpin p  = (Pushpin)sender;
            MessageDialog dialog = new MessageDialog("Taxi request from " + client[int.Parse(p.Text)], "Dispatch Taxi?");
            dialog.Commands.Add(new UICommand("Dispatch", async (UICommandInvokeHandler) =>
            {
                TaxiRequest item = cb[int.Parse(p.Text)].DataContext as TaxiRequest;
                await UpdateChecked(item);
                pushpinLayer.Children.Remove(p);
                message("Dispatch");

            }
                )); //First Command Ends here
            // Second command starts here
            dialog.Commands.Add(new UICommand("Don't Dispatch!", async (UICommandInvokeHandler) =>
            {
                TaxiRequest item = cb[int.Parse(p.Text)].DataContext as TaxiRequest;
                await UpdateChecked(item);
                pushpinLayer.Children.Remove(p);
                message("Don't");
            }
                ));
            dialog.Commands.Add(new UICommand("Cancel"));
            await dialog.ShowAsync();
            await RefreshTodoItems();
            
        }

        private async Task UpdateChecked(TaxiRequest item)
        {
            // This code takes a freshly completed TodoItem and updates the database. When the MobileService 
            // responds, the item is removed from the list 

            items.Remove(item);
            ListItems.Focus(Windows.UI.Xaml.FocusState.Unfocused);
            await itemsTable.DeleteAsync(item);


        }

        private void message(string str)
        {
            if(str=="Dispatch")
            {
                MessageBox("Customer detail has been sent to the driver!", "Dispatched!");
            }
            else if(str == "Don't")
            {
                MessageBox("The request has been cancelled.", "Not Dispatched!");
            }

        }

        private async void MessageBox(string message, string m)
        {
            var dialog = new MessageDialog(message.ToString(), m.ToString());
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await dialog.ShowAsync());
        }

        private void TaxDri_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TaxiDriver));
        }

        private void UserD_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(User));
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

        private async void nameblk_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CheckBox tb = (CheckBox)sender;
            List<TaxiRequest> items = await itemsTable.Where(x => x.Username == tb.Content.ToString()).ToListAsync();
            ListItems.ItemsSource = items;


            var naam = items.First();

            user_name = tb.Content.ToString();
            string latstr = naam.Lattitude;          
            string lonstr = naam.Longitude;
     
            lattt = double.Parse(latstr);
            longgg = double.Parse(lonstr);

            cb[count] = new CheckBox();
            cb[count].DataContext = tb.DataContext;

            client[count] = user_name;
            mappp(lattt, longgg, count);
            count++;
            await RefreshTodoItems();
        }

        private async void refresh_Click(object sender, RoutedEventArgs e)
        {
            rin.IsActive = true;
            await RefreshTodoItems();
            rin.IsActive = false;
        }
    }
}
