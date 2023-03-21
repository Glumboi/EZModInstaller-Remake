using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Windows.Shapes;

namespace EZModInstallerRemake.BatchFileMerger
{
    public static class Merger
    {
        public static void MergeBatFilesFromPath(bool usePause, string pcbsPath)
        {
            string mergedName = @"\merged.bat";

            if (File.Exists(pcbsPath + mergedName))
            {
                File.Delete(pcbsPath + mergedName);
            }

            if (string.IsNullOrWhiteSpace(pcbsPath))
            {
                MessageBox.Show("Please select the PCBS folder first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string[] batFiles = Directory.GetFiles(pcbsPath + @"\Glumboi", "*.bat", SearchOption.AllDirectories);
            string mergedBat = "";

            if (batFiles == null || batFiles.Length == 0)
            {
                MessageBox.Show($"No Mods have been found in the Glumboi Folder!",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

                return;
            }

            foreach (string bat in batFiles)
            {
                string[] lines = File.ReadAllLines(bat);
                foreach (string line in lines)
                {
                    if (line.Contains("pause") || line.Contains("@echo off"))
                    {
                        continue;
                    }
                    else
                    {
                        mergedBat += line + Environment.NewLine;
                    }
                }
            }

            if (usePause)
            {
                mergedBat += "@echo off" + Environment.NewLine + "pause";
            }

            File.WriteAllText(pcbsPath + mergedName, mergedBat);
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = pcbsPath + mergedName;
            psi.WorkingDirectory = pcbsPath;
            psi.UseShellExecute = true;
            Process p = Process.Start(psi);
            p.WaitForExit();

            File.Delete(pcbsPath + mergedName);
        }
    }
}