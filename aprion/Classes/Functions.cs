using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace aprion
{
    internal class Functions
    {
        //Arrays
        string[] tips = {
            "Feel free to join our discord to discuss any issues or questions",
        };

        public string randomString()
        {
            Random rand = new Random();
            int word = rand.Next(tips.Length);

            return tips[word];
        }
        public void OpenLink(string link)
        {
            string title = "Confirmation";
            string message = "You are about to open a link are you sure?";

            MessageBoxButton buttons = MessageBoxButton.YesNo;

            if (MessageBox.Show(message, title, buttons, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = link,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
        }

        public void Fade_out(object sender, int seconds)
        {
            TextBlock TextLabel = (TextBlock)sender;
            DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(seconds));
            TextLabel.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        public void Fade_in(object sender, int seconds)
        {
            TextBlock TextLabel = (TextBlock)sender;
            DoubleAnimation animation = new DoubleAnimation(1, TimeSpan.FromSeconds(seconds));
            TextLabel.BeginAnimation(UIElement.OpacityProperty, animation);
        }
    }
}
