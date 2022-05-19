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

namespace WebTraffic_Exchanger.Browser
{
    /// <summary>
    /// Interaction logic for browserWin.xaml
    /// </summary>
    public partial class browserWin : Window
    {
        public bool closed = false;
        public browserWin()
        {
            InitializeComponent();
               
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.closed = true;
        }

        public void changeURL(String url)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.wbMain.Address=( url);
            });
            
        }
        public void Show(String url)
        {
            this.wbMain.Address =( url);
            this.Show();
        }

    }
}
