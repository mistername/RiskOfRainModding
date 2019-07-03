using BepInEx;
using BepInEx.Configuration;
using Facepunch.Steamworks;
using MonoMod.Cil;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModdedQuickplay
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class Main : BaseUnityPlugin
    {
        const string version = "1.1.3";
        const string modname = "ModdedQuickplay";

        const string quickplaynumber = "100";

        internal static ConfigFile file = new ConfigFile(Paths.ConfigPath + "\\" + modname + ".cfg", true);

        public void Awake()
        {
            On.RoR2.DisableIfGameModded.OnEnable += DisableIfGameModded_OnEnable;
            RoR2.SteamworksLobbyManager.onLobbyChanged += SteamworksLobbyManager_onLobbyChanged;
            On.RoR2.Networking.SteamLobbyFinder.CanJoinLobby += SteamLobbyFinder_CanJoinLobby;
            On.RoR2.RoR2Application.Update += RoR2Application_Update;
            //On.RoR2.SteamworksLobbyManager.JoinLobby += SteamworksLobbyManager_JoinLobby;
            RoR2. SteamworksLobbyManager.onLobbyJoined += SteamworksLobbyManager_onLobbyJoined;
            //SettingsPlusInit();
            //On.RoR2.RoR2Application.OnLoad += RoR2Application_OnLoad;
            IL.RoR2.Networking.SteamLobbyFinder.RequestLobbyListRefresh += SteamLobbyFinder_RequestLobbyListRefresh;
            IL.RoR2.SteamworksLobbyManager.SetLobbyQuickPlayQueuedIfOwner += SteamworksLobbyManager_SetLobbyQuickPlayQueuedIfOwner;
        }

        private void SteamworksLobbyManager_SetLobbyQuickPlayQueuedIfOwner(ILContext il)
        {
            var cursor = new ILCursor(il);

            cursor.GotoNext(
                x => x.MatchLdstr("1")
            );

            cursor.Next.Operand = quickplaynumber;

            //Debug.Log(il.ToString());
        }

        private void SteamLobbyFinder_RequestLobbyListRefresh(ILContext il)
        {
            var cursor = new ILCursor(il);

            cursor.GotoNext(
                x => x.MatchLdstr("appid")
                );
            cursor.GotoNext(
                x => x.MatchDup()
                );
            cursor.RemoveRange(5);

            cursor.GotoNext(
                x => x.MatchLdstr("qp"),
                x => x.MatchLdstr("1")
            );

            cursor.GotoNext(
                x => x.MatchLdstr("1")
                );

            cursor.Next.Operand = quickplaynumber;
        }

        private void RoR2Application_OnLoad(On.RoR2.RoR2Application.orig_OnLoad orig, RoR2.RoR2Application self)
        {
            BepInEx.Bootstrap.Chainloader.Plugins.ForEach(p => Debug.Log(MetadataHelper.GetMetadata(p).GUID)); 
            orig(self);
            Debug.Log(self.steamworksClient.BuildId.ToString());
        }

        private void SteamworksLobbyManager_onLobbyJoined(bool success)
        {
            if (success)
            {
                foreach (var data in Client.Instance.Lobby.CurrentLobbyData.GetAllData())
                {
                    if (data.Value.Contains("wants: "))
                    {
                        string GUID = data.Key.Substring(data.Key.IndexOf("wants: ") + "wants: ".Length);
                        bool hasmod = BepInEx.Bootstrap.Chainloader.Plugins.Any(p => MetadataHelper.GetMetadata(p).GUID == GUID);
                        Client.Instance.Lobby.SetMemberData(GUID, hasmod.ToString());
                    }
                }
            }
        }

        //private void SteamworksLobbyManager_JoinLobby(On.RoR2.SteamworksLobbyManager.orig_JoinLobby orig, CSteamID newLobbyId)
        //{
        //    orig(newLobbyId);
        //}

        private void RoR2Application_Update(On.RoR2.RoR2Application.orig_Update orig, RoR2.RoR2Application self)
        {
            if ((bool)typeof(BepInEx.Bootstrap.Chainloader).GetField("_loaded", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null))
            {
                file.Wrap("hosting", "!MOD GUID!", "0 means it isn't used, 1 means each clients needs to have it, -1 means each clients needs to not have it", "0");
                file.Wrap("joining", "!MOD GUID!", "0 means it isn't used, 1 means that the host needs to have it, -1 means that the host needs to not have it", "0");

                foreach (BaseUnityPlugin plugin in BepInEx.Bootstrap.Chainloader.Plugins)
                {
                    var pluginname = MetadataHelper.GetMetadata(plugin).GUID;
                    configPlugin(pluginname);
                }

                On.RoR2.RoR2Application.Update -= RoR2Application_Update;
            }
        }

        private static void configPlugin(string pluginname)
        {
            file.Wrap("hosting", pluginname, null, "0");
            file.Wrap("joining", pluginname, null, "0");
        }

        private bool SteamLobbyFinder_CanJoinLobby(On.RoR2.Networking.SteamLobbyFinder.orig_CanJoinLobby orig, int currentLobbySize, LobbyList.Lobby lobby)
        {
            var dataList = lobby.GetAllData();
            var configs = file.ConfigDefinitions;

#if DEBUG
            Debug.LogError(lobby.Name);
            foreach (var data in dataList)
            {
                Debug.LogWarning(data.Key + "   :   " + data.Value);
            }
#endif

            configP2Pplugin(dataList, configs);

            foreach (var assetmod in Modeffects.BothSideMod.HostClientMod)
            {
                if (lobby.GetData("has: " + assetmod) != "1")
                {
                    return false;
                }

                if (lobby.GetData("wants: " + assetmod) != "1")
                {
                    return false;
                }
            }

            foreach (var config in configs)
            {
                if (config.Section == "joining")
                {
                    var data = lobby.GetData("has: " + config.Key);
                    var value = file.Wrap<string>(config).Value;
                    if (value == "1")
                    {
                        if (data != "1")
                        {
#if DEBUG
                            Debug.LogError("no cuz host not have: " + config.Key);
#endif
                            return false;
                        }
                    }
                    else if (value == "-1")
                    {
                        if (data == "1")
                        {
#if DEBUG
                            Debug.LogError("no cuz host have: " + config.Key);
#endif
                            return false;
                        }
                    }
                }
            }

            foreach (var data in dataList)
            {
                if (data.Key.Contains("wants: "))
                {
                    string toBeSearched = "wants: ";
                    string GUID = data.Key.Substring(data.Key.IndexOf(toBeSearched) + toBeSearched.Length);
                    bool hasmod = BepInEx.Bootstrap.Chainloader.Plugins.Any(p => MetadataHelper.GetMetadata(p).GUID == GUID);
                    if (data.Value == "1")
                    {
                        if (!hasmod)
                        {
#if DEBUG
                            Debug.LogError("no cuz host want: " + GUID);
#endif
                            return false;
                        }
                    }
                    else if (data.Value == "-1")
                    {
                        if (hasmod)
                        {
#if DEBUG
                            Debug.LogError("no cuz host not want: " + GUID);
#endif
                            return false;
                        }
                    }
                }
            }

            string playercount = dataList["player_count"];
            int result = int.MinValue;
            if (playercount != null)
            {
                TextSerialization.TryParseInvariant(playercount, out result);
                if (result != int.MinValue)
                {
                    if (result + currentLobbySize <= lobby.MemberLimit)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static void configP2Pplugin(Dictionary<string, string> dataList, System.Collections.ObjectModel.ReadOnlyCollection<ConfigDefinition> configs)
        {
            foreach (var data in dataList)
            {
                if (data.Key.Contains("wants: "))
                {
                    string toBeSearched = "wants: ";
                    string GUID = data.Key.Substring(data.Key.IndexOf(toBeSearched) + toBeSearched.Length);
                    if (!configs.Any(p => p.Key == GUID))
                    {
                        configPlugin(GUID);
                    }
                }
                else if (data.Key.Contains("has: "))
                {
                    {
                        string toBeSearched = "has: ";
                        string GUID = data.Key.Substring(data.Key.IndexOf(toBeSearched) + toBeSearched.Length);
                        if (!configs.Any(p => p.Key == GUID))
                        {
                            configPlugin(GUID);
                        }
                    }
                }
            }
        }

        private void SteamworksLobbyManager_onLobbyChanged()
        {
            var client = Client.Instance;
            if (client?.Lobby?.CurrentLobbyData != null)
            {
                foreach (var item in file.ConfigDefinitions)
                {
                    if (item.Section == "hosting")
                    {
                        var value = file.Wrap<string>(item).Value;
                        client.Lobby.CurrentLobbyData.SetData("wants: " + item.Key, value);
                    }
                }

                foreach (BaseUnityPlugin plugin in BepInEx.Bootstrap.Chainloader.Plugins)
                {
                    var pluginname = MetadataHelper.GetMetadata(plugin).GUID;
                    client.Lobby.CurrentLobbyData.SetData("has: " + pluginname, "1");
                }


                foreach (var assetmod in Modeffects.BothSideMod.HostClientMod)
                {
                    client.Lobby.CurrentLobbyData.SetData("has: " + assetmod, "1");
                    client.Lobby.CurrentLobbyData.SetData("wants: " + assetmod, "1");
                }
            }
        }

        private void DisableIfGameModded_OnEnable(On.RoR2.DisableIfGameModded.orig_OnEnable orig, RoR2.DisableIfGameModded self)
        {
            Debug.Log("check");
            if (RoR2.RoR2Application.isModded == true && RoR2.RoR2Application.GetBuildId() == "MOD" && self.name == "Button, QP")
            {
                self.gameObject.AddComponent<check>();
                return;
            }
            else
            {
                self.gameObject.SetActive(false);
            }
        }
    }

    internal class check : MonoBehaviour
    {
        public void Update()
        {
            if (RoR2.RoR2Application.GetBuildId() != "MOD")
            {
                gameObject.SetActive(false);
                base.gameObject.SetActive(false);
            }
        }
    }

    public static class Modeffects
    {
        public static class BothSideMod
        {
            internal static List<string> HostClientMod = new List<string>();

            /// <summary>
            /// Forces quickplay users to have the same assetmod
            /// </summary>
            /// <param name="GUID"></param>
            public static void Add(string GUID)
            {
                HostClientMod.Add(GUID);
            }
        }
    }
}
