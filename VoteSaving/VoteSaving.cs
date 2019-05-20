using BepInEx;
using BepInEx.Configuration;
using RoR2;

namespace VoteSaving
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.mistername." + modname, modname, version)]
    public class VoteSaving : BaseUnityPlugin
    {
        internal const string modname = "VoteSaving";
        internal const string version = "1.0.1";

        internal ConfigFile configfile = new ConfigFile(Paths.ConfigPath + "\\" + modname + ".cfg", true);

        public void Awake()
        {
            On.RoR2.PreGameRuleVoteController.GetDefaultChoice += PreGameRuleVoteController_GetDefaultChoice;
            On.RoR2.PreGameRuleVoteController.SetVote += PreGameRuleVoteController_SetVote;
        }

        public void OnDestroy()
        {
            On.RoR2.PreGameRuleVoteController.GetDefaultChoice -= PreGameRuleVoteController_GetDefaultChoice;
            On.RoR2.PreGameRuleVoteController.SetVote -= PreGameRuleVoteController_SetVote;
        }

        private void PreGameRuleVoteController_SetVote(On.RoR2.PreGameRuleVoteController.orig_SetVote orig, PreGameRuleVoteController self, int ruleIndex, int choiceValue)
        {
            var ruledef = RuleCatalog.GetRuleDef(ruleIndex);
            if (!ruledef.category.isHidden)
            {
                if (choiceValue >= 0)
                {
                    string catagorie = ruledef.category.displayToken;
                    string name = ruledef.globalName;
                    configfile.Wrap(catagorie, name, null, choiceValue).Value = choiceValue;
                }
            }

            orig(self, ruleIndex, choiceValue);
        }

        private RuleChoiceDef PreGameRuleVoteController_GetDefaultChoice(On.RoR2.PreGameRuleVoteController.orig_GetDefaultChoice orig, PreGameRuleVoteController self, RuleDef ruleDef)
        {
            int choice = PreGameController.instance.readOnlyRuleBook.GetRuleChoiceIndex(ruleDef);

            if (!ruleDef.category.isHidden)
            {
                string catagory = ruleDef.category.displayToken;
                string name = ruleDef.globalName;
                choice = configfile.Wrap(catagory, name, null, choice).Value;
            }

            return ruleDef.choices[choice];
        }
    }
}
