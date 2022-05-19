using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SetProxy;

namespace WebTraffic_Exchanger.Views
{
    /// <summary>
    /// Interaction logic for EarnPage.xaml
    /// </summary>
    public partial class EarnPage : Page
    {
        public EarnPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        private static Timer timer;
        private static Timer surfTimer;
        private Browser.browserWin browsing;

        private async void SurfBTN_Click_1(object sender, RoutedEventArgs e)
        {
            openWindow(false);
            startKeepcheck();
        }

        private void startKeepcheck()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 1000;

            timer.Elapsed += KeepCheckingClosed;
            timer.Start();
        }

        private async void openWindow(bool changed)
        {
            classes.GetPost getpost = new classes.GetPost();
            var parsed = await getpost.GetbyQuery("http://webtraffic.live/api/session?userid=" + Properties.Settings.Default.Authid + "&getSite=1");
            Datas.Website website = new Datas.Website(parsed.SelectToken("website"));
          if (changed == true)
            {
                browsing.changeURL(website.url);
            }
            else
            {
            browsing = new Browser.browserWin();
            browsing.Show(website.url);
            }

            surfTimer = new Timer();
            surfTimer.Interval = 1000;
            surfTimer.Elapsed += (sender,e)=>keepSurfing(website);
            surfTimer.Start();
        }

        private void keepSurfing(Datas.Website website)
        {
           
            if (website.duration > 1)
            {
                website.duration -= 1;
                this.Dispatcher.Invoke(() =>
                {
                    this.timeleft.Text = website.duration.ToString();
                });
            }
            else
            {
               
                this.Dispatcher.Invoke(() =>
                {
                    earnedTXT.Text = String.Format("+{0} earned!", website.credits.ToString());
                    showEarnedTXT.IsChecked = false;
                    showEarnedTXT.IsChecked = true;
                    this.timeleft.Text ="0";
                });

                surfTimer.Close();
                openWindow(true);
            }
        }
        private void KeepCheckingClosed(Object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (browsing.closed)
                {                    
                    surfTimer.Close();
                    timer.Close();
                    MessageBox.Show("Closed window!");
                    this.Dispatcher.Invoke(() =>
                    {
                        this.timeleft.Text = "0";
                    });

                }
            }
            catch (Exception ex) { }
          
        }

    }
}
