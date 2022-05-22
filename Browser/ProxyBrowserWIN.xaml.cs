using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WebTraffic_Exchanger.Browser
{
    /// <summary>
    /// Interaction logic for ProxyBrowserWIN.xaml
    /// </summary>
    public partial class ProxyBrowserWIN : Window
    {
        public ProxyBrowserWIN()
        {
            InitializeComponent();
        }
        bool closed = false;

        public void RunURL(string url)
        {
            this.MainWB.Navigate(url, null, null, userAgent);

        }

        private void MainWB_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            classes.MakeSilent.SetSilent(MainWB, true);
        }
        public string userAgent = "";

        private void MainWB_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            
        }
    }
}
