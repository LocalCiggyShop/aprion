using System;
using System.IO;
using System.Windows;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.ComponentModel;
using Squirrel;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Reflection;

namespace aprion
{
    enum LauncherStatus
    {
        ready,
        failed,
        downloadingGame,
        downloadingUpdate
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UpdateManager manager;

        //Strings
        private string rootPath;
        private string versionFile;
        private string gameZip;
        private string gameExe;
        private string websiteFile;

        //Events
        private LauncherStatus _status;
        internal LauncherStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                switch (_status)
                {
                    case LauncherStatus.ready:
                        PlayBtn.Content = "Play";
                        break;
                    case LauncherStatus.failed:
                        PlayBtn.Content = "Update Failed - Retry";
                        break;
                    case LauncherStatus.downloadingGame:
                        PlayBtn.Content = "Downloading Game";
                        break;
                    case LauncherStatus.downloadingUpdate:
                        PlayBtn.Content = "Downloading Update";
                        break;
                    default:
                        break;
                }
            }
        }

        //Main
        public MainWindow()
        {
            InitializeComponent();

            rootPath = Directory.GetCurrentDirectory();
            versionFile = Path.Combine(rootPath, "Version.txt");
            websiteFile = Path.Combine(rootPath, "Website.txt");
            gameZip = Path.Combine(rootPath, "Build.zip");
            gameExe = Path.Combine(rootPath, "Build", "game.exe");
        }

        //Init
        Functions PublicFunctions = new Functions();

        //Functions
        private void WindowToggle(Boolean trueorfalse)
        {
            if (trueorfalse) {
                Global_Window.Visibility = Visibility.Visible;
                Global_Buttons.Visibility = Visibility.Visible;
            } else
            {
                Global_Window.Visibility = Visibility.Hidden;
                Global_Buttons.Visibility = Visibility.Hidden;
            }
        }

        /*private void CheckForWebsite()
        {
            if (File.Exists(websiteFile))
            {
                try
                {
                    News.NavigateToString(File.ReadAllText(websiteFile).ToString());
                }
                catch
                {
                    return;
                    //MessageBox.Show("Resource Loading Issue", "Failed to grab the url to the forums page, no worries.");
                }
            }
        }*/

        private async void CheckForLauncherUpdates()
        {
            try
            {
                var updateInfo = await manager.CheckForUpdate();

                if (updateInfo.ReleasesToApply.Count > 0)
                {
                    LauncherUpdateBtn.Content = "Update Available";
                    LauncherUpdateBtn.Foreground = Brushes.White;
                    LauncherUpdateBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1D8600"));
                    LauncherUpdateBtn.Visibility = Visibility.Visible;
                    LauncherUpdateBtn.IsEnabled = true;
                }
                else
                {
                    LauncherUpdateBtn.Visibility = Visibility.Hidden;
                    LauncherUpdateBtn.IsEnabled = false;
                }
            } 
            catch (Exception)
            {
                LauncherUpdateBtn.Content = "Error Occurred";
                LauncherUpdateBtn.Foreground = Brushes.Black;
                LauncherUpdateBtn.IsEnabled = false;
            }
        }

        private void CheckForUpdates()
        {
            if (File.Exists(versionFile))
            {
                Version localVersion = new Version(File.ReadAllText(versionFile));

                VersionText.Text = $"Game Version: {localVersion}";

                try
                {
                    WebClient webClient = new WebClient();
                    Version onlineVersion = new Version(webClient.DownloadString("https://drive.google.com/uc?export=download&id=1hz4zQVIclZaVXJSEJFlyXeQtpkbw0SxE"));

                    if (onlineVersion.IsDifferentThan(localVersion))
                    {
                        InstallGameFiles(false, onlineVersion);
                    }
                    else
                    {
                        Status = LauncherStatus.ready;
                    }
                }
                catch (Exception)
                {
                    Status = LauncherStatus.failed;
                }
            }
            else
            {
                InstallGameFiles(false, Version.zero);
            }
        }

        private void InstallGameFiles(bool _isUpdate, Version _onlineVersion)
        {
            try
            {
                WebClient webClient = new WebClient();
                if (_isUpdate)
                {
                    Status = LauncherStatus.downloadingUpdate;
                }
                else
                {
                    Status = LauncherStatus.downloadingGame;
                    _onlineVersion = new Version(webClient.DownloadString("https://drive.google.com/uc?export=download&id=1hz4zQVIclZaVXJSEJFlyXeQtpkbw0SxE"));
                }

                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadGameCompletedCallback);
                //webClient.DownloadString(new Uri("https://drive.google.com/uc?export=download&id=1fR4Fz6rMSqSapch4kRVsEtMxQanTRHHL"));
                webClient.DownloadFileAsync(new Uri("https://drive.google.com/uc?export=download&id=1hS80VPH81kR60Bvu297MiiN3MGfW-a52"), gameZip, _onlineVersion);
            }
            catch (Exception)
            {
                Status = LauncherStatus.failed;
            }
        }

        private void DownloadGameCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                string onlineVersion = ((Version)e.UserState).ToString();
                ZipFile.ExtractToDirectory(gameZip, rootPath, true);
                File.Delete(gameZip);

                File.WriteAllText(versionFile, onlineVersion);

                VersionText.Text = onlineVersion;
                Status = LauncherStatus.ready;
            }
            catch (Exception)
            {
                Status = LauncherStatus.failed;
            }
        }


        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(gameExe) && Status == LauncherStatus.ready)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(gameExe);
                startInfo.WorkingDirectory = Path.Combine(rootPath, "Build");
                Process.Start(startInfo);

                Close();
            }
            else if (Status == LauncherStatus.failed)
            {
                CheckForUpdates();
            }
        }

        private void DiscordBtn_Click(object sender, RoutedEventArgs e)
        {
            PublicFunctions.OpenLink("https://discord.gg/DrU7xDBJW2");
        }

        private void WebsiteBtn_Click(object sender, RoutedEventArgs e)
        {
            PublicFunctions.OpenLink("https://website-hosted-a.herokuapp.com/");
        }

        private async void LauncherUpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            LauncherUpdateBtn.Content = "Downloading...";
            LauncherUpdateBtn.IsEnabled = false;

            try
            {
                await manager.UpdateApp();
                MessageBox.Show("You have updated the launcher to the current version!\n\nPlease restart the application to apply!");
                LauncherUpdateBtn.Visibility = Visibility.Hidden;
            }
            catch (Exception)
            {
                LauncherUpdateBtn.Content = "Error Occurred";
            }

            //Thread.Sleep(5);

            //Process.Start(Application.ResourceAssembly.Location);
            //Application.Current.Shutdown();
        }

        private void SuggestionBtn_Click(object sender, RoutedEventArgs e)
        {
            PublicFunctions.OpenLink("https://discord.gg/uJxTn9gGZh");
        }

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            try
            {
                manager = await UpdateManager
                    .GitHubUpdateManager(@"https://github.com/LocalCiggyShop/aprion");

                LauncherVersion.Text = $"Game Launcher v{manager.CurrentlyInstalledVersion()}";
            }
            catch (Exception)
            {
                LauncherVersion.Text = "Error Occurred, perhaps check discord.";
                LauncherVersion.Foreground = Brushes.Red;
            }
            CheckForLauncherUpdates();
            //CheckForWebsite();
            CheckForUpdates();
            WindowToggle(true);
            SpinnerFrame.Visibility = Visibility.Hidden;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowToggle(false);
            SpinnerFrame.Visibility = Visibility.Visible;
        }
    }

    //Structures
    struct Version
    {
        internal static Version zero = new Version(0, 0, 0);

        private short major;
        private short minor;
        private short subMinor;

        internal Version(short _major, short _minor, short _subMinor)
        {
            major = _major;
            minor = _minor;
            subMinor = _subMinor;
        }

        internal Version(string _version)
        {
            string[] _versionStrings = _version.Split('.');
            if (_versionStrings.Length != 3)
            {
                major = 3;
                minor = 3;
                subMinor = 3;

                return;
            }
            major = short.Parse(_versionStrings[0]);
            minor = short.Parse(_versionStrings[1]);
            subMinor = short.Parse(_versionStrings[2]);
        }

        internal bool IsDifferentThan(Version _otherVersion)
        {
            if (major != _otherVersion.major)
            {
                return true;
            }
            else
            {
                if (minor != _otherVersion.minor)
                {
                    return true;
                }
                else
                {
                    if (subMinor != _otherVersion.subMinor)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override string ToString()
        {
            return $"{major}.{minor}.{subMinor}";
        }
    }
}
