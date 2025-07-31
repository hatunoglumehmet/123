using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Library;

namespace EnhancedDiplomacyWarfare
{
    public static class LocalizationManager
    {
        private static Dictionary<string, Dictionary<string, string>> _localizations = new Dictionary<string, Dictionary<string, string>>();
        private static string _currentLanguage = "English";

        public static void Initialize()
        {
            LoadLocalizations();
        }

        public static void SetLanguage(string language)
        {
            if (_localizations.ContainsKey(language))
            {
                _currentLanguage = language;
            }
        }

        public static string GetText(string key, string defaultValue = "")
        {
            if (_localizations.ContainsKey(_currentLanguage) && _localizations[_currentLanguage].ContainsKey(key))
            {
                return _localizations[_currentLanguage][key];
            }
            
            // Fallback to English
            if (_localizations.ContainsKey("English") && _localizations["English"].ContainsKey(key))
            {
                return _localizations["English"][key];
            }
            
            return string.IsNullOrEmpty(defaultValue) ? key : defaultValue;
        }

        private static void LoadLocalizations()
        {
            // English localizations
            var englishTexts = new Dictionary<string, string>
            {
                // General
                { "settings_display_name", "Enhanced Diplomacy & Warfare" },
                { "mod_loaded", "Enhanced Diplomacy & Warfare loaded successfully!" },
                { "mod_failed", "Enhanced Diplomacy & Warfare failed to load!" },
                
                // Diplomacy
                { "diplomacy_success", "Diplomatic action successful!" },
                { "peace_extended", "Peace agreement extended by {0} days" },
                { "alliance_formed", "Alliance formed with {0}" },
                { "trade_agreement", "Trade agreement established with {0}" },
                
                // Army & Party
                { "party_size_increased", "Party size limit increased to {0}" },
                { "daily_recruitment", "Recruited {0} {1} troops for free" },
                { "clan_parties_increased", "Clan party limit increased by {0}" },
                
                // Rebellions
                { "rebellion_started", "Rebellion started in {0}!" },
                { "kingdom_formed", "{0} clan has formed a new kingdom!" },
                { "clan_tier_increased", "{0} clan tier increased to 6" },
                
                // Economy
                { "income_increased", "Settlement income increased by {0}%" },
                { "recruitment_speed", "Village recruitment speed increased by {0}%" },
                
                // Statistics
                { "level_up", "Statistics level increased to {0}!" },
                { "experience_gained", "Gained {0} experience points" },
                { "statistics_bonus", "Gained statistics bonus: {0}" },
                
                // Bandits
                { "new_bandit_quest", "New bandit quest available: {0}" },
                { "bandit_encounter", "Enhanced bandit encounter!" },
                
                // Performance
                { "performance_optimized", "Performance optimizations applied" },
                { "ai_optimization", "AI calculations optimized" },
                
                // UI
                { "open_statistics", "Open Statistics" },
                { "player_level", "Player Level: {0}" },
                { "total_experience", "Total Experience: {0}" },
                { "battles_won", "Battles Won: {0}" },
                { "settlements_captured", "Settlements Captured: {0}" },
                { "prisoners_taken", "Prisoners Taken: {0}" }
            };

            // Turkish localizations
            var turkishTexts = new Dictionary<string, string>
            {
                // General
                { "settings_display_name", "Gelişmiş Diplomasi ve Savaş" },
                { "mod_loaded", "Gelişmiş Diplomasi ve Savaş başarıyla yüklendi!" },
                { "mod_failed", "Gelişmiş Diplomasi ve Savaş yüklenemedi!" },
                
                // Diplomacy
                { "diplomacy_success", "Diplomatik eylem başarılı!" },
                { "peace_extended", "Barış anlaşması {0} gün uzatıldı" },
                { "alliance_formed", "{0} ile ittifak kuruldu" },
                { "trade_agreement", "{0} ile ticaret anlaşması yapıldı" },
                
                // Army & Party
                { "party_size_increased", "Grup boyutu limiti {0} olarak artırıldı" },
                { "daily_recruitment", "Ücretsiz {0} {1} askeri alındı" },
                { "clan_parties_increased", "Klan grup limiti {0} artırıldı" },
                
                // Rebellions
                { "rebellion_started", "{0} bölgesinde isyan başladı!" },
                { "kingdom_formed", "{0} klanı yeni krallık kurdu!" },
                { "clan_tier_increased", "{0} klan seviyesi 6'ya yükseltildi" },
                
                // Economy
                { "income_increased", "Yerleşim geliri %{0} artırıldı" },
                { "recruitment_speed", "Köy asker alma hızı %{0} artırıldı" },
                
                // Statistics
                { "level_up", "İstatistik seviyesi {0} oldu!" },
                { "experience_gained", "{0} deneyim puanı kazanıldı" },
                { "statistics_bonus", "İstatistik bonusu kazanıldı: {0}" },
                
                // Bandits
                { "new_bandit_quest", "Yeni haydut görevi mevcut: {0}" },
                { "bandit_encounter", "Gelişmiş haydut karşılaşması!" },
                
                // Performance
                { "performance_optimized", "Performans optimizasyonları uygulandı" },
                { "ai_optimization", "AI hesaplamaları optimize edildi" },
                
                // UI
                { "open_statistics", "İstatistikleri Aç" },
                { "player_level", "Oyuncu Seviyesi: {0}" },
                { "total_experience", "Toplam Deneyim: {0}" },
                { "battles_won", "Kazanılan Savaşlar: {0}" },
                { "settlements_captured", "Ele Geçirilen Yerleşimler: {0}" },
                { "prisoners_taken", "Alınan Esirler: {0}" }
            };

            _localizations["English"] = englishTexts;
            _localizations["Turkish"] = turkishTexts;
        }
    }
}