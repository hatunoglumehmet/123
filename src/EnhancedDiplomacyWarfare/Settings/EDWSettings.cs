using MCM.Abstractions.Base.Global;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;

namespace EnhancedDiplomacyWarfare.Settings
{
    public class EDWSettings : AttributeGlobalSettings<EDWSettings>
    {
        public override string Id => "EnhancedDiplomacyWarfareSettings";
        public override string DisplayName => LocalizationManager.GetText("settings_display_name");
        public override string FolderName => "EnhancedDiplomacyWarfare";
        public override string FormatType => "xml";

        #region General Settings
        [SettingPropertyBool("Enable Mod", Order = 0, RequireRestart = false, HintText = "Enable/Disable the entire mod")]
        [SettingPropertyGroup("General", GroupOrder = 0)]
        public bool EnableMod { get; set; } = true;

        [SettingPropertyDropdown("Language", Order = 1, RequireRestart = true, HintText = "Select mod language")]
        [SettingPropertyGroup("General", GroupOrder = 0)]
        public Dropdown<string> Language { get; set; } = new Dropdown<string>(new string[] { "English", "Turkish" }, 0);

        [SettingPropertyBool("Debug Mode", Order = 2, RequireRestart = false, HintText = "Enable debug messages")]
        [SettingPropertyGroup("General", GroupOrder = 0)]
        public bool DebugMode { get; set; } = false;
        #endregion

        #region Diplomacy Settings
        [SettingPropertyBool("Enhanced Diplomacy", Order = 0, RequireRestart = false, HintText = "Enable enhanced diplomacy features")]
        [SettingPropertyGroup("Diplomacy", GroupOrder = 1)]
        public bool EnableEnhancedDiplomacy { get; set; } = true;

        [SettingPropertyFloatingInteger("Diplomacy Success Chance Multiplier", 0.1f, 5.0f, Order = 1, RequireRestart = false, HintText = "Multiply diplomacy success chances")]
        [SettingPropertyGroup("Diplomacy", GroupOrder = 1)]
        public float DiplomacySuccessMultiplier { get; set; } = 1.5f;

        [SettingPropertyInteger("Peace Duration Bonus Days", 0, 365, Order = 2, RequireRestart = false, HintText = "Extra days added to peace agreements")]
        [SettingPropertyGroup("Diplomacy", GroupOrder = 1)]
        public int PeaceDurationBonus { get; set; } = 30;

        [SettingPropertyBool("Auto Accept Reasonable Offers", Order = 3, RequireRestart = false, HintText = "Automatically accept reasonable diplomatic offers")]
        [SettingPropertyGroup("Diplomacy", GroupOrder = 1)]
        public bool AutoAcceptReasonableOffers { get; set; } = false;
        #endregion

        #region Army & Party Settings
        [SettingPropertyInteger("Party Size Bonus", 0, 200, Order = 0, RequireRestart = false, HintText = "Additional party size for all parties")]
        [SettingPropertyGroup("Army & Party", GroupOrder = 2)]
        public int PartySizeBonus { get; set; } = 50;

        [SettingPropertyInteger("Clan Party Count Bonus", 0, 10, Order = 1, RequireRestart = false, HintText = "Additional party count for all clans")]
        [SettingPropertyGroup("Army & Party", GroupOrder = 2)]
        public int ClanPartyCountBonus { get; set; } = 3;

        [SettingPropertyBool("Enable Daily Free Recruitment", Order = 2, RequireRestart = false, HintText = "Enable daily free troop recruitment")]
        [SettingPropertyGroup("Army & Party", GroupOrder = 2)]
        public bool EnableDailyFreeRecruitment { get; set; } = true;

        [SettingPropertyInteger("Daily Free Troops Min", 1, 10, Order = 3, RequireRestart = false, HintText = "Minimum daily free troops")]
        [SettingPropertyGroup("Army & Party", GroupOrder = 2)]
        public int DailyFreeTroopsMin { get; set; } = 1;

        [SettingPropertyInteger("Daily Free Troops Max", 1, 10, Order = 4, RequireRestart = false, HintText = "Maximum daily free troops")]
        [SettingPropertyGroup("Army & Party", GroupOrder = 2)]
        public int DailyFreeTroopsMax { get; set; } = 5;
        #endregion

        #region Rebellion Settings
        [SettingPropertyBool("Enhanced Rebellions", Order = 0, RequireRestart = false, HintText = "Enable enhanced rebellion system")]
        [SettingPropertyGroup("Rebellions", GroupOrder = 3)]
        public bool EnableEnhancedRebellions { get; set; } = true;

        [SettingPropertyFloatingInteger("Rebellion Chance Multiplier", 0.1f, 10.0f, Order = 1, RequireRestart = false, HintText = "Multiply rebellion chances")]
        [SettingPropertyGroup("Rebellions", GroupOrder = 3)]
        public float RebellionChanceMultiplier { get; set; } = 3.0f;

        [SettingPropertyInteger("Days Until Kingdom Formation", 1, 365, Order = 2, RequireRestart = false, HintText = "Days rebel clans wait before forming kingdoms")]
        [SettingPropertyGroup("Rebellions", GroupOrder = 3)]
        public int DaysUntilKingdomFormation { get; set; } = 30;

        [SettingPropertyBool("Force Rebel Clan Tier 6", Order = 3, RequireRestart = false, HintText = "Force all rebel clans to tier 6")]
        [SettingPropertyGroup("Rebellions", GroupOrder = 3)]
        public bool ForceRebelClanTier6 { get; set; } = true;
        #endregion

        #region Economy Settings
        [SettingPropertyFloatingInteger("Village Income Multiplier", 0.1f, 10.0f, Order = 0, RequireRestart = false, HintText = "Multiply village income")]
        [SettingPropertyGroup("Economy", GroupOrder = 4)]
        public float VillageIncomeMultiplier { get; set; } = 2.0f;

        [SettingPropertyFloatingInteger("Castle Income Multiplier", 0.1f, 10.0f, Order = 1, RequireRestart = false, HintText = "Multiply castle income")]
        [SettingPropertyGroup("Economy", GroupOrder = 4)]
        public float CastleIncomeMultiplier { get; set; } = 2.0f;

        [SettingPropertyFloatingInteger("Town Income Multiplier", 0.1f, 10.0f, Order = 2, RequireRestart = false, HintText = "Multiply town income")]
        [SettingPropertyGroup("Economy", GroupOrder = 4)]
        public float TownIncomeMultiplier { get; set; } = 2.0f;

        [SettingPropertyFloatingInteger("Village Recruitment Speed", 0.1f, 10.0f, Order = 3, RequireRestart = false, HintText = "Village recruitment speed multiplier")]
        [SettingPropertyGroup("Economy", GroupOrder = 4)]
        public float VillageRecruitmentSpeed { get; set; } = 3.0f;
        #endregion

        #region Graphics Settings
        [SettingPropertyFloatingInteger("Draw Distance Multiplier", 0.5f, 5.0f, Order = 0, RequireRestart = false, HintText = "Multiply draw distance")]
        [SettingPropertyGroup("Graphics", GroupOrder = 5)]
        public float DrawDistanceMultiplier { get; set; } = 1.5f;

        [SettingPropertyBool("Enhanced Graphics", Order = 1, RequireRestart = false, HintText = "Enable enhanced graphics features")]
        [SettingPropertyGroup("Graphics", GroupOrder = 5)]
        public bool EnableEnhancedGraphics { get; set; } = true;
        #endregion

        #region Statistics Settings
        [SettingPropertyBool("Enable Player Statistics", Order = 0, RequireRestart = false, HintText = "Enable player statistics tracking")]
        [SettingPropertyGroup("Statistics", GroupOrder = 6)]
        public bool EnablePlayerStatistics { get; set; } = true;

        [SettingPropertyBool("Statistics Level System", Order = 1, RequireRestart = false, HintText = "Enable statistics-based level system")]
        [SettingPropertyGroup("Statistics", GroupOrder = 6)]
        public bool EnableStatisticsLevelSystem { get; set; } = true;

        [SettingPropertyInteger("Level Up Experience Requirement", 100, 10000, Order = 2, RequireRestart = false, HintText = "Experience needed per level")]
        [SettingPropertyGroup("Statistics", GroupOrder = 6)]
        public int LevelUpExperienceRequirement { get; set; } = 1000;
        #endregion

        #region Bandit Enhancement Settings
        [SettingPropertyBool("Enhanced Bandit System", Order = 0, RequireRestart = false, HintText = "Enable enhanced bandit features")]
        [SettingPropertyGroup("Bandits", GroupOrder = 7)]
        public bool EnableEnhancedBandits { get; set; } = true;

        [SettingPropertyFloatingInteger("Bandit Count Multiplier", 0.1f, 10.0f, Order = 1, RequireRestart = false, HintText = "Multiply bandit party count")]
        [SettingPropertyGroup("Bandits", GroupOrder = 7)]
        public float BanditCountMultiplier { get; set; } = 2.0f;

        [SettingPropertyInteger("New Bandit Quests Count", 0, 200, Order = 2, RequireRestart = false, HintText = "Number of new bandit quests to add")]
        [SettingPropertyGroup("Bandits", GroupOrder = 7)]
        public int NewBanditQuestsCount { get; set; } = 100;
        #endregion

        #region Performance Settings
        [SettingPropertyBool("Performance Optimizations", Order = 0, RequireRestart = false, HintText = "Enable performance optimizations")]
        [SettingPropertyGroup("Performance", GroupOrder = 8)]
        public bool EnablePerformanceOptimizations { get; set; } = true;

        [SettingPropertyBool("Reduce AI Calculations", Order = 1, RequireRestart = false, HintText = "Reduce AI calculation frequency")]
        [SettingPropertyGroup("Performance", GroupOrder = 8)]
        public bool ReduceAICalculations { get; set; } = true;

        [SettingPropertyBool("Optimize Battle Performance", Order = 2, RequireRestart = false, HintText = "Optimize battle performance")]
        [SettingPropertyGroup("Performance", GroupOrder = 8)]
        public bool OptimizeBattlePerformance { get; set; } = true;
        #endregion
    }
}