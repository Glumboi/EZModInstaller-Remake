using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EZModInstallerRemake.Files
{
    public static class ManageFiles
    {
        private static readonly FileZipper fileZipper = new FileZipper();

        public static void DeleteFilesContaining(string path, string contains, string ending)
        {
            string[] files = Directory.GetFiles(path, $"*.{ending}", SearchOption.TopDirectoryOnly);

            foreach (string file in files)
            {
                string text = File.ReadAllText(file);
                if (text.Contains(contains))
                {
                    File.Delete(file);
                }
            }
        }

        public static void MoveAssetFilesTo(string srcPath, string destPath)
        {
            if (!Directory.Exists(srcPath))
            {
                System.Windows.Forms.MessageBox.Show($"The 'Glumboi' Folder was not found in {destPath}, please make sure that the app created it!",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            //Get all .xml files
            string[] files = Directory.GetFiles(srcPath, "*.xml", SearchOption.AllDirectories);

            //Move all .xml files
            CopyFilesTo(files, destPath);

            //Get all .txt files
            files = Directory.GetFiles(srcPath, "*.txt", SearchOption.AllDirectories);

            //Move all .txt files
            CopyFilesTo(files, destPath);
        }

        private static void CopyFilesTo(string[] files, string destPath)
        {
            foreach (string file in files)
            {
                File.Copy(file, Path.Combine(destPath, Path.GetFileName(file)), true);
            }
        }

        public static void CreateSaveBackup(string pcbsPath)
        {
            KillPCBSTask();

            DialogResult dlg = System.Windows.Forms.MessageBox.Show("Do you want to backup your save files?", "info", MessageBoxButtons.YesNo,
            MessageBoxIcon.Information);

            if (dlg == DialogResult.Yes)
            {
                if (File.Exists(pcbsPath + @"\Saves.zip"))
                {
                    File.Delete(pcbsPath + @"\Saves.zip");
                }

                if (!Directory.Exists(pcbsPath + @"\Saves"))
                {
                    System.Windows.Forms.MessageBox.Show("Saves Directory does not exist, did you play the game yet?",
                        "Info",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                fileZipper.Zip(pcbsPath + @"\Saves", pcbsPath + @"\Saves.zip");

                System.Windows.Forms.MessageBox.Show("Saves backed up!", "info", MessageBoxButtons.OK,
                  MessageBoxIcon.Information);
            }
        }

        private static async void KillPCBSTask()
        {
            await Task.Run(() =>
            {
                //Checks if PCBS.exe is running and closes it
                if (Process.GetProcessesByName("PCBS").Length > 0)
                {
                    Process.GetProcessesByName("PCBS")[0].Kill();
                    System.Windows.Forms.MessageBox.Show("PCBS.exe got closed so the install can begin!", "info", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                }
            });
        }
    }
}