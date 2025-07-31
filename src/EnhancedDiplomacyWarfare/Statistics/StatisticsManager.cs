using System;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace EnhancedDiplomacyWarfare.Statistics
{
    public class PlayerStatistics
    {
        [SaveableField(1)] public int Level { get; set; } = 1;
        [SaveableField(2)] public long TotalExperience { get; set; } = 0;
        [SaveableField(3)] public int BattlesWon { get; set; } = 0;
        [SaveableField(4)] public int BattlesLost { get; set; } = 0;
        [SaveableField(5)] public int EnemiesKilled { get; set; } = 0;
        [SaveableField(6)] public int PrisonersTaken { get; set; } = 0;
        [SaveableField(7)] public int SettlementsCaptured { get; set; } = 0;
        [SaveableField(8)] public int TradeProfitTotal { get; set; } = 0;
        [SaveableField(9)] public int DiplomaticSuccesses { get; set; } = 0;
        [SaveableField(10)] public int QuestsCompleted { get; set; } = 0;
        [SaveableField(11)] public int TournamentsWon { get; set; } = 0;
        [SaveableField(12)] public int DaysPlayed { get; set; } = 0;
        [SaveableField(13)] public int RenownGained { get; set; } = 0;
        [SaveableField(14)] public int InfluenceGained { get; set; } = 0;
        [SaveableField(15)] public int GoldEarned { get; set; } = 0;
        [SaveableField(16)] public int VillagesTownsCaptured { get; set; } = 0;
        [SaveableField(17)] public int CastlesCaptured { get; set; } = 0;
        [SaveableField(18)] public int BanditPartiesDefeated { get; set; } = 0;
        [SaveableField(19)] public int MarriagesArranged { get; set; } = 0;
        [SaveableField(20)] public int WorkshopsOwned { get; set; } = 0;

        // Level-based bonuses
        public int GetPartyLimitBonus() => Level * 5;
        public int GetArmySizeBonus() => Level * 10;
        public int GetDailyGoldBonus() => Level * 20;
        public int GetDailyInfluenceBonus() => Level * 2;
        public float GetMoraleBonus() => Math.Min(Level * 0.1f, 2.0f);
        public float GetRecruitmentBonus() => Math.Min(Level * 0.05f, 1.0f);
        
        public long GetExperienceForNextLevel() => Level * Settings.EDWSettings.Instance.LevelUpExperienceRequirement;
        
        public bool CanLevelUp() => TotalExperience >= GetExperienceForNextLevel();
        
        public void AddExperience(int amount)
        {
            TotalExperience += amount;
            
            while (CanLevelUp())
            {
                Level++;
                // Show level up message
                InformationManager.DisplayMessage(new InformationMessage(
                    LocalizationManager.GetText("level_up", "Level up to {0}!").Replace("{0}", Level.ToString()),
                    TaleWorlds.Library.Colors.Green));
            }
        }
    }

    public static class StatisticsManager
    {
        private static PlayerStatistics _playerStats;
        
        public static PlayerStatistics PlayerStats
        {
            get
            {
                if (_playerStats == null)
                    _playerStats = new PlayerStatistics();
                return _playerStats;
            }
            set => _playerStats = value;
        }

        public static void SaveStatistics()
        {
            // Statistics are automatically saved with the game save system
        }

        public static void LoadStatistics()
        {
            // Statistics are automatically loaded with the game save system
        }

        // Experience gaining methods
        public static void OnBattleWon() 
        { 
            PlayerStats.BattlesWon++;
            PlayerStats.AddExperience(100);
        }
        
        public static void OnBattleLost() 
        { 
            PlayerStats.BattlesLost++;
            PlayerStats.AddExperience(25);
        }
        
        public static void OnEnemyKilled() 
        { 
            PlayerStats.EnemiesKilled++;
            PlayerStats.AddExperience(5);
        }
        
        public static void OnPrisonerTaken() 
        { 
            PlayerStats.PrisonersTaken++;
            PlayerStats.AddExperience(10);
        }
        
        public static void OnSettlementCaptured() 
        { 
            PlayerStats.SettlementsCaptured++;
            PlayerStats.AddExperience(500);
        }
        
        public static void OnTradeProfit(int profit) 
        { 
            PlayerStats.TradeProfitTotal += profit;
            PlayerStats.AddExperience(Math.Max(1, profit / 100));
        }
        
        public static void OnDiplomaticSuccess() 
        { 
            PlayerStats.DiplomaticSuccesses++;
            PlayerStats.AddExperience(50);
        }
        
        public static void OnQuestCompleted() 
        { 
            PlayerStats.QuestsCompleted++;
            PlayerStats.AddExperience(75);
        }
        
        public static void OnTournamentWon() 
        { 
            PlayerStats.TournamentsWon++;
            PlayerStats.AddExperience(150);
        }
        
        public static void OnDayPassed() 
        { 
            PlayerStats.DaysPlayed++;
            PlayerStats.AddExperience(1);
        }
    }
}