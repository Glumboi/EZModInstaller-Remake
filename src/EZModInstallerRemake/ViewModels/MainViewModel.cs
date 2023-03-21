using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using EZModInstallerRemake.BatchFileMerger;
using EZModInstallerRemake.Files;
using EZModInstallerRemake.UnityPatcher;
using Wpf.Ui.Controls;

namespace EZModInstallerRemake.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly UnityPatcherInstaller patcherInstaller = new UnityPatcherInstaller();

        private bool _debugInstall = Properties.Settings.Default.DebugInstall;

        public bool DebugInstall
        {
            get => _debugInstall;
            set
            {
                if (value != _debugInstall)
                {
                    SetProperty(ref _debugInstall, value);
                    Properties.Settings.Default.DebugInstall = value;
                }
            }
        }

        private string _pcbsPath = Properties.Settings.Default.PCBSPath;

        public string PCBSPath
        {
            get => _pcbsPath;
            set
            {
                if (value != _pcbsPath)
                {
                    SetProperty(ref _pcbsPath, value);
                    Properties.Settings.Default.PCBSPath = value;
                    CreateGlumboiFolder();
                }
            }
        }

        private string _pcbsPathPlaceholder = "PCBS Path";

        public string PCBSPathPlaceholder
        {
            get => _pcbsPathPlaceholder;
            set
            {
                if (value != _pcbsPathPlaceholder)
                {
                    SetProperty(ref _pcbsPathPlaceholder, value);
                }
            }
        }

        private string _snackContent = Properties.Settings.Default.PCBSPath;

        public string SnackContent
        {
            get => _snackContent;
            set
            {
                if (value != _snackContent)
                {
                    SetProperty(ref _snackContent, value);
                }
            }
        }

        private int _snackBarTimeOut = 4000;

        public int SnackBarTimeOut
        {
            get => _snackBarTimeOut;
            set
            {
                if (value != _snackBarTimeOut)
                {
                    SetProperty(ref _snackBarTimeOut, value);
                }
            }
        }

        private Wpf.Ui.Common.SymbolRegular _snackBarIcon;

        public Wpf.Ui.Common.SymbolRegular SnackBarIcon
        {
            get => _snackBarIcon;
            set
            {
                if (value != _snackBarIcon)
                {
                    SetProperty(ref _snackBarIcon, value);
                }
            }
        }

        public void CheckForRequirements()
        {
            var unitypatcherValid = CheckUnityPatcher();
            var pcbsValid = CheckForCorrectFolder();

            CreateGlumboiFolder();

            if (string.IsNullOrWhiteSpace(PCBSPath))
            {
                ShowSnackBar("Please select the PCBS Folder", Wpf.Ui.Common.SymbolRegular.Info28);
                return;
            }

            if (!pcbsValid)
            {
                ShowSnackBar("PCBS Folder doesnt contain PCBS related Files!", Wpf.Ui.Common.SymbolRegular.ErrorCircle24);
                return;
            }

            if (!unitypatcherValid && pcbsValid)
            {
                ShowSnackBar("UnityPatcher is not installed!", Wpf.Ui.Common.SymbolRegular.ErrorCircle24);
                return;
            }
        }

        private void CreateGlumboiFolder()
        {
            string glumboiFolder = PCBSPath + @"\Glumboi";

            if (!Directory.Exists(glumboiFolder) && CheckForCorrectFolder())
            {
                Directory.CreateDirectory(glumboiFolder);
            }
        }

        private bool CheckForCorrectFolder()
        {
            if (!File.Exists(PCBSPath + @"\PCBS.exe") && PCBSPath != string.Empty)
            {
                return false;
            }
            return true;
        }

        private bool CheckUnityPatcher()
        {
            if (Directory.Exists(PCBSPath + "\\unitypatcher"))
            {
                return true;
            }
            return false;
        }

        public void OpenPCBS()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    PCBSPath = fbd.SelectedPath;
                }
            }
        }

        public async void InstallUnityPatcher()
        {
            await Task.Run(() =>
            {
                patcherInstaller.InstallUnitypatcher(PCBSPath);

                while (!patcherInstaller.DownloadDone)
                {
                    Task.Delay(50);
                }
            });

            ShowSnackBar("Successfully installed UnityPatcher!", Wpf.Ui.Common.SymbolRegular.Checkmark28);
        }

        private Snackbar _snackBar;

        public Snackbar SnackBar
        {
            get => _snackBar;
            set
            {
                if (value != _snackBar)
                {
                    _snackBar = value;
                }
            }
        }

        public void ShowSnackBar(string content, Wpf.Ui.Common.SymbolRegular icon)
        {
            if (this.SnackBar == null) return;

            if (!this.SnackBar.IsShown)
            {
                SnackContent = content;
                SnackBarIcon = icon;

                this.SnackBar.Show();
            }
        }

        public void MergeAndExecuteModInstallers()
        {
            BackupSaves();

            if (!CheckUnityPatcher())
            {
                ShowSnackBar("UnityPatcher is not installed!\nCancelling the Operation..",
                    Wpf.Ui.Common.SymbolRegular.ErrorCircle24);

                return;
            }

            ManageFiles.MoveAssetFilesTo(PCBSPath + @"\Glumboi", PCBSPath);
            Merger.MergeBatFilesFromPath(DebugInstall, PCBSPath);

            string[] fileContentToCheck = new string[]
            {
                "</table>",
                "unitypatcher"
            };

            foreach (string content in fileContentToCheck)
            {
                ManageFiles.DeleteFilesContaining(PCBSPath, content, "txt");
                ManageFiles.DeleteFilesContaining(PCBSPath, content, "xml");
            }
        }

        public void BackupSaves()
        {
            ManageFiles.CreateSaveBackup(PCBSPath);
        }

        public void ResetSaves()
        {
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Are you sure?\nThis will delete ALL current saves!", "info",
            MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            bool dirExists = Directory.Exists(PCBSPath + @"\Saves");

            if (dialogResult == DialogResult.Yes && dirExists)
            {
                //Deletes the folders and files of the save folder of PCBS
                Directory.Delete(PCBSPath + @"\Saves", true);
                Directory.CreateDirectory(PCBSPath + @"\Saves");
                return;
            }

            if (dialogResult != DialogResult.Yes && !dirExists)
            {
                ShowSnackBar("Could not find the Saves Directory!\nMake sure you played the game once!", Wpf.Ui.Common.SymbolRegular.ErrorCircle24);
            }
        }

        public void RestoreSaves()
        {
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Do you want to restore your saves?", "info", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            FileZipper fileZipper = new FileZipper();

            bool fileExists = File.Exists(PCBSPath + @"\Saves.zip");

            if (dialogResult == DialogResult.Yes && fileExists)
            {
                fileZipper.Unzip(PCBSPath + @"\Saves.zip", PCBSPath + @"\Saves");
                System.Windows.Forms.MessageBox.Show("Done restoring saves!", "info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (dialogResult != DialogResult.Yes && !fileExists)
            {
                ShowSnackBar("Could not find the Saves Backup!\nMake sure you didn't rename or delete the Backup if you created one!",
                    Wpf.Ui.Common.SymbolRegular.ErrorCircle24);
            }
        }
    }
}