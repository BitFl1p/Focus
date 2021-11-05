using System;
using System.Collections.Generic;
using System.IO;
using IWshRuntimeLibrary;
using static System.Console;
using System.Linq;

namespace focus
{

    class Program
    {
        static List<string> focusedSH = new List<string> { }, unfocusedS = new List<string> { };

        static void Main(string[] args)
        {
            foreach(string arg in args)
            {
                switch (arg)
                {
                    case "--help":
                        WriteLine(" --help        brings up this menu");
                        WriteLine(" --del         deletes your save file(this file includes all paths for the programs you set to appear in focus mode)");
                        WriteLine(" arguments can be used together for each of their separate effects");
                        break;
                    case "--del":
                        DelSave("FocusedPaths");
                        break;
                }
            }
            
            
            focusedSH = CheckSave("FocusedPaths");
            if (!focusedSH.Any())
            {
                MakeFocusedSave("FocusedPaths");
                focusedSH = CheckSave("FocusedPaths");
            }
            ClearDesktop();
            foreach(string sh in focusedSH)
            {
                string name = Path.GetFileName(sh);
                if(name.IndexOf('.')>= 0)name = name.Substring(0, name.IndexOf('.'));
                CreateShortcut(name, Environment.GetFolderPath(Environment.SpecialFolder.Desktop), sh);
            }
        }

        private static void DelSave(string saveName)
        {
            if (System.IO.File.Exists(Directory.GetCurrentDirectory() + "\\" + saveName + ".txt"))
            {
                System.IO.File.Delete(Directory.GetCurrentDirectory() + "\\" + saveName + ".txt");
                WriteLine("File deleted.");
            }
            else WriteLine("Save File not found");
        }

        static void MakeFocusedSave(string saveName)
        {
            using (StreamWriter sw = System.IO.File.CreateText(Directory.GetCurrentDirectory() + "\\" + saveName + ".txt"))
            {
                List<string> questions = new List<string>
                {
                    "Path for Unity Hub: ",
                    "Path for Blender: ",
                    "Path for Google Chrome: "
                };
                foreach (string question in questions)
                {
                    Write(question);
                    sw.WriteLine(ReadLine());
                }
                sw.Close();
            }
        }
        static void ClearDesktop()
        {
            string[] filePaths = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            foreach (string filePath in filePaths)
                System.IO.File.Delete(filePath);
        }
        static void MakeUnfocusedSave(string saveName)
        {

        }


        public static List<string> CheckSave(string saveName)
        {
            List<string> lines = new List<string> { };
            try
            {
                StreamReader sr = new StreamReader(Directory.GetCurrentDirectory()+"\\" + saveName + ".txt");
                string currentLine = "";
                while (currentLine != null)
                {
                    currentLine = sr.ReadLine();
                    if(currentLine != null) lines.Add(currentLine);
                } 
                sr.Close();
                return lines;
            }
            catch
            {
                return lines;
            }
            
        }

        public static void CreateShortcut(string shortcutName, string shortcutPath, string targetFileLocation)
        {
            string shortcutLocation = Path.Combine(shortcutPath, shortcutName + ".lnk");
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);
            shortcut.Description = "My shortcut description";   // The description of the shortcut
            shortcut.IconLocation = targetFileLocation;// The icon of the shortcut
            shortcut.TargetPath = targetFileLocation;                 // The path of the file that will launch when the shortcut is run
            shortcut.Save();                                    // Save the shortcut
        }
    }
}
