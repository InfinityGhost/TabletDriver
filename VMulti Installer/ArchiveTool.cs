using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VMulti_Installer
{
    internal class ArchiveTool : IDisposable
    {
        public ArchiveTool() { }

        private DirectoryInfo CurrentDir => new DirectoryInfo(Directory.GetCurrentDirectory());
        private List<FileInfo> TemporaryFiles { set; get; } = new List<FileInfo>();
        private DirectoryInfo TemporaryDirectory { set; get; }

        /// <summary>
        /// Extracts the archive resource to a temporary location.
        /// </summary>
        /// <param name="archiveName"></param>
        /// <returns>Directory in which the files are extracted to</returns>
        public DirectoryInfo ExtractArchive(string archiveName)
        {
            if (TemporaryDirectory == null)
                TemporaryDirectory = CurrentDir.CreateSubdirectory("temp");

            var archiveStream = GetArchiveResourceStream(archiveName);
            var archive = new FileInfo(Path.Combine(TemporaryDirectory.FullName, archiveName));
            archiveStream.WriteBytesToFile(archive);
            TemporaryFiles.Add(archive);

            archive.Extract(TemporaryDirectory);
            TemporaryFiles.AddRange(TemporaryDirectory.GetFiles());

            return TemporaryDirectory;
        }

        private Stream GetArchiveResourceStream(string archiveName) => 
            Assembly.GetExecutingAssembly().GetManifestResourceStream("VMulti_Installer.Archives." + archiveName);

        /// <summary>
        /// Removes all unmanaged files
        /// </summary>
        public void Dispose()
        {
            foreach (var file in TemporaryFiles)
                if (file.Exists)
                    file.Delete();
            TemporaryDirectory.Delete();

            TemporaryDirectory = null;
            TemporaryFiles.Clear();
        }
    }
}
