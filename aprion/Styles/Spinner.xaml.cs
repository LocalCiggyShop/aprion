using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace aprion
{
    /// <summary>
    /// Interaction logic for Spinner.xaml
    /// </summary>
    public partial class Spinner : Page
    {
        //Init
        Functions PublicFunctions = new Functions();

        //Variables
        private int timePerAnim = 3;

        public Spinner()
        {
            InitializeComponent();

            try
            {
                Text.Text = PublicFunctions.randomString().ToString();

                PublicFunctions.Fade_out(Text, timePerAnim);
            }
            catch (Exception)
            {
                return;
            }
        }

        private void DiscordLink_Click(object sender, RoutedEventArgs e)
        {
            PublicFunctions.OpenLink("https://discord.gg/DrU7xDBJW2");
        }
    }
}
