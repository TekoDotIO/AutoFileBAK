using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Threading;

namespace AutoFileBAK
{
    internal class FileSystemReflector
    {
        public static void CheckForImage(string Path, string ToPath)
        {
            try
            {
                Directory.CreateDirectory(ToPath);
                //Disk = "F:\";
                Path.Replace('\\', '/');
                string[] AllPaths = Directory.GetDirectories(Path);
                for (int i = 0; i < AllPaths.Length; i++)
                {
                    AllPaths[i] = AllPaths[i].Split("\\")[^1].Split("/")[^1];
                }
                string[] ToPaths = Directory.GetDirectories(ToPath);
                for (int i = 0; i < ToPaths.Length; i++)
                {
                    ToPaths[i] = ToPaths[i].Split("\\")[^1].Split("/")[^1];
                }
                foreach (string FirstPath in AllPaths)
                {
                    if (!((IList)ToPaths).Contains(FirstPath))
                    {
                        Directory.CreateDirectory(ToPath + "/" + FirstPath);
                        Log.SaveLog("Created \"" + ToPath + "/" + FirstPath + "\"");
                        CheckForImage(Path + "/" + FirstPath, ToPath + "/" + FirstPath);
                    }
                    else
                    {
                        Log.SaveLog("Path \"" + FirstPath + "\" is exists.");
                        CheckForImage(Path + "/" + FirstPath, ToPath + "/" + FirstPath);
                    }
                }
                //Part of folders.
                string[] AllFiles = Directory.GetFiles(Path);
                for (int i = 0; i < AllFiles.Length; i++)
                {
                    AllFiles[i] = AllFiles[i].Split("\\")[^1].Split("/")[^1];
                }
                string[] OldFiles = Directory.GetFiles(ToPath);
                for (int i = 0; i < OldFiles.Length; i++)
                {
                    OldFiles[i] = OldFiles[i].Split("\\")[^1].Split("/")[^1];
                }
                foreach (string NowFile in AllFiles)
                {
                    if (!((IList)OldFiles).Contains(NowFile))
                    {
                        File.Copy(Path + "/" + NowFile, ToPath + "/" + NowFile);
                        Log.SaveLog("File \"" + NowFile + "\" copied.");
                    }
                    else
                    {
                        var NowFileTime = File.GetLastWriteTime(Path + "/" + NowFile);
                        var OldFileTime = File.GetLastWriteTime(ToPath + "/" + NowFile);
                        if (NowFileTime == OldFileTime)
                        {
                            Log.SaveLog("File \"" + NowFile + "\" is exists and it is same as local file.");
                        }
                        else
                        {
                            File.Copy(Path + "/" + NowFile, ToPath + "/" + NowFile, true);
                            Log.SaveLog("File \"" + NowFile + "\" updated.");
                        }
                    }
                }
                //Part of files.
            }
            catch(Exception ex)
            {
                Log.SaveLog("Exception :" + ex.ToString());
                Thread.Sleep(10000);
                CheckForImage(Path, ToPath);
            }
        }
    }
}
