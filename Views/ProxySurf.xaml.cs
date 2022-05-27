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
using System.Diagnostics;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace WebTraffic_Exchanger.Views
{

    public partial class ProxySurf : Page
    {
        public ProxySurf()
        {
            InitializeComponent();
        }

        #region "Setting value of chrome"
        String crhomeversion = "";
        IWebDriver driver;
        int height = Screen.AllScreens[0].WorkingArea.Height / 2;
        int width = Screen.AllScreens[0].WorkingArea.Width / 2;
        #endregion

        #region "Waiting"
        bool isDriverWorking = false;
        bool isOpeining = false;
        private async Task waitUntilDriver(int waittype)
        {
            while (isDriverWorking == true)
            {
                System.Windows.Forms.Application.DoEvents();
                if (waittype == 1)
                    await Task.Delay(500);
                else await Task.Delay(300);
            }
            isDriverWorking = true;
        }
        #endregion



        public void changeStatus(string status)
        {
            Main.Main myWindow = (Main.Main)System.Windows.Application.Current.Windows[1];

            myWindow.statusTXT.Text = status;
        }

        //RegistryKey reg_key;
        int current = 0;
        bool finished = true;
        Random rnd = new Random();
        List<string> browserHandalers = new List<string>();
        List<IWebDriver> browserDriver = new List<IWebDriver>();
        System.Threading.Thread work_thread;

        //XML decode
        public List<string> RetrieveValuesForAttribute(XDocument doc, string attributeName)
        {
            return doc.Descendants()
                           .Attributes(attributeName)
                           .Select(x => x.Value)
                           .ToList();
        }
        private void UpdateAndOpenDriver(string proxy)
        {
            bool update = false;
            while (true)
            {
                #region  "Driver Opening"
                if (update == true)
                {
                    object path = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", "", null);
                    object path2 = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", "", null);
                    if (path != null)
                        crhomeversion = FileVersionInfo.GetVersionInfo(path.ToString()).FileVersion;
                    if (path2 != null)
                        crhomeversion = FileVersionInfo.GetVersionInfo(path2.ToString()).FileVersion; ;
                    string GetListxml = "";

                    using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
                    {
                        string url = "https://chromedriver.storage.googleapis.com/?delimiter=/&prefix=";
                        GetListxml = client.DownloadString(String.Format("{0}{1}", url, crhomeversion.Split('.')[0]));
                    }
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(GetListxml);
                    string ChromeDriverVersion = "";
                    foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                    {
                        if (node.InnerText.Contains(String.Format("{0}.", crhomeversion.Split('.')[0])))
                        {
                            ChromeDriverVersion = node.InnerText; //or loop through its children as well
                            break;
                        }
                    }
                    WebClient webClient = new WebClient();
                    webClient.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                    webClient.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
                    webClient.DownloadFile(new Uri(String.Format("https://chromedriver.storage.googleapis.com/{0}chromedriver_win32.zip", ChromeDriverVersion)), "chromedriver_win32.zip");

                    System.Windows.Forms.MessageBox.Show("Close all the chromedriver.exe from Taskbar to continue!");

                    var zipFileName = String.Format("{0}\\{1}", System.Windows.Forms.Application.StartupPath, "chromedriver_win32.zip");
                    var targetDir = String.Format("{0}\\chromedriver", System.Windows.Forms.Application.StartupPath);
                    FastZip fastZip = new FastZip();
                    string fileFilter = null;

                    // Will always overwrite if target filenames already exist
                    fastZip.ExtractZip(zipFileName, targetDir, fileFilter);
                }

                try
                {
                    var chromeDriverService = ChromeDriverService.CreateDefaultService("./chromedriver");
                    chromeDriverService.HideCommandPromptWindow = true;
                    ChromeOptions chromeOptions = new ChromeOptions();
                    string arg = String.Format("--window-size={0},{1} --proxy-server={2}", width.ToString(), height.ToString(), proxy);
                    chromeOptions.AddArgument(arg);
                    driver = new ChromeDriver(chromeDriverService, chromeOptions);
                    driver.Manage().Window.Position = new System.Drawing.Point(width, 0);
                    break;
                }
                catch (Exception ex)
                {
                    update = true;
                    continue;
                }
                #endregion
            }
        }

        private async void SurfBTN_Click(object sender, RoutedEventArgs e)
        {
            changeStatus("Statring...");
            SurfBTN.IsEnabled = false;
            StopSurfBTN.IsEnabled = true;
            browserHandalers = new List<string>();
            work_thread = new Thread(new ThreadStart(KeepWork));
            work_thread.Start();
        }
        int currentProxy = 0;
        private async void KeepWork()
        {
            while (true)
            {
                for (int i = 0; i < WebsiteGrid.Items.Count; i++)
                {
                    int max_window = 10;
                    this.Dispatcher.Invoke(() =>
                    {
                        max_window = (int)maximum_windows.Value;
                    });
                    while (finished == false || browserDriver.Count >= max_window)
                    {
                        if (current <= max_window)
                        {
                            current = 0;
                            finished = true;
                        }
                        System.Windows.Forms.Application.DoEvents();
                    }
                    var obj = (Proxy)ProxyGrid.Items[currentProxy];
                    this.Dispatcher.Invoke(() =>
                    {
                        changeStatus(String.Format("Opening chrome as {0} proxy", obj.proxy));
                        UpdateAndOpenDriver(obj.proxy);
                        WebsiteGrid.SelectedIndex = i;
                        WebsiteGrid.ScrollIntoView(WebsiteGrid.Items[i]);
                        ProxyGrid.SelectedIndex = (currentProxy);
                        ProxyGrid.ScrollIntoView(ProxyGrid.Items[currentProxy]);
                    });

                    browserDriver.Add(driver);

                    string goingURL = ((Website)WebsiteGrid.Items[i]).Name;
                    string executingGoingString = @"setTimeout (function () { window.location.href = '" + goingURL + "' }, " + (1000).ToString() + ");";

                    ((IJavaScriptExecutor)driver).ExecuteScript(executingGoingString);

                    this.Dispatcher.Invoke(() =>
                    {

                        int manimum_Val = 0; int maximum_Val = 0; int all_proxy = 0;
                        manimum_Val = ((int)Minimum_Seconds.Value);
                        maximum_Val = ((int)Maximum_Seconds.Value);
                        all_proxy = ProxyGrid.Items.Count;

                        int random_time = rnd.Next(manimum_Val, maximum_Val);
          

                        Thread thread = new Thread(async (seder) => Random_click(driver, random_time+10, i));
                        thread.Start();

                        if (all_proxy <= currentProxy)
                        {
                            currentProxy = 0;
                        }
                        else
                        {
                            currentProxy += 1;
                        }
                    });
                }
            }
        }


        private bool Destroy_Browser(IWebDriver Current_driver)
        {
            try
            {
                if (browserDriver.Where(e => e == Current_driver).Count() > 0)
                {
                    return false;
                }
            }
            catch (Exception)
            {
            }

            return true;

        }
        private async void Random_click(IWebDriver Current_driver, int random_time, int auto)
        {

            if (!Destroy_Browser(Current_driver))
            {

                await Task.Delay(random_time * 1000);

                browserDriver.Remove(Current_driver);

#pragma warning disable CS4014 
                this.Dispatcher.Invoke(async() =>
                {
                    Current_driver.Quit();
                    totalHitsTXT.Text = (int.Parse(totalHitsTXT.Text) + 1).ToString();
                    changeStatus(String.Format("Clicked!"));
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                current += 1;
                Thread.CurrentThread.Abort();
                return;

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
            driver.Quit();
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

