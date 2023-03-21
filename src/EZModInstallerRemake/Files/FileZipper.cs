using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace EZModInstallerRemake.Files
{
    internal class FileZipper
    {
        //Zips a folder in a zip file
        public void Zip(string sourceFolder, string destinationFile)
        {
            ZipFile.CreateFromDirectory(sourceFolder, destinationFile);
        }

        //Unzips a zip file in a folder
        public void Unzip(string sourceFile, string destinationFolder)
        {
            Thread t1 = new Thread(() =>
            {
                ZipFile.ExtractToDirectory(sourceFile, destinationFolder);
            });

            t1.Start();
        }
    }
}