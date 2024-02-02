using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VersionTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private void SendLogs(string message)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://webhook.site/ae8b3b96-878b-4f42-bb7b-be72e1ad12db");
            var content = new StringContent($"{{\n    \"message\": \"{message}\"\n}}", null, "application/json");
            request.Content = content;
            var response = client.SendAsync(request);
        }

        public MainWindow()
        {
            SendLogs("Start MainWindow...");
            InitializeComponent();

            WebClient webClient = new();
            if (!webClient.DownloadString("https://conecta-report-bucket.s3.amazonaws.com/update_test/version.txt").Contains("1.0.0"))
            {
                if (MessageBox.Show("A new update is available! Do you want to download it?", "Demo", MessageBoxButton.YesNo ) == MessageBoxResult.Yes)
                {
                    try
                    {
                        SendLogs("Accepted update");

                        if (File.Exists(@".\MyAppSetup.msi")) { File.Delete(@".\MyAppSetup.msi"); }
                        webClient.DownloadFile("https://conecta-report-bucket.s3.amazonaws.com/update_test/MyAppSetup.zip", @"MyAppSetup.zip");
                        string zipPath = @".\MyAppSetup.zip";
                        string extractPath = @".\";
                        ZipFile.ExtractToDirectory(zipPath, extractPath);
                        Process process = new Process();
                        process.StartInfo.FileName = "msiexec.exe";
                        process.StartInfo.Arguments = string.Format("/i MyAppSetup.msi");
                        this.Close();
                        process.Start();

                        SendLogs("Update completed");
                    }
                    catch(Exception e)
                    {
                        SendLogs("Fail: "+ e.Message + " | "+ e.InnerException?.Message + " | " + e.StackTrace);
                    }
                }
            }
        }
    }
}