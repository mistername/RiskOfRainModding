using BepInEx;
using System.IO;

namespace AssetPlus
{

    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(modguid, modname, version)]
    public partial class AssetPlus : BaseUnityPlugin
    {
        /// <summary>
        /// Use this in as the dependency string
        /// </summary>
        public const string modguid = "com.mistername." + modname;

        internal const string modname = nameof(AssetPlus);
        internal const string version = "0.1.0";

        internal void Awake()
        {
            SoundAwake();
            FontAwake();
            LanguageAwake();
        }

        private static string[] LoadFiles(string searchFor)
        {
            return Directory.GetFiles(Paths.PluginPath, searchFor, SearchOption.AllDirectories);
        }
    }
}
