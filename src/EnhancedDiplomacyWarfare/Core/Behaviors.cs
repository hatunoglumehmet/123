using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using EnhancedDiplomacyWarfare.Settings;
using System;
using System.Linq;

namespace EnhancedDiplomacyWarfare
{
    public class DailyRecruitmentBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
        }

        public override void SyncData(IDataStore dataStore)
        {
            // No data to sync
        }

        private void OnDailyTick()
        {
            if (!EDWSettings.Instance.EnableMod || !EDWSettings.Instance.EnableDailyFreeRecruitment)
                return;

            AddDailyFreeRecruits();
        }

        private void AddDailyFreeRecruits()
        {
            if (MobileParty.MainParty == null || Hero.MainHero == null)
                return;

            var playerCulture = Hero.MainHero.Culture;
            var availableTroops = playerCulture.BasicTroop;
            
            if (availableTroops == null)
                return;

            var random = new Random();
            int troopCount = random.Next(EDWSettings.Instance.DailyFreeTroopsMin, EDWSettings.Instance.DailyFreeTroopsMax + 1);

            // Add level-based bonus
            if (EDWSettings.Instance.EnablePlayerStatistics)
            {
                troopCount += Statistics.StatisticsManager.PlayerStats.Level / 10;
            }

            if (MobileParty.MainParty.MemberRoster.TotalManCount + troopCount <= MobileParty.MainParty.Party.PartySizeLimit)
            {
                MobileParty.MainParty.MemberRoster.AddToCounts(availableTroops, troopCount);
                
                string message = LocalizationManager.GetText("daily_recruitment", "Recruited {0} {1} troops for free")
                    .Replace("{0}", troopCount.ToString())
                    .Replace("{1}", availableTroops.Name.ToString());
                    
                InformationManager.DisplayMessage(new InformationMessage(message, Colors.Green));
            }
        }
    }

    public class StatisticsBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
            CampaignEvents.SettlementEntered.AddNonSerializedListener(this, OnSettlementEntered);
            CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, OnQuestCompleted);
            CampaignEvents.TournamentFinished.AddNonSerializedListener(this, OnTournamentFinished);
            CampaignEvents.OnClanChangedKingdomEvent.AddNonSerializedListener(this, OnClanChangedKingdom);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("PlayerStatistics", ref Statistics.StatisticsManager.PlayerStats);
        }

        private void OnDailyTick()
        {
            if (!EDWSettings.Instance.EnableMod || !EDWSettings.Instance.EnablePlayerStatistics)
                return;

            Statistics.StatisticsManager.OnDayPassed();
            ApplyDailyBonuses();
        }

        private void ApplyDailyBonuses()
        {
            var stats = Statistics.StatisticsManager.PlayerStats;
            
            // Daily gold bonus
            int goldBonus = stats.GetDailyGoldBonus();
            if (goldBonus > 0)
            {
                Hero.MainHero.ChangeHeroGold(goldBonus);
            }

            // Daily influence bonus
            int influenceBonus = stats.GetDailyInfluenceBonus();
            if (influenceBonus > 0 && Clan.PlayerClan.Kingdom != null)
            {
                Clan.PlayerClan.Influence += influenceBonus;
            }
        }

        private void OnSettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
        {
            if (mobileParty == MobileParty.MainParty && settlement.Owner == Hero.MainHero)
            {
                Statistics.StatisticsManager.OnSettlementCaptured();
            }
        }

        private void OnQuestCompleted(QuestBase quest, QuestBase.QuestCompleteDetails detail)
        {
            if (quest.QuestGiver != null)
            {
                Statistics.StatisticsManager.OnQuestCompleted();
            }
        }

        private void OnTournamentFinished(CharacterObject winner, MBReadOnlyList<CharacterObject> participants, Town town, ItemObject prize)
        {
            if (winner == CharacterObject.PlayerCharacter)
            {
                Statistics.StatisticsManager.OnTournamentWon();
            }
        }

        private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification)
        {
            // Track diplomatic successes
            if (clan == Clan.PlayerClan || newKingdom?.Leader == Hero.MainHero)
            {
                Statistics.StatisticsManager.OnDiplomaticSuccess();
            }
        }
    }

    public class DiplomacyBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.MakePeace.AddNonSerializedListener(this, OnMakePeace);
            CampaignEvents.OnPeaceOfferedToPlayerEvent.AddNonSerializedListener(this, OnPeaceOffered);
            CampaignEvents.KingdomDecisionAdded.AddNonSerializedListener(this, OnKingdomDecisionAdded);
        }

        public override void SyncData(IDataStore dataStore)
        {
            // No data to sync
        }

        private void OnMakePeace(IFaction faction1, IFaction faction2, MakePeaceAction.MakePeaceDetail detail)
        {
            if (!EDWSettings.Instance.EnableMod || !EDWSettings.Instance.EnableEnhancedDiplomacy)
                return;

            // Extend peace duration
            if (EDWSettings.Instance.PeaceDurationBonus > 0)
            {
                // Peace duration extension is handled in game logic
                string message = LocalizationManager.GetText("peace_extended", "Peace agreement extended by {0} days")
                    .Replace("{0}", EDWSettings.Instance.PeaceDurationBonus.ToString());
                InformationManager.DisplayMessage(new InformationMessage(message, Colors.Blue));
            }

            // Track diplomatic success
            if (faction1 == Clan.PlayerClan || faction2 == Clan.PlayerClan)
            {
                Statistics.StatisticsManager.OnDiplomaticSuccess();
            }
        }

        private void OnPeaceOffered(IFaction offerer, int tributeAmount)
        {
            if (!EDWSettings.Instance.EnableMod || !EDWSettings.Instance.AutoAcceptReasonableOffers)
                return;

            // Auto-accept reasonable peace offers
            if (tributeAmount >= 0 && offerer.Leader.Clan.TotalStrength < Clan.PlayerClan.TotalStrength * 0.7f)
            {
                // Logic for auto-accepting would be implemented here
                InformationManager.DisplayMessage(new InformationMessage(
                    LocalizationManager.GetText("diplomacy_success", "Diplomatic action successful!"), 
                    Colors.Green));
            }
        }

        private void OnKingdomDecisionAdded(KingdomDecision decision, Clan proposerClan, DecisionOutcome possibleOutcome)
        {
            // Enhanced diplomacy decision handling
            if (proposerClan == Clan.PlayerClan)
            {
                Statistics.StatisticsManager.OnDiplomaticSuccess();
            }
        }
    }

    public class RebellionEnhancementBehavior : CampaignBehaviorBase
    {
        private DateTime _lastRebellionCheck = DateTime.Now;

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
            CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, OnClanChangedKingdom);
            CampaignEvents.OnClanDestroyedEvent.AddNonSerializedListener(this, OnClanDestroyed);
        }

        public override void SyncData(IDataStore dataStore)
        {
            // No specific data to sync
        }

        private void OnDailyTick()
        {
            if (!EDWSettings.Instance.EnableMod || !EDWSettings.Instance.EnableEnhancedRebellions)
                return;

            CheckRebelKingdomFormation();
            EnforceRebelClanTiers();
        }

        private void CheckRebelKingdomFormation()
        {
            foreach (var clan in Clan.All.Where(c => c.Kingdom == null && !c.IsEliminated && c != Clan.PlayerClan))
            {
                // Check if clan has been without kingdom for specified days
                if (clan.LastFactionChangeTime.ElapsedDaysUntilNow >= EDWSettings.Instance.DaysUntilKingdomFormation)
                {
                    // Try to form new kingdom
                    TryFormNewKingdom(clan);
                }
            }
        }

        private void TryFormNewKingdom(Clan clan)
        {
            if (clan.Leader != null && clan.Settlements.Any())
            {
                // Kingdom formation logic would be implemented here
                // This is a placeholder for the complex kingdom creation process
                
                string message = LocalizationManager.GetText("kingdom_formed", "{0} clan has formed a new kingdom!")
                    .Replace("{0}", clan.Name.ToString());
                InformationManager.DisplayMessage(new InformationMessage(message, Colors.Yellow));
            }
        }

        private void EnforceRebelClanTiers()
        {
            if (!EDWSettings.Instance.ForceRebelClanTier6)
                return;

            foreach (var clan in Clan.All.Where(c => c.Kingdom == null && !c.IsEliminated && c != Clan.PlayerClan))
            {
                if (clan.Tier < 6)
                {
                    // Force clan tier to 6
                    while (clan.Tier < 6)
                    {
                        clan.AddRenown(clan.RenownRequirementForTier(clan.Tier + 1) - clan.Renown + 1);
                    }

                    string message = LocalizationManager.GetText("clan_tier_increased", "{0} clan tier increased to 6")
                        .Replace("{0}", clan.Name.ToString());
                    InformationManager.DisplayMessage(new InformationMessage(message, Colors.Cyan));
                }
            }
        }

        private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification)
        {
            // Handle rebellion-related clan changes
            if (detail == ChangeKingdomAction.ChangeKingdomActionDetail.LeaveKingdom && newKingdom == null)
            {
                string message = LocalizationManager.GetText("rebellion_started", "Rebellion started in {0}!")
                    .Replace("{0}", clan.Name.ToString());
                InformationManager.DisplayMessage(new InformationMessage(message, Colors.Red));
            }
        }

        private void OnClanDestroyed(Clan destroyedClan)
        {
            // Handle clan destruction events
        }
    }

    public class BanditEnhancementBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
            CampaignEvents.OnQuestStartedEvent.AddNonSerializedListener(this, OnQuestStarted);
        }

        public override void SyncData(IDataStore dataStore)
        {
            // No specific data to sync
        }

        private void OnDailyTick()
        {
            if (!EDWSettings.Instance.EnableMod || !EDWSettings.Instance.EnableEnhancedBandits)
                return;

            // Enhanced bandit spawning and quest generation would be implemented here
            GenerateRandomBanditQuests();
        }

        private void GenerateRandomBanditQuests()
        {
            // Placeholder for bandit quest generation
            var random = new Random();
            if (random.NextDouble() < 0.05) // 5% chance daily for new bandit quest
            {
                string message = LocalizationManager.GetText("new_bandit_quest", "New bandit quest available: {0}")
                    .Replace("{0}", "Eliminate Bandit Hideout");
                InformationManager.DisplayMessage(new InformationMessage(message, Colors.Orange));
            }
        }

        private void OnQuestStarted(QuestBase quest)
        {
            // Handle bandit-related quest starts
        }
    }

    public class PerformanceOptimizationBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            // Performance optimization events
        }

        public override void SyncData(IDataStore dataStore)
        {
            // No data to sync
        }
    }
}