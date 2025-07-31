using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using HarmonyLib;
using EnhancedDiplomacyWarfare.Settings;
using System;

namespace EnhancedDiplomacyWarfare.Patches
{
    // Party Limit Enhancement Patches
    [HarmonyPatch(typeof(DefaultPartyLimitModel), "GetPartyLimitForTier")]
    public class PartySizePatch
    {
        static void Postfix(ref int __result, Clan clan, int tier)
        {
            if (EDWSettings.Instance.EnableMod)
            {
                __result += EDWSettings.Instance.PartySizeBonus;
                
                // Add statistics-based bonus for player
                if (clan == Clan.PlayerClan)
                {
                    __result += Statistics.StatisticsManager.PlayerStats.GetPartyLimitBonus();
                }
            }
        }
    }

    // Clan Party Count Enhancement
    [HarmonyPatch(typeof(DefaultClanTierModel), "GetPartyLimit")]
    public class ClanPartyLimitPatch
    {
        static void Postfix(ref int __result, Clan clan)
        {
            if (EDWSettings.Instance.EnableMod)
            {
                __result += EDWSettings.Instance.ClanPartyCountBonus;
            }
        }
    }

    // Army Size Enhancement
    [HarmonyPatch(typeof(DefaultPartyLimitModel), "GetPartyMemberSizeLimit")]
    public class ArmySizePatch
    {
        static void Postfix(ref int __result, PartyBase party)
        {
            if (EDWSettings.Instance.EnableMod)
            {
                __result += EDWSettings.Instance.PartySizeBonus;
                
                // Add statistics-based bonus for player party
                if (party == PartyBase.MainParty)
                {
                    __result += Statistics.StatisticsManager.PlayerStats.GetArmySizeBonus();
                }
            }
        }
    }

    // Income Enhancement Patches
    [HarmonyPatch(typeof(DefaultSettlementIncomeModel), "CalculateIncomeFromTaxes")]
    public class SettlementIncomePatch
    {
        static void Postfix(ref int __result, Town town)
        {
            if (EDWSettings.Instance.EnableMod)
            {
                __result = (int)(__result * EDWSettings.Instance.TownIncomeMultiplier);
            }
        }
    }

    [HarmonyPatch(typeof(DefaultSettlementIncomeModel), "CalculateIncomeFromProjects")]
    public class CastleIncomePatch
    {
        static void Postfix(ref int __result, Town town)
        {
            if (EDWSettings.Instance.EnableMod && town.IsCastle)
            {
                __result = (int)(__result * EDWSettings.Instance.CastleIncomeMultiplier);
            }
        }
    }

    // Village Income Enhancement
    [HarmonyPatch(typeof(DefaultVillageIncomeCalculatorModel), "CalculateDailyIncome")]
    public class VillageIncomePatch
    {
        static void Postfix(ref int __result, Village village)
        {
            if (EDWSettings.Instance.EnableMod)
            {
                __result = (int)(__result * EDWSettings.Instance.VillageIncomeMultiplier);
            }
        }
    }

    // Recruitment Speed Enhancement
    [HarmonyPatch(typeof(DefaultVolunteerModel), "GetDailyVolunteerProductionProbability")]
    public class RecruitmentSpeedPatch
    {
        static void Postfix(ref float __result, Hero hero, int index, Settlement settlement)
        {
            if (EDWSettings.Instance.EnableMod && settlement.IsVillage)
            {
                __result *= EDWSettings.Instance.VillageRecruitmentSpeed;
                
                // Add statistics-based bonus
                if (hero == Hero.MainHero)
                {
                    __result *= (1.0f + Statistics.StatisticsManager.PlayerStats.GetRecruitmentBonus());
                }
            }
        }
    }

    // Rebellion Enhancement Patches
    [HarmonyPatch(typeof(DefaultRebellionModel), "GetDailyRebellionProbability")]
    public class RebellionChancePatch
    {
        static void Postfix(ref float __result, Town town)
        {
            if (EDWSettings.Instance.EnableMod && EDWSettings.Instance.EnableEnhancedRebellions)
            {
                __result *= EDWSettings.Instance.RebellionChanceMultiplier;
            }
        }
    }

    // Draw Distance Enhancement
    [HarmonyPatch(typeof(TaleWorlds.Engine.Options), "GetRenderScale")]
    public class DrawDistancePatch
    {
        static void Postfix(ref float __result)
        {
            if (EDWSettings.Instance.EnableMod && EDWSettings.Instance.EnableEnhancedGraphics)
            {
                __result *= EDWSettings.Instance.DrawDistanceMultiplier;
            }
        }
    }

    // Diplomacy Enhancement Patches
    [HarmonyPatch(typeof(DefaultDiplomacyModel), "GetScoreOfMakingPeace")]
    public class DiplomacyPeacePatch
    {
        static void Postfix(ref float __result, IFaction factionMakingPeace, IFaction factionMakingPeaceWith)
        {
            if (EDWSettings.Instance.EnableMod && EDWSettings.Instance.EnableEnhancedDiplomacy)
            {
                if (factionMakingPeace == Clan.PlayerClan || factionMakingPeaceWith == Clan.PlayerClan)
                {
                    __result *= EDWSettings.Instance.DiplomacySuccessMultiplier;
                }
            }
        }
    }

    // Statistics Tracking Patches
    [HarmonyPatch(typeof(PlayerEncounter), "Finish")]
    public class BattleResultPatch
    {
        static void Postfix(bool isPlayerWinner)
        {
            if (EDWSettings.Instance.EnableMod && EDWSettings.Instance.EnablePlayerStatistics)
            {
                if (isPlayerWinner)
                {
                    Statistics.StatisticsManager.OnBattleWon();
                }
                else
                {
                    Statistics.StatisticsManager.OnBattleLost();
                }
            }
        }
    }

    [HarmonyPatch(typeof(Agent), "Die")]
    public class EnemyKillPatch
    {
        static void Postfix(Agent __instance, Blow blow)
        {
            if (EDWSettings.Instance.EnableMod && EDWSettings.Instance.EnablePlayerStatistics)
            {
                if (blow.AttackerAgent != null && blow.AttackerAgent.IsMainAgent && !__instance.IsFriendOf(blow.AttackerAgent))
                {
                    Statistics.StatisticsManager.OnEnemyKilled();
                }
            }
        }
    }

    // Performance Optimization Patches
    [HarmonyPatch(typeof(DefaultAiBehaviorModel), "GetRandomizedActionFrequency")]
    public class AIOptimizationPatch
    {
        static void Postfix(ref float __result)
        {
            if (EDWSettings.Instance.EnableMod && EDWSettings.Instance.EnablePerformanceOptimizations && EDWSettings.Instance.ReduceAICalculations)
            {
                __result *= 1.5f; // Reduce AI calculation frequency
            }
        }
    }

    // Daily Morale Bonus from Statistics
    [HarmonyPatch(typeof(DefaultPartyMoraleModel), "GetDailyMoraleChange")]
    public class MoraleBonusPatch
    {
        static void Postfix(ref ExplainedNumber __result, MobileParty party)
        {
            if (EDWSettings.Instance.EnableMod && EDWSettings.Instance.EnablePlayerStatistics && party == MobileParty.MainParty)
            {
                float moraleBonus = Statistics.StatisticsManager.PlayerStats.GetMoraleBonus();
                if (moraleBonus > 0)
                {
                    __result.Add(moraleBonus, LocalizationManager.GetText("statistics_bonus", "Statistics Bonus"));
                }
            }
        }
    }
}