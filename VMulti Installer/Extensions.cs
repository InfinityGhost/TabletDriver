using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace VMulti_Installer
{
    internal static class Extensions
    {
        public static void Extract(this FileInfo file, DirectoryInfo directory) => 
            ZipFile.ExtractToDirectory(file.FullName, directory.FullName);

        public static Process CreateProcess(this FileInfo file, string args, bool admin)
        {
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = file.FullName,
                    Arguments = args,
                    Verb = admin ? "runas" : null,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = file.DirectoryName,
                    UseShellExecute = false,
                },
            };
        }

        public static void WriteBytesToFile(this Stream stream, FileInfo file)
        {
            if (file.Exists)
                file.Delete();
            using (var outputStream = file.Create())
            {
                for (int i = 0; i < stream.Length; i++)
                    outputStream.WriteByte((byte)stream.ReadByte());
            }
        }
    }
}
