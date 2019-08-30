using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using VMulti_Installer;
using Version = VMulti_Installer.Version;

namespace VMulti_Installer_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Log("Operating System Architecture: " + Version);
        }

        public Version Version => Environment.Is64BitOperatingSystem ? Version.x64 : Version.x86;

        public void InstallVMulti(object sender, RoutedEventArgs e)
        {
            var installer = new Installer();
            installer.LogUpdated += LogEvent;

            RunBackground(() => installer.Install(Version));
        }

        public void UninstallVMulti(object sender, RoutedEventArgs e)
        {
            var installer = new Installer();
            installer.LogUpdated += LogEvent;

            RunBackground(() => installer.Uninstall(Version));
        }

        private void RunBackground(Action method)
        {
            using (var worker = new BackgroundWorker())
            {
                worker.DoWork += (s, a) => method();
                worker.RunWorkerCompleted += (s, a) => ButtonPanel.IsEnabled = true;

                ButtonPanel.IsEnabled = false;
                worker.RunWorkerAsync();
            }
        }

        private void LogEvent(object sender, string e) => Dispatcher.Invoke(() => Log(e));

        public void Log(string line)
        {
            if (!string.IsNullOrWhiteSpace(OutputTextbox.Text))
                OutputTextbox.Text += Environment.NewLine;
            OutputTextbox.Text += line;
            OutputTextbox.ScrollToEnd();
        }

        private void CopyAllConsole(object sender, RoutedEventArgs e) => Clipboard.SetText(OutputTextbox.Text);
        private void ClearAllConsole(object sender, RoutedEventArgs e) => OutputTextbox.Text = string.Empty;

        private void CheckVMulti(object sender, RoutedEventArgs e) => Log($"VMulti is {(Installer.Detect() ? null : "not ")}installed.");
    }
}
