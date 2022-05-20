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
namespace WebTraffic_Exchanger.Views
{
    /// <summary>
    /// Interaction logic for ProxySurf.xaml
    /// </summary>
    public partial class ProxySurf : Page
    {
        RegistryKey reg_key;

        public ProxySurf()
        {
            InitializeComponent();
        }

        public List<string> RetrieveValuesForAttribute(XDocument doc, string attributeName)
        {
            // Assume document is an XDocument
            return doc.Descendants()
                           .Attributes(attributeName)
                           .Select(x => x.Value)
                           .ToList();
        }
        int current = 0;
        bool finished = true;
        List<string> allUserAgetnt = new List<string>();
        private async void SurfBTN_Click(object sender, RoutedEventArgs e)
        {
           reg_key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            reg_key.SetValue("ProxyEnable", 1);
            SurfBTN.IsEnabled = false;
            var client = new WebClient();

            string xml = await client.DownloadStringTaskAsync("https://techpatterns.com/downloads/firefox/useragentswitcher.xml");
            XDocument doc = XDocument.Parse(xml);
            List<string> allUserAgetnt = RetrieveValuesForAttribute(doc, "useragent");
            for (int x=0; x < ProxyGrid.Items.Count; x++)
            {
                while (finished == false)
                {
                    if (current == WebsiteGrid.Items.Count)
                    {
                        finished = true;
                    }
                    System.Windows.Forms.Application.DoEvents();
                }
                        
                var obj = (Proxy)ProxyGrid.Items[x];
                //reg_key.SetValue("ProxyServer", obj.proxy);
                WinInetInterop.SetConnectionProxy(obj.proxy);

                for (int i = 0; i < WebsiteGrid.Items.Count; i++)
                {
                    current = 0;
                    finished = false;
                    System.Windows.Forms.WebBrowser webBrowser = new System.Windows.Forms.WebBrowser();
                    webBrowser.ScriptErrorsSuppressed = true;
                    String useragent = "User-Agent: " + allUserAgetnt[i];
                    webBrowser.Navigate(((Website)WebsiteGrid.Items[i]).Name, null, null, useragent);
                    webBrowser.DocumentCompleted += (seder, ex) => Random_click(webBrowser);
                }
            }
            SurfBTN.IsEnabled = true;
        }


        private void Random_click(System.Windows.Forms.WebBrowser webBrowser)
        {
            current += 1;
            try
            {
                HtmlElementCollection elc = webBrowser.Document.GetElementsByTagName("a");
                foreach (HtmlElement el in elc)
                {
                    if (el.GetAttribute("href") != "" && !el.GetAttribute("href").Contains("#"))
                    {
                        totalHitsTXT.Text = (int.Parse(totalHitsTXT.Text) + 1).ToString();
                        el.InvokeMember("Click");                        
                        break;
                    }
                }
            } catch (Exception ex)
            { }


        }

        private void addWebsiteBTN_Click(object sender, RoutedEventArgs e)
        {
            WebsiteGrid.Items.Add(new Website() { ID = WebsiteGrid.Items.Count, Name = websiteTXT.Text });
            websiteTXT.Text = "";
        }


        private async void LoadProxyBTN_Click(object sender, RoutedEventArgs e)
        {
            LoadProxyBTN.IsEnabled = false;
            var url = "https://api.proxyscrape.com/v2/account/datacenter_shared/proxy-list?auth=exrabwwjri9qbck50dqn&type=getproxies&country[]=all&protocol=http&format=normal&status=online";

            var httpRequest =  (HttpWebRequest)WebRequest.Create(url);

            var httpResponse =await httpRequest.GetResponseAsync();
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
    }
    class Website{

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
       
        
        private string _status ="online";
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
