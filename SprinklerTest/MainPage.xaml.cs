using SprinklerCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SprinklerTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SprinklerController _sprinklerController;

        public MainPage()
        {
            _sprinklerController = new SprinklerController();
            this.InitializeComponent();
            //this.listBox.ItemsSource = _sprinklerController.Zones.Select(zone => zone.ToString());
            DebuggerTest();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //var isRunning = _sprinklerController.IsZoneRunning(int.Parse((string)listBox.SelectedItem));
            //textBlock.Text = isRunning.ToString();
        }

        private void DebuggerTest()
        {
            var zones = _sprinklerController.GetAllZones();
            foreach (var zone in zones)
            {
                var isRunning = _sprinklerController.IsZoneRunning(zone.ZoneNumber);
                Debug.WriteLine("Zone " + zone + " isRunning = " + isRunning);
                _sprinklerController.StartZone(zone.ZoneNumber);
                //_sprinklerController.StopZone(zone);
         

            }
        }
           
    }
}
