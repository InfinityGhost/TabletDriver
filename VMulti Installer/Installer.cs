using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace VMulti_Installer
{
    public partial class Installer
    {
        #region Public Methods

        public void Install(Version ver)
        {
            switch (ver)
            {
                case Version.x86:
                    // Install x86 (32-bit) VMulti
                    Install(archiveName: "driver_vmulti32.zip");
                    break;
                case Version.x64:
                    // Install x64 (64-bit) VMulti
                    Install(archiveName: "driver_vmulti64.zip");
                    break;
                default:
                    throw new ArgumentException("Invalid version");
            }
        }

        public void Uninstall(Version ver)
        {
            switch (ver)
            {
                // Uninstall x86 (32-bit) VMulti
                case Version.x86:
                    Uninstall(archiveName: "driver_vmulti32.zip");
                    break;
                // Uninstall x64 (64-bit) VMulti
                case Version.x64:
                    Uninstall(archiveName: "driver_vmulti64.zip");
                    break;
                default:
                    throw new ArgumentException("Invalid version");
            }
        }

        public static bool Detect()
        {
            var drivers = new DirectoryInfo(Path.Combine(Environment.SystemDirectory, @"DriverStore\FileRepository\"));
            var dirs = drivers.GetDirectories();
            return dirs.Any(f => f.Name.Contains("vmulti.inf"));
        }

        #endregion

        #region Private Methods

        private void Install(string archiveName)
        {
            // Archive tool for managing files
            var arcTool = new ArchiveTool();
            DirectoryInfo folder = arcTool.ExtractArchive(archiveName);
            IList<FileInfo> files = folder.GetFiles();

            // Get devcon process
            var devcon = files.First(f => f.Name == "devcon.exe");
            var process = devcon.CreateProcess(args: @"/r install vmulti.inf pentablet\hid", admin: true);

            // Consume output events
            process.OutputDataReceived += DataReceived;
            process.ErrorDataReceived += DataReceived;

            // Install VMulti driver
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            // Dispose unmanaged objects
            process.Dispose();
            arcTool.Dispose();
        }

        private void Uninstall(string archiveName)
        {
            // Archive tool for managing files
            var arcTool = new ArchiveTool();
            DirectoryInfo folder = arcTool.ExtractArchive(archiveName);
            IList<FileInfo> files = folder.GetFiles();

            // Get DIFxCmd process
            var DIFxCmd = files.First(f => f.Name == "DIFxCmd.exe");
            var DIFProcess = DIFxCmd.CreateProcess(args: @"/u vmulti.inf", admin: true);

            // Get devcon process
            var devcon = files.First(f => f.Name == "devcon.exe");
            var devconProcess = devcon.CreateProcess(args: @"remove pentablet\hid", admin: true);

            // Consume output events
            DIFProcess.OutputDataReceived += DataReceived;
            DIFProcess.ErrorDataReceived += DataReceived;
            devconProcess.OutputDataReceived += DataReceived;
            devconProcess.ErrorDataReceived += DataReceived;

            // Uninstall VMulti driver
            DIFProcess.Start();
            DIFProcess.BeginOutputReadLine();
            DIFProcess.BeginErrorReadLine();
            DIFProcess.WaitForExit();

            devconProcess.Start();
            devconProcess.BeginOutputReadLine();
            devconProcess.BeginErrorReadLine();
            devconProcess.WaitForExit();

            // Dispose unmanaged objects
            DIFProcess.Dispose();
            devconProcess.Dispose();
            arcTool.Dispose();
        }

        #endregion

        #region Logging

        public IList<string> Logs { protected set; get; } = new List<string>();
        public event EventHandler<string> LogUpdated;

        protected void Log(string text)
        {
            Logs.Add(text);
            LogUpdated?.Invoke(this, text);
        }

        protected void DataReceived(object sender, DataReceivedEventArgs e) => Log(e.Data);

        #endregion
    }
}
