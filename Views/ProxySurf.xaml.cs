using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Xml.XPath;
using SetProxy;
using Microsoft.Win32;
using System.Threading;
using System.Reflection;
using System.ComponentModel;

namespace WebTraffic_Exchanger.Views
{

    public partial class ProxySurf : Page
    {
        public ProxySurf()
        {
            InitializeComponent();
        }

        //RegistryKey reg_key;
        int current = 0;
        bool finished = true;
        List<Browser.ProxyBrowserWIN> proxybrowsers = new List<Browser.ProxyBrowserWIN>();
        System.Threading.Thread work_thread;
        List<string> allUserAgetnt = new List<string>();

        //XML decode
        public List<string> RetrieveValuesForAttribute(XDocument doc, string attributeName)
        {
            return doc.Descendants()
                           .Attributes(attributeName)
                           .Select(x => x.Value)
                           .ToList();
        }

        private async void SurfBTN_Click(object sender, RoutedEventArgs e)
        {
            SurfBTN.IsEnabled = false;
            StopSurfBTN.IsEnabled = true;

            work_thread = new Thread(new ThreadStart(KeepWork));
            work_thread.Start();
        }

        private async void KeepWork()
        {
            while (true)
            {
                for (int x = 0; x < ProxyGrid.Items.Count; x++)
                {
                    while (finished == false)
                    {
                        if (current == WebsiteGrid.Items.Count)
                        {
                            current = 0;
                            finished = true;
                        }
                        System.Windows.Forms.Application.DoEvents();
                    }

                    var obj = (Proxy)ProxyGrid.Items[x];
                    WinInetInterop.SetConnectionProxy(obj.proxy);

                    for (int i = 0; i < WebsiteGrid.Items.Count; i++)
                    {
                        current = 0;
                        finished = false;
                        this.Dispatcher.Invoke(() =>
                        {
                            Browser.ProxyBrowserWIN proxyBrowser = new Browser.ProxyBrowserWIN();
                            proxyBrowser.RunURL(((Website)WebsiteGrid.Items[i]).Name);
                            String useragent = "User-Agent: " + allUserAgetnt[i];

                            proxyBrowser.MainWB.LoadCompleted += async (seder, ex) => Random_click(proxyBrowser);
                            proxyBrowser.MainWB.LoadCompleted += async (seder, ex) => proxyBrowser.MainWB_LoadCompleted(Destroy_Browser(proxyBrowser));

                            proxyBrowser.Show();
                            proxybrowsers.Add(proxyBrowser);
                        });

                    }
                }
            }
        }


        private bool Destroy_Browser(Browser.ProxyBrowserWIN proxyBrowser)
        {
            if (proxybrowsers.Where(e => e == proxyBrowser).Count() > 0)
            {
                proxybrowsers.Remove(proxyBrowser);
                totalHitsTXT.Text = (int.Parse(totalHitsTXT.Text) + 1).ToString();
                return false;
            }
            return true;

        }
        private async void Random_click(Browser.ProxyBrowserWIN proxyBrowser)
        {

            if (!Destroy_Browser(proxyBrowser))
            {
                current += 1;
                Random rnd = new Random();

                int random_time = rnd.Next(3000, 10000);
                await Task.Delay(random_time);

                proxyBrowser.Close();

            }
        }


        private void addWebsiteBTN_Click(object sender, RoutedEventArgs e)
        {
            string[] allWebsties = websiteTXT.Text.Split(' ');
            foreach (string website in allWebsties)
            {
                WebsiteGrid.Items.Add(new Website() { ID = WebsiteGrid.Items.Count, Name = website });
            }
            websiteTXT.Text = "";
        }


        private async void LoadProxyBTN_Click(object sender, RoutedEventArgs e)
        {
            LoadProxyBTN.IsEnabled = false;

            //UserAgent
            var client = new WebClient();
            string xml = await client.DownloadStringTaskAsync("https://techpatterns.com/downloads/firefox/useragentswitcher.xml");
            XDocument doc = XDocument.Parse(xml);
            allUserAgetnt = RetrieveValuesForAttribute(doc, "useragent");

            //Proxies
            var url = "https://api.proxyscrape.com/v2/account/datacenter_shared/proxy-list?auth=exrabwwjri9qbck50dqn&type=getproxies&country[]=all&protocol=http&format=normal&status=online";
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            var httpResponse = await httpRequest.GetResponseAsync();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                string[] alllines = Regex.Split(result, @"\r?\n|\r");
                for (int i = 0; i < alllines.Length; i++)
                {
                    if (alllines[i] != "")
                    {
                        ProxyGrid.Items.Add(new Proxy() { ID = ProxyGrid.Items.Count, proxy = alllines[i] });
                    }
                }
                LoadProxyBTN.IsEnabled = true;
            }
        }

        private void StopSurfBTN_Click(object sender, RoutedEventArgs e)
        {
            work_thread.Suspend();
            StopSurfBTN.IsEnabled = false;
            SurfBTN.IsEnabled = true;
        }

        private async void ReadWebsiteBTN_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.ShowDialog();
            using (System.IO.StreamReader SR = new System.IO.StreamReader(ofd.FileName))
            {
                string line = String.Empty;

                while ((line = SR.ReadLine()) != null && line != "")
                {
                    WebsiteGrid.Items.Add(new Website() { ID = WebsiteGrid.Items.Count, Name = line.Trim() });
                }
            }
        }
    }
    class Website
    {

        private int _id;
        public int ID
        {
            get
            {
                return _id;
            }
            set
            {
                this._id = value;
            }
        }


        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                this._name = value;
            }
        }
    }


    class Proxy
    {

        private int _id;
        public int ID
        {
            get
            {
                return _id;
            }
            set
            {
                this._id = value;
            }
        }


        private string _proxy;
        public string proxy
        {
            get
            {
                return _proxy;
            }
            set
            {
                this._proxy = value;
            }
        }


        private string _status = "online";
        public string status
        {
            get
            {
                return _status;
            }
            set
            {
                this._status = value;
            }
        }

    }



}
