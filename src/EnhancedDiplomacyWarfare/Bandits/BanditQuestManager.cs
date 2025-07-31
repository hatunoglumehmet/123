using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.Core;
using EnhancedDiplomacyWarfare.Settings;

namespace EnhancedDiplomacyWarfare.Bandits
{
    public class BanditQuestManager
    {
        private static readonly List<string> QuestTypes = new List<string>
        {
            "eliminate_bandit_camp",
            "rescue_merchants",
            "destroy_stronghold",
            "hunt_bandit_leader",
            "recover_stolen_goods",
            "clear_trade_route",
            "protect_caravan",
            "infiltrate_network",
            "ransom_negotiations",
            "gather_information",
            "destroy_hideout",
            "escort_noble",
            "retrieve_artifacts",
            "stop_raids",
            "capture_bandit_chief",
            "disrupt_operations",
            "rescue_prisoners",
            "track_movement",
            "secure_passage",
            "eliminate_scouts"
        };

        private static readonly Dictionary<string, BanditQuestData> QuestTemplates = new Dictionary<string, BanditQuestData>
        {
            ["eliminate_bandit_camp"] = new BanditQuestData
            {
                Name = "Eliminate Bandit Camp",
                Description = "A bandit camp has been spotted near {SETTLEMENT}. Eliminate all bandits and destroy their camp.",
                BaseReward = 500,
                ExperienceReward = 100,
                RenownReward = 2,
                Difficulty = 1
            },
            ["rescue_merchants"] = new BanditQuestData
            {
                Name = "Rescue Captured Merchants",
                Description = "Merchants from {SETTLEMENT} have been captured by bandits. Rescue them and escort them to safety.",
                BaseReward = 750,
                ExperienceReward = 150,
                RenownReward = 3,
                Difficulty = 2
            },
            ["destroy_stronghold"] = new BanditQuestData
            {
                Name = "Destroy Bandit Stronghold",
                Description = "A powerful bandit stronghold threatens the region around {SETTLEMENT}. Assault and destroy it.",
                BaseReward = 1200,
                ExperienceReward = 250,
                RenownReward = 5,
                Difficulty = 3
            },
            ["hunt_bandit_leader"] = new BanditQuestData
            {
                Name = "Hunt the Bandit Leader",
                Description = "The notorious bandit leader {BANDIT_NAME} has been terrorizing the area. Hunt them down.",
                BaseReward = 1000,
                ExperienceReward = 200,
                RenownReward = 4,
                Difficulty = 3
            },
            ["recover_stolen_goods"] = new BanditQuestData
            {
                Name = "Recover Stolen Goods",
                Description = "Valuable goods worth {GOLD} denars were stolen from {SETTLEMENT}. Track down and recover them.",
                BaseReward = 600,
                ExperienceReward = 120,
                RenownReward = 2,
                Difficulty = 2
            }
        };

        public static List<BanditQuest> GenerateRandomQuests(int count)
        {
            var quests = new List<BanditQuest>();
            var random = new Random();
            
            for (int i = 0; i < count && i < QuestTypes.Count; i++)
            {
                var questType = QuestTypes[random.Next(QuestTypes.Count)];
                var settlement = Settlement.All.Where(s => s.IsTown || s.IsVillage).GetRandomElement();
                
                if (settlement != null && QuestTemplates.ContainsKey(questType))
                {
                    var template = QuestTemplates[questType];
                    var quest = new BanditQuest
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = questType,
                        Name = template.Name,
                        Description = template.Description.Replace("{SETTLEMENT}", settlement.Name.ToString()),
                        TargetSettlement = settlement,
                        BaseReward = template.BaseReward + random.Next(-100, 100),
                        ExperienceReward = template.ExperienceReward,
                        RenownReward = template.RenownReward,
                        Difficulty = template.Difficulty,
                        IsActive = false,
                        CreationDate = CampaignTime.Now
                    };
                    
                    quests.Add(quest);
                }
            }
            
            return quests;
        }

        public static void CompleteBanditQuest(BanditQuest quest)
        {
            if (quest == null) return;

            // Award rewards
            Hero.MainHero.ChangeHeroGold(quest.BaseReward);
            Hero.MainHero.AddSkillXp(DefaultSkills.Leadership, quest.ExperienceReward);
            Clan.PlayerClan.AddRenown(quest.RenownReward);

            // Update statistics
            Statistics.StatisticsManager.PlayerStats.BanditPartiesDefeated++;
            Statistics.StatisticsManager.PlayerStats.AddExperience(quest.ExperienceReward);

            // Show completion message
            string message = LocalizationManager.GetText("bandit_quest_completed", "Bandit quest completed: {0}")
                .Replace("{0}", quest.Name);
            InformationManager.DisplayMessage(new InformationMessage(message, Colors.Green));
        }
    }

    public class BanditQuestData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int BaseReward { get; set; }
        public int ExperienceReward { get; set; }
        public int RenownReward { get; set; }
        public int Difficulty { get; set; }
    }

    public class BanditQuest
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Settlement TargetSettlement { get; set; }
        public int BaseReward { get; set; }
        public int ExperienceReward { get; set; }
        public int RenownReward { get; set; }
        public int Difficulty { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompleted { get; set; }
        public CampaignTime CreationDate { get; set; }
        public CampaignTime CompletionDate { get; set; }
    }

    public static class EnhancedBanditDialogues
    {
        public static readonly Dictionary<string, List<string>> BanditDialogues = new Dictionary<string, List<string>>
        {
            ["surrender_demand"] = new List<string>
            {
                "Your purse or your life, traveler!",
                "Stand and deliver! Your gold belongs to us now!",
                "You've wandered into the wrong territory, friend.",
                "Hand over your valuables and you might live to see another day.",
                "This road has a toll, and we're here to collect it!"
            },
            ["combat_taunts"] = new List<string>
            {
                "You'll regret crossing us!",
                "No one escapes our band alive!",
                "Your head will make a fine trophy!",
                "We've killed better warriors than you!",
                "The crows will feast on your corpse!"
            },
            ["defeated_pleas"] = new List<string>
            {
                "Mercy! I yield to your superior skill!",
                "Please, spare me! I have children to feed!",
                "I surrender! Take whatever you want!",
                "You've bested us fairly. I submit!",
                "I'll never trouble these roads again, I swear!"
            },
            ["recruitment_offers"] = new List<string>
            {
                "You fight well. Join our band and share in the spoils!",
                "A warrior of your skill could go far with us.",
                "Why serve others when you could lead bandits to riches?",
                "The road is hard, but the rewards are great for those who dare.",
                "Cast off your chains of law and join the free life!"
            }
        };

        public static string GetRandomDialogue(string category)
        {
            if (!BanditDialogues.ContainsKey(category))
                return "...";

            var dialogues = BanditDialogues[category];
            return dialogues[new Random().Next(dialogues.Count)];
        }
    }
}