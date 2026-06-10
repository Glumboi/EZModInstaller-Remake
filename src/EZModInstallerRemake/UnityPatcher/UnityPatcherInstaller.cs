using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EZModInstallerRemake.Files;
using System.Windows.Forms;
using AnonFileAPI;
using System.Net;

namespace EZModInstallerRemake.UnityPatcher
{
    public class UnityPatcherInstaller
    {
        private readonly FileZipper _fileZipper = new FileZipper();
        private string _pcbsfolder;
        private string _tempDlPath;
        private string _installPath; 
        private const string  UNITYPATCHER_FOLDER = "\\unitypatcher";
        private bool _downloadDone = false;
        private const string UNITYPATCHER_DONWLOAD_URL = "https://github.com/Glumboi/unitypatcher/releases/download/0.0.5/unitypatcher.zip";
        
        public bool DownloadDone => _downloadDone;

        public void InstallUnitypatcher(string pcbsfolder)
        {
            _pcbsfolder = pcbsfolder;
            _installPath =$"{_pcbsfolder}{UNITYPATCHER_FOLDER}";
            
            //Check if already isntalled
            if (Directory.Exists(_installPath))
            {
                DialogResult dialogResult = MessageBox.Show("unitypatcher is already installed!\nDo you want to reinstall it?", "info", MessageBoxButtons.YesNo,
                  MessageBoxIcon.Information);

                if (dialogResult == DialogResult.Yes)
                {
                    Directory.Delete(_installPath, true);
                    StartDownload(UNITYPATCHER_DONWLOAD_URL);
                }

                return;
            }
            StartDownload(UNITYPATCHER_DONWLOAD_URL);
        }

        
        
        private void StartDownload(string url)
        {
            _downloadDone = false;
            using (WebClient wbc = new WebClient())
            {
                wbc.DownloadFileCompleted += Wbc_DownloadFileCompleted;
                wbc.DownloadProgressChanged += (sender, args) =>
                {
                    Debug.WriteLine($"Downloading: {args.ProgressPercentage}%");
                };
                _tempDlPath = _pcbsfolder + "unitypatcher.temp.zip";
                wbc.DownloadFileAsync(new Uri(url), _tempDlPath);
            }
        }

        private void Wbc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if(!Directory.Exists(_installPath)) Directory.CreateDirectory(_installPath);
            _fileZipper.Unzip(_tempDlPath, _installPath);
            _downloadDone = true;
        }
    }
}