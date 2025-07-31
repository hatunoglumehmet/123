using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using EnhancedDiplomacyWarfare.Settings;

namespace EnhancedDiplomacyWarfare.Factions
{
    public class FactionManager
    {
        private static Dictionary<string, FactionData> _customFactions = new Dictionary<string, FactionData>();

        public static void InitializeCustomFactions()
        {
            CreateTradeFedera tion();
            CreateNorthernAlliance();
            CreateDesertConfederacy();
            CreateMountainRepublic();
            CreateSeaPeoples();
            CreateFreeCompanies();
        }

        private static void CreateTradeFederation()
        {
            var factionData = new FactionData
            {
                Id = "enhanced_trade_federation",
                Name = LocalizationManager.GetText("edw_trade_fed", "Enhanced Trade Federation"),
                Culture = "empire",
                PrimaryColor = TaleWorlds.Library.Color.FromUint(0xFF8C42FF),
                SecondaryColor = TaleWorlds.Library.Color.FromUint(0xFFFFD700),
                Traits = new List<FactionTrait>
                {
                    FactionTrait.WealthyTraders,
                    FactionTrait.ProfessionalArmy,
                    FactionTrait.TradingBonus,
                    FactionTrait.ExpensiveEquipment
                },
                SpecialFeatures = new List<string>
                {
                    "Enhanced trade income (+50%)",
                    "Access to mercenary units",
                    "Advanced equipment for troops",
                    "Diplomatic bonus with neutral factions",
                    "Can hire foreign troops"
                }
            };
            
            _customFactions[factionData.Id] = factionData;
        }

        private static void CreateNorthernAlliance()
        {
            var factionData = new FactionData
            {
                Id = "northern_clans_alliance",
                Name = LocalizationManager.GetText("edw_north_alliance", "Northern Clans Alliance"),
                Culture = "sturgia",
                PrimaryColor = TaleWorlds.Library.Color.FromUint(0xFF4169E1),
                SecondaryColor = TaleWorlds.Library.Color.FromUint(0xFFFFFFFF),
                Traits = new List<FactionTrait>
                {
                    FactionTrait.FearlessWarriors,
                    FactionTrait.ColdResistance,
                    FactionTrait.BerserkerRage,
                    FactionTrait.TribalUnity
                },
                SpecialFeatures = new List<string>
                {
                    "Berserker units with rage bonus",
                    "Immunity to winter penalties",
                    "Increased morale in harsh conditions",
                    "Clan loyalty bonuses",
                    "Enhanced raiding capabilities"
                }
            };
            
            _customFactions[factionData.Id] = factionData;
        }

        private static void CreateDesertConfederacy()
        {
            var factionData = new FactionData
            {
                Id = "desert_confederacy",
                Name = LocalizationManager.GetText("edw_desert_conf", "Desert Confederacy"),
                Culture = "aserai",
                PrimaryColor = TaleWorlds.Library.Color.FromUint(0xFFD2691E),
                SecondaryColor = TaleWorlds.Library.Color.FromUint(0xFFFFF8DC),
                Traits = new List<FactionTrait>
                {
                    FactionTrait.DesertMasters,
                    FactionTrait.SwiftCavalry,
                    FactionTrait.NomadicMobility,
                    FactionTrait.OasisControl
                },
                SpecialFeatures = new List<string>
                {
                    "Camel cavalry units",
                    "Desert movement bonuses",
                    "Oasis income bonuses",
                    "Enhanced scouting range",
                    "Sandstorm tactical advantage"
                }
            };
            
            _customFactions[factionData.Id] = factionData;
        }

        private static void CreateMountainRepublic()
        {
            var factionData = new FactionData
            {
                Id = "mountain_republic",
                Name = LocalizationManager.GetText("edw_mountain_rep", "Mountain Republic"),
                Culture = "battania",
                PrimaryColor = TaleWorlds.Library.Color.FromUint(0xFF228B22),
                SecondaryColor = TaleWorlds.Library.Color.FromUint(0xFF8FBC8F),
                Traits = new List<FactionTrait>
                {
                    FactionTrait.MountainFighters,
                    FactionTrait.GuerrillaTactics,
                    FactionTrait.ForestAmbush,
                    FactionTrait.IndependentSpirit
                },
                SpecialFeatures = new List<string>
                {
                    "Mountain fortress bonuses",
                    "Guerrilla warfare tactics",
                    "Forest ambush capabilities",
                    "Independence diplomacy bonus",
                    "Enhanced archer units"
                }
            };
            
            _customFactions[factionData.Id] = factionData;
        }

        private static void CreateSeaPeoples()
        {
            var factionData = new FactionData
            {
                Id = "sea_peoples_coalition",
                Name = LocalizationManager.GetText("edw_sea_peoples", "Sea Peoples Coalition"),
                Culture = "vlandia",
                PrimaryColor = TaleWorlds.Library.Color.FromUint(0xFF1E90FF),
                SecondaryColor = TaleWorlds.Library.Color.FromUint(0xFFF0F8FF),
                Traits = new List<FactionTrait>
                {
                    FactionTrait.SeaMasters,
                    FactionTrait.CoastalRaiders,
                    FactionTrait.NavalSuperiority,
                    FactionTrait.AmphibiousWarfare
                },
                SpecialFeatures = new List<string>
                {
                    "Naval battle advantages",
                    "Coastal settlement bonuses",
                    "Amphibious assault capabilities",
                    "Sea trade route control",
                    "Ship-based recruitment"
                }
            };
            
            _customFactions[factionData.Id] = factionData;
        }

        private static void CreateFreeCompanies()
        {
            var factionData = new FactionData
            {
                Id = "free_companies",
                Name = LocalizationManager.GetText("edw_free_companies", "Free Companies Brotherhood"),
                Culture = "empire",
                PrimaryColor = TaleWorlds.Library.Color.FromUint(0xFF800080),
                SecondaryColor = TaleWorlds.Library.Color.FromUint(0xFFFFD700),
                Traits = new List<FactionTrait>
                {
                    FactionTrait.ProfessionalMercenaries,
                    FactionTrait.EliteTraining,
                    FactionTrait.ContractWarfare,
                    FactionTrait.DisciplinedTroops
                },
                SpecialFeatures = new List<string>
                {
                    "Elite mercenary units",
                    "Contract-based warfare",
                    "Advanced military training",
                    "Flexible allegiances",
                    "Superior equipment access"
                }
            };
            
            _customFactions[factionData.Id] = factionData;
        }

        public static FactionData GetFactionData(string factionId)
        {
            return _customFactions.ContainsKey(factionId) ? _customFactions[factionId] : null;
        }

        public static List<FactionData> GetAllCustomFactions()
        {
            return _customFactions.Values.ToList();
        }

        public static void ApplyFactionBonuses(Clan clan)
        {
            if (clan?.Kingdom == null) return;

            var factionData = GetFactionData(clan.Kingdom.StringId);
            if (factionData == null) return;

            // Apply faction-specific bonuses
            foreach (var trait in factionData.Traits)
            {
                ApplyFactionTrait(clan, trait);
            }
        }

        private static void ApplyFactionTrait(Clan clan, FactionTrait trait)
        {
            switch (trait)
            {
                case FactionTrait.WealthyTraders:
                    // Apply trade income bonus
                    if (clan.Leader?.PartyBelongedTo != null)
                    {
                        // Trade bonus would be applied through patches
                    }
                    break;

                case FactionTrait.FearlessWarriors:
                    // Apply morale bonus
                    if (clan.Leader?.PartyBelongedTo != null)
                    {
                        // Morale bonus applied through patches
                    }
                    break;

                case FactionTrait.DesertMasters:
                    // Apply desert movement bonus
                    break;

                // Add more trait applications as needed
            }
        }
    }

    public class FactionData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Culture { get; set; }
        public TaleWorlds.Library.Color PrimaryColor { get; set; }
        public TaleWorlds.Library.Color SecondaryColor { get; set; }
        public List<FactionTrait> Traits { get; set; } = new List<FactionTrait>();
        public List<string> SpecialFeatures { get; set; } = new List<string>();
        public Dictionary<string, float> StatModifiers { get; set; } = new Dictionary<string, float>();
    }

    public enum FactionTrait
    {
        // Trade Federation
        WealthyTraders,
        ProfessionalArmy,
        TradingBonus,
        ExpensiveEquipment,

        // Northern Alliance
        FearlessWarriors,
        ColdResistance,
        BerserkerRage,
        TribalUnity,

        // Desert Confederacy
        DesertMasters,
        SwiftCavalry,
        NomadicMobility,
        OasisControl,

        // Mountain Republic
        MountainFighters,
        GuerrillaTactics,
        ForestAmbush,
        IndependentSpirit,

        // Sea Peoples
        SeaMasters,
        CoastalRaiders,
        NavalSuperiority,
        AmphibiousWarfare,

        // Free Companies
        ProfessionalMercenaries,
        EliteTraining,
        ContractWarfare,
        DisciplinedTroops
    }

    public static class FactionFeatureManager
    {
        public static readonly Dictionary<string, List<FactionFeature>> FactionFeatures = new Dictionary<string, List<FactionFeature>>
        {
            ["enhanced_trade_federation"] = new List<FactionFeature>
            {
                new FactionFeature("Enhanced Trade Routes", "50% bonus to trade income from caravans"),
                new FactionFeature("Mercenary Recruitment", "Can recruit mercenary units from taverns"),
                new FactionFeature("Advanced Equipment", "20% better equipment quality for troops"),
                new FactionFeature("Diplomatic Wealth", "Gold bonuses for successful negotiations"),
                new FactionFeature("Banking System", "Passive income from controlled settlements"),
                new FactionFeature("Foreign Troops", "Can recruit troops from other cultures"),
                new FactionFeature("Trade Agreements", "Automatic trade deals with neutral factions"),
                new FactionFeature("Economic Warfare", "Can damage enemy economy through trade"),
                new FactionFeature("Luxury Goods", "Special trade goods with higher profits"),
                new FactionFeature("Financial Intelligence", "Know enemy gold reserves"),
                // ... continue to 100 features
            },
            // Add features for other factions...
        };

        public static List<FactionFeature> GetFactionFeatures(string factionId)
        {
            return FactionFeatures.ContainsKey(factionId) ? FactionFeatures[factionId] : new List<FactionFeature>();
        }
    }

    public class FactionFeature
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;

        public FactionFeature(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}