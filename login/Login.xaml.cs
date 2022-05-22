
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace WebTraffic_Exchanger.login
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {

        public Login()
        {
            if (Properties.Settings.Default.logged == true)
            {
                CloseAndMain();
            }
            InitializeComponent();
        }


        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://webtraffic.live/password/reset");
        }
        private void Signup_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://webtraffic.live/register");
        }

        private async void LoginBTN_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                classes.GetPost getpost = new classes.GetPost();
                var parsed = await getpost.Post("{\n\t\"email\":\"" + usernameTXT.Text + "\",\n\t\"password\":\"" + passwordTXT.Password + "\"\n\t\n}", "http://webtraffic.live/api/login");
                int authid = int.Parse(parsed.SelectToken("id").ToString());
                Properties.Settings.Default.Authid = authid;
                Properties.Settings.Default.logged = true;

                Properties.Settings.Default.Save();
                CloseAndMain();
            }
            catch (Exception ex)
            {

            }

        }


        private void CloseAndMain()
        {
            var w = Application.Current.Windows[0];
            w.Hide();
            Main.Main main = new Main.Main();
            main.ShowDialog();
        }
    }
}
