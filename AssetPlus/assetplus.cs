using BepInEx;
using BepInEx.Configuration;
using System.IO;
using System.Text;
using AK.Wwise;

namespace AssetPlus
{
    /// <summary>
    /// Simple class for adding all the individual classes together, use for modguid
    /// </summary>
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(modguid, _modName, version)]
    public class AssetPlus : BaseUnityPlugin
    {
        /// <summary>
        /// Use this in as the dependency string
        /// </summary>
        public const string modguid = "com.mistername." + _modName;

        private const string _modName = nameof(AssetPlus);
        internal const string version = "6.6.6";

        internal void Awake()
        {
            UnityEngine.Debug.LogWarning("assetplus is deprecated and now included in r2api, please use that");
        }
    }
}
