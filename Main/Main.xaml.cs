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
        public Main()
        {
            InitializeComponent();
            this.DataContext = new TextboxText() { username = "Test" };
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        public class TextboxText
        {
            public string username { get; set; }

        }

        private void EarnBTN_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Views.EarnPage earnpage = new Views.EarnPage();
            PagesFrames.Navigate(earnpage);
        }
    }
}
