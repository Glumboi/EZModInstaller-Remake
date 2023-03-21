using System;
using System.Collections.Generic;
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
        private readonly string _unityPatcher = @"\unitypatcher";
        private bool _downloadDone = false;

        public bool DownloadDone => _downloadDone;

        public void InstallUnitypatcher(string pcbsfolder)
        {
            _pcbsfolder = pcbsfolder;

            string unityPatcherURL = "https://anonfiles.com/2bw9bcg1z7/unitypatcher_zip";

            //Check if already isntalled
            if (Directory.Exists(_pcbsfolder + _unityPatcher))
            {
                DialogResult dialogResult = MessageBox.Show("unitypatcher is already installed!\nDo you want to reinstall it?", "info", MessageBoxButtons.YesNo,
                  MessageBoxIcon.Information);

                if (dialogResult == DialogResult.Yes)
                {
                    Directory.Delete(pcbsfolder + _unityPatcher, true);
                    StartDownload(unityPatcherURL);
                }

                return;
            }
            StartDownload(unityPatcherURL);
        }

        private void StartDownload(string url)
        {
            _downloadDone = false;

            using (AnonFileWrapper afwAnonFileWrapper = new AnonFileWrapper())
            {
                string ddl =
                    afwAnonFileWrapper.
                    GetDirectDownloadLinkFromLink(url);

                using (WebClient wbc = new WebClient())
                {
                    wbc.DownloadFileCompleted += Wbc_DownloadFileCompleted;

                    //Set path of Download
                    _tempDlPath = _pcbsfolder + "TempDL.zip";

                    wbc.DownloadFileAsync(new Uri(ddl), _tempDlPath);
                }
            }
        }

        private void Wbc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            _fileZipper.Unzip(_tempDlPath, _pcbsfolder);
            _downloadDone = true;
        }
    }
}