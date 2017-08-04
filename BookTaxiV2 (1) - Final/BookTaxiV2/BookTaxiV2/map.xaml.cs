using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace BookTaxiV2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class map : Page
    {
        double latt,lon;
        Geolocator locator;
        string validuser = Login.userback;
        MapIcon mapIcon;
        TimePicker reqtime;
        DispatcherTimer timer;
        int maxTime = 31;


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

        public map()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            progressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        //Azure Start
        private async Task InsertTodoItem(TaxiRequest student)
        {
            // This code inserts a new TodoItem into the database. When the operation completes
            // and Mobile App backend has assigned an Id, the item is added to the CollectionView.
            await itemsTable.InsertAsync(student);
        }
        //Azure End

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                e.Handled = true;
                Frame.GoBack();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MyMap.MapServiceToken = "abcdef-abcdefghijklmno";
            reqtime = new TimePicker();
            reqtime.Name = "r_time";
            reqtime.HorizontalAlignment = HorizontalAlignment.Left;
            reqtime.VerticalAlignment = VerticalAlignment.Bottom;
            reqtime.Margin = new Thickness(14, 0, 0, 4);
            reqtime.Foreground = new SolidColorBrush(Colors.Black);
            map1.Children.Add(reqtime);
            locate();
            base.OnNavigatedTo(e);

        }

        public async void locate()
        {
            progressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            positionTextBlock.Text = "Searching your location...";
            locator = new Geolocator();
            locator.DesiredAccuracyInMeters = 50;
 
            try
            {
                Geoposition geoposition = await locator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10));

                mapIcon = new MapIcon();
                mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/Images/Maps-Define-Location-icon.png"));
                mapIcon.Title = "Current Location";
                mapIcon.Location = new Geopoint(new BasicGeoposition()
                {
                    //Latitude = geoposition.Coordinate.Latitude,
                    //Longitude = geoposition.Coordinate.Longitude
                    Latitude = geoposition.Coordinate.Point.Position.Latitude,
                    Longitude = geoposition.Coordinate.Point.Position.Longitude
                });
                mapIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
                MyMap.MapElements.Add(mapIcon);
                await MyMap.TrySetViewAsync(mapIcon.Location, 18D, 0, 0, MapAnimationKind.Bow);

                progressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                mySlider.Value = MyMap.ZoomLevel;           
                
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox("", "Location service is turned off!");
            }
            positionTextBlock.Text = "";
        }
        private DependencyObject CreatePushPin()
        {
            // Creating a Polygon Marker
            Polygon polygon = new Polygon();
            polygon.Points.Add(new Point(0, 0));
            polygon.Points.Add(new Point(0, 50));
            polygon.Points.Add(new Point(25, 0));
            polygon.Fill = new SolidColorBrush(Colors.Red);
            return polygon;
        }

        void timer_Tick(object sender, object e)
        {
            maxTime--;
            positionTextBlock.Text = "Cancelling Time:  " + maxTime.ToString() + "(s)";
            if (maxTime == 0)
            {
                timer.Stop();
                reqhere.Visibility = Visibility.Collapsed;
                logreg.Visibility = Visibility.Collapsed;
                progressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                positionTextBlock.Text = "Thank you for using our service.";
            }
        }

        private async void reqhere_Click(object sender, RoutedEventArgs e)
        {

            if (reqhere.Label == "Request")
            {
               
                string time = DateTime.Now.TimeOfDay.ToString();
                string rtime = reqtime.Time.ToString();
                if (reqtime.Time < DateTime.Now.TimeOfDay)
                {
                    MessageBox("", "Invalid Time Chosen!");
                }
                else
                {
                    try
                    {
                        reqhere.Icon = new SymbolIcon(Symbol.Cancel);
                        reqhere.Label = "Cancel Request";
                        latt = MyMap.Center.Position.Latitude;
                    lon = MyMap.Center.Position.Longitude;
                    var items = new TaxiRequest { Username = validuser, Lattitude = latt.ToString(), Longitude = lon.ToString(), Time = time, RTime = rtime };

                    var messagedialog = new MessageDialog("Time of Request: " + reqtime.Time, "Please Confirm your Taxi booking, " + validuser);
                    //First command starts
                    messagedialog.Commands.Add(new UICommand("Yes", async (UICommandInvokeHandler) =>
                    {
                        await InsertTodoItem(items);
                        progressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        positionTextBlock.Text = "Your request is on the way...";
                        timer.Start();

                       
                    }
                        )); //First Command Ends here
                            // Second command starts here
                    messagedialog.Commands.Add(new UICommand("No"));


                    await messagedialog.ShowAsync();

                    
                    }
                    catch (Exception ex)
                    {
                        MessageBox("Error. Try again!", "");
                    }
                }
            }
            else
            {
                progressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                reqhere.Icon = new SymbolIcon(Symbol.ReShare);
                reqhere.Label = "Request";
                timer.Stop();
                maxTime = 31;
                positionTextBlock.Text = "Your Request has been cancelled!";
            }
        }

        
        private void mylocation_Click(object sender, RoutedEventArgs e)
        {
            MyMap.MapElements.Remove(mapIcon);           
            locate();
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (MyMap != null)
                MyMap.ZoomLevel = e.NewValue;
        }

        private async void MyMap_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            progressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Geopoint pointToReverseGeocode = new Geopoint(args.Location.Position);

            // Reverse geocode the specified geographic location.
            MapLocationFinderResult result =
                await MapLocationFinder.FindLocationsAtAsync(pointToReverseGeocode);

            var resultText = new StringBuilder();

            if (result.Status == MapLocationFinderStatus.Success)
            {
                resultText.AppendLine("Location : "+result.Locations[0].Address.District + ", " + result.Locations[0].Address.Town + ", " + result.Locations[0].Address.Country);
            }
            positionTextBlock.Text = resultText.ToString();
            progressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private async void MessageBox(string message, string m)
        {
            var dialog = new MessageDialog(message.ToString(), m.ToString());
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await dialog.ShowAsync());
        }

        private void logreg_Click(object sender, RoutedEventArgs e)
        {
            MyMap.MapElements.Remove(mapIcon);
            map1.Children.Remove(reqtime);
            Application.Current.Exit();
            
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            taxifare.Visibility = Visibility.Collapsed;
        }

        private void map1_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
        }

        private void help_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(BlankPage1));
        }

        private void price_Click(object sender, RoutedEventArgs e)
        {
            taxifare.Visibility = Visibility.Visible;
        }
    }
}
