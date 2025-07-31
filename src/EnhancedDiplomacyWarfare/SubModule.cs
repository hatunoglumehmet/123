using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using MCM.Abstractions.Base.Global;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace EnhancedDiplomacyWarfare
{
    public class SubModule : MBSubModuleBase
    {
        private static readonly Harmony Harmony = new Harmony("EnhancedDiplomacyWarfare");
        
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            
            try
            {
                Harmony.PatchAll(Assembly.GetExecutingAssembly());
                InformationManager.DisplayMessage(new InformationMessage("Enhanced Diplomacy & Warfare: Loaded successfully!", Colors.Green));
            }
            catch (Exception ex)
            {
                InformationManager.DisplayMessage(new InformationMessage($"Enhanced Diplomacy & Warfare: Failed to load - {ex.Message}", Colors.Red));
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            
            // Initialize localization
            LocalizationManager.Initialize();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            
            if (game.GameType is Campaign)
            {
                CampaignGameStarter campaignStarter = (CampaignGameStarter)gameStarterObject;
                
                // Add behaviors
                campaignStarter.AddBehavior(new DiplomacyBehavior());
                campaignStarter.AddBehavior(new RebellionEnhancementBehavior());
                campaignStarter.AddBehavior(new StatisticsBehavior());
                campaignStarter.AddBehavior(new DailyRecruitmentBehavior());
                campaignStarter.AddBehavior(new BanditEnhancementBehavior());
                campaignStarter.AddBehavior(new PerformanceOptimizationBehavior());
                
                // Add models
                campaignStarter.AddModel(new EnhancedPartyLimitModel());
                campaignStarter.AddModel(new EnhancedIncomeModel());
                campaignStarter.AddModel(new EnhancedRecruitmentModel());
            }
        }

        public override void OnGameEnd(Game game)
        {
            base.OnGameEnd(game);
            
            // Save statistics and settings
            if (game.GameType is Campaign)
            {
                StatisticsManager.SaveStatistics();
            }
        }
    }
}