using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WebTraffic_Exchanger.Main
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        Datas.User user;
        public Main()
        {
            InitializeComponent();
            GetUser();
           
        }
        private async void GetUser()
        {
            classes.GetPost getpost = new classes.GetPost();
            var parsed = await getpost.GetbyQuery("http://webtraffic.live/api/user?id=" + Properties.Settings.Default.Authid );
             user = new Datas.User(parsed);

            this.DataContext = new TextboxText() { username = user.username, credits =  user.credits, userlevel  = user.userlevel == 0?"Hidden":"Visible"};
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        public class TextboxText
        {
            public string username { get; set; }
            public float credits { get; set; }
            public string userlevel { get; set; }
        }

        private void EarnBTN_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Views.EarnPage earnpage = new Views.EarnPage();
            PagesFrames.Navigate(earnpage);
        }

        private void ProxySurfBTN_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Views.ProxySurf proxypage = new Views.ProxySurf();
            PagesFrames.Navigate(proxypage);
        }

        private void logoutBTN_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.logged = false;
            Properties.Settings.Default.Save();
            var w = Application.Current.Windows[0]; 
            this.Close();
            w.Show();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            GetUser();
        }
    }
}
