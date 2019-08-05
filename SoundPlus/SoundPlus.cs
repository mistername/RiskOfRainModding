using BepInEx;
using RoR2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace SoundPlus
{
    [BepInDependency(R2API.R2API.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(modguid, modname, version)]
    public class SoundPlus : BaseUnityPlugin
    {
        public const string modguid = "com.mistername." + modname;
        internal const string modname = "SoundPlus";
        internal const string version = "0.1.0";

        public void Awake()
        {
            On.RoR2.RoR2Application.OnLoad += RoR2Application_OnLoad;
        }

        private void RoR2Application_OnLoad(On.RoR2.RoR2Application.orig_OnLoad orig, RoR2Application self)
        {
            CopySounds();


            AddingBank(self, FindBanks());

            orig(self);
        }

        private List<Tuple<string, bool>> FindBanks()
        {
            List<Tuple<string, bool>> banks = new List<Tuple<string, bool>>();
            var files = Directory.GetFiles(Paths.PluginPath, "*.sound", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (File.Exists(NameToBank(SoundToName(file))))
                {
                    banks.Add(new Tuple<string, bool>(file, false));
                }
                else
                {
                    banks.Add(new Tuple<string, bool>(file, true));
                }
            }
            return banks;
        }

        private static string NameToBank(string tmp)
        {
            return AkBasePathGetter.GetPlatformBasePath() + tmp + ".bnk";
        }

        private static string SoundToName(string file)
        {
            var tmp = file.Remove(file.Length - ".sound".Length);
            var tmparray = tmp.Split('\\');
            tmp = tmparray[tmparray.Length - 1];
            return tmp;
        }

        private static void AddingBank(RoR2Application self, List<Tuple<string, bool>> banks)
        {
            foreach (var bank in banks)
            {
                if (bank.Item2)
                {
                    File.Copy(bank.Item1, NameToBank(SoundToName(bank.Item1)));
                }

                var bankname = SoundToName(bank.Item1);

                var custombank = self.wwiseGlobalPrefab.AddComponent<AkBank>();
                custombank.data.ObjectReference = ScriptableObject.CreateInstance<WwiseBankReference>();
                custombank.decodeBank = true;
                custombank.saveDecodedBank = true;
                typeof(WwiseObjectReference).GetField("objectName", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(custombank.data.ObjectReference, bankname);
            }
        }

        private static void CopySounds()
        {
            var path = GetDirectoryPath(Assembly.GetExecutingAssembly());
            path += "/sounds";
            AkBasePathGetter.FixSlashes(ref path);
            Debug.Log(path);

            var copypath = AkBasePathGetter.GetFullSoundBankPath();

            DirectoryCopy(copypath, path, true);

            AkBasePathGetter.DefaultBasePath = path;
            ((AkWindowsSettings)AkWwiseInitializationSettings.ActivePlatformSettings).UserSettings.m_BasePath = path;
        }

        public static string GetDirectoryPath(Assembly assembly)
        {
            string filePath = new Uri(assembly.CodeBase).LocalPath;
            return System.IO.Path.GetDirectoryName(filePath);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {

            if (!Directory.Exists(sourceDirName))
            {
                Debug.LogError("sound folder does not exists");
                return;
            }

            if (Directory.Exists(destDirName))
            {
                Debug.Log("sounds already copied");
                return;
            }

            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = System.IO.Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = System.IO.Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
