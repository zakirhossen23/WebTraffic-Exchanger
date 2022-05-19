using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
namespace WebTraffic_Exchanger.Views
{
    /// <summary>
    /// Interaction logic for ProxySurf.xaml
    /// </summary>
    public partial class ProxySurf : Page
    {
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
            var client = new WebClient();

            string xml = await client.DownloadStringTaskAsync("https://techpatterns.com/downloads/firefox/useragentswitcher.xml");
            XDocument doc = XDocument.Parse(xml);
            List<string> allUserAgetnt = RetrieveValuesForAttribute(doc, "useragent");
            for (int x=0; x < ProxyGrid.Items.Count; x++)
            {
                while (finished == false)
                {
                    if (current == threadTXT.Value)
                    {
                        finished = true;
                    }
                    System.Windows.Forms.Application.DoEvents();
                }
                        
                var obj = (Proxy)ProxyGrid.Items[x];
                WinInetInterop.SetConnectionProxy(obj.proxy);

                for (int i = 0; i < threadTXT.Value; i++)
                {
                    current = 0;
                    finished = false;
                    System.Windows.Forms.WebBrowser webBrowser = new System.Windows.Forms.WebBrowser();
                    webBrowser.ScriptErrorsSuppressed = true;
                    String useragent = "User-Agent: " + allUserAgetnt[i];
                    webBrowser.Navigate(websiteTXT.Text, null, null, useragent);
                    webBrowser.DocumentCompleted += (seder, ex) => Random_click(webBrowser);
                }

            }

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
            WebsiteGrid.Items.Add(new Website() { ID = WebsiteGrid.Items.Count, Name = websiteTXT.Text }) ;
            websiteTXT.Text = "";
        }

        private void addProxyBTN_Click(object sender, RoutedEventArgs e)
        {
            ProxyGrid.Items.Add(new Proxy() { ID = ProxyGrid.Items.Count, proxy = String.Format("{0}:{1}", this.proxyTXT.Text.Trim(), this.ProxyPortTXT.Text.Trim()) });
            proxyTXT.Text = "";
            ProxyPortTXT.Text = "";
        }

        private void openTxtBTN_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open Text File";
            theDialog.Filter = "TXT files|*.txt";
            theDialog.InitialDirectory = @"C:\";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = theDialog.FileName;

                string[] filelines = File.ReadAllLines(filename);
                
                for (int i = 0; i< filelines.Length; i++)
                {
                    ProxyGrid.Items.Add(new Proxy() { ID = ProxyGrid.Items.Count, proxy = filelines[i]});
                    
                }

                Console.WriteLine(filelines);
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
       
        
        private string _status;
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
