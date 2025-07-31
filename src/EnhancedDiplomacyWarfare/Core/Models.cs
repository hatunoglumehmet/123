using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;
using EnhancedDiplomacyWarfare.Settings;

namespace EnhancedDiplomacyWarfare
{
    public class EnhancedPartyLimitModel : DefaultPartyLimitModel
    {
        public override int GetPartyLimitForTier(Clan clan, int tier)
        {
            int baseLimit = base.GetPartyLimitForTier(clan, tier);
            
            if (EDWSettings.Instance.EnableMod)
            {
                baseLimit += EDWSettings.Instance.ClanPartyCountBonus;
                
                // Add statistics bonus for player clan
                if (clan == Clan.PlayerClan && EDWSettings.Instance.EnablePlayerStatistics)
                {
                    baseLimit += Statistics.StatisticsManager.PlayerStats.Level / 5;
                }
            }
            
            return baseLimit;
        }

        public override int GetPartyMemberSizeLimit(PartyBase party, bool includeDescriptions = false)
        {
            int baseLimit = base.GetPartyMemberSizeLimit(party, includeDescriptions);
            
            if (EDWSettings.Instance.EnableMod)
            {
                baseLimit += EDWSettings.Instance.PartySizeBonus;
                
                // Add statistics bonus for main party
                if (party == PartyBase.MainParty && EDWSettings.Instance.EnablePlayerStatistics)
                {
                    baseLimit += Statistics.StatisticsManager.PlayerStats.GetPartyLimitBonus();
                }
            }
            
            return baseLimit;
        }
    }

    public class EnhancedIncomeModel : DefaultSettlementIncomeModel
    {
        public override int CalculateIncomeFromTaxes(Town town, bool includeDescriptions = false)
        {
            int baseIncome = base.CalculateIncomeFromTaxes(town, includeDescriptions);
            
            if (EDWSettings.Instance.EnableMod)
            {
                if (town.IsTown)
                {
                    baseIncome = (int)(baseIncome * EDWSettings.Instance.TownIncomeMultiplier);
                }
                else if (town.IsCastle)
                {
                    baseIncome = (int)(baseIncome * EDWSettings.Instance.CastleIncomeMultiplier);
                }
            }
            
            return baseIncome;
        }

        public override int CalculateIncomeFromProjects(Town town, bool includeDescriptions = false)
        {
            int baseIncome = base.CalculateIncomeFromProjects(town, includeDescriptions);
            
            if (EDWSettings.Instance.EnableMod)
            {
                if (town.IsTown)
                {
                    baseIncome = (int)(baseIncome * EDWSettings.Instance.TownIncomeMultiplier);
                }
                else if (town.IsCastle)
                {
                    baseIncome = (int)(baseIncome * EDWSettings.Instance.CastleIncomeMultiplier);
                }
            }
            
            return baseIncome;
        }
    }

    public class EnhancedRecruitmentModel : DefaultVolunteerModel
    {
        public override float GetDailyVolunteerProductionProbability(Hero hero, int index, Settlement settlement)
        {
            float baseProbability = base.GetDailyVolunteerProductionProbability(hero, index, settlement);
            
            if (EDWSettings.Instance.EnableMod && settlement.IsVillage)
            {
                baseProbability *= EDWSettings.Instance.VillageRecruitmentSpeed;
                
                // Add statistics bonus for player
                if (hero == Hero.MainHero && EDWSettings.Instance.EnablePlayerStatistics)
                {
                    baseProbability *= (1.0f + Statistics.StatisticsManager.PlayerStats.GetRecruitmentBonus());
                }
            }
            
            return baseProbability;
        }
    }
}