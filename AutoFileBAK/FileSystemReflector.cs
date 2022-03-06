using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace AutoFileBAK
{
    internal class FileSystemReflector
    {
        public static void CheckForImage(string Path, string ToPath)
        {
            //Disk = "F:\";
            Path.Replace('\\', '/');
            string[] AllPaths = Directory.GetDirectories(Path);
            for (int i = 0; i < AllPaths.Length; i++) 
            {
                AllPaths[i] = AllPaths[i].Split("\\")[^1].Split("/")[^1];
                Console.WriteLine(AllPaths[i]);
            }
            string[] ToPaths = Directory.GetDirectories(ToPath);
            for (int i = 0; i < ToPaths.Length; i++)
            {
                ToPaths[i] = ToPaths[i].Split("\\")[^1].Split("/")[^1];
                Console.WriteLine(ToPaths[i]);
            }
            foreach (string FirstPath in AllPaths)
            {
                if (!((IList)ToPaths).Contains(FirstPath))
                {
                    Directory.CreateDirectory(ToPath + "/" + FirstPath);
                    Log.SaveLog("Created " + ToPath + "/" + FirstPath);
                    CheckForImage(Path + "/" + FirstPath, ToPath + "/" + FirstPath);
                }
            }
        }
    }
}
