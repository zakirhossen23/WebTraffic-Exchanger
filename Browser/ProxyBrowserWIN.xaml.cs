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
            MainWB.Source = new Uri(url);

        }

        private void MainWB_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            classes.MakeSilent.SetSilent(MainWB, true);
        }

        public async void MainWB_LoadCompleted(bool isClicked)
        {
            try
            {
                if (isClicked || !closed)
                {
                    var document = MainWB.Document as mshtml.HTMLDocument;
                    var inputs = document.getElementsByTagName("a");
                    foreach (mshtml.IHTMLElement element in inputs)
                    {
                        if (element.getAttribute("href") != "" && element.getAttribute("href") != MainWB.Source.OriginalString && !element.getAttribute("href").Contains("#"))
                        {
                            string url = element.getAttribute("href").ToString();
                            MainWB.Source = new Uri(url);
                            closed = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex) { }


        }

    }
}
