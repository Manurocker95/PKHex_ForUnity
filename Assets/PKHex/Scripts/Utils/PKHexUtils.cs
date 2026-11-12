// Needs PKHeX.Core.dll: excluded from data-only player builds (see PKHexExportWindow).
#if !PKHEX_DATA_ONLY || UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PKHeX.Core;
using UnityEngine;

namespace PKHexForUnity
{
    public enum LanguageCodes
    {
        ja,
        en,
        fr,
        it,
        de,
        es,
        ko,
        zh,
        zh2
    }

    public enum SpeciesLanguageCodes
    {
        ja1,
        ja,
        en,
        fr,
        it,
        de,
        es,
        es2,
        ko,
        zh,
        zh2
    }

    /// <summary>
    /// Game/generation selector. Existing values are kept unchanged (they may be serialized); new games are appended.
    /// </summary>
    public enum PokemonGeneration
    {
        None = 0,
        Kanto = 1,
        Johto = 2,
        Hoenn = 3,
        Sinnoh = 4,
        Tesselia = 5,
        Kalos = 6,
        Alola = 7,
        Galar = 8,
        SinnohPt = 9,
        HGSS = 10,
        TesseliaBW2 = 11,
        ORAS = 12,
        USUM = 13,
        KantoPLGPE = 14,
        BDSP = 15,
        LegendsArceus = 16,
        ScarletViolet = 17,
        Stadium = 18,
        Stadium2 = 19,
        Colosseum = 20,
        Box = 21,
        XD = 22,
        LegendsZA = 23,
        BattleRevolution = 24,
        Ranch = 25,
        StadiumJapan = 26,
        ORASDemo = 27
    }

    public static class PKHexUtils
    {
        private const string DEFAULT_LANGUAGE_CODE = "en";
        private const string NONE_TEXT = "NONE";
        private const string IL2CPP_DEFINE = "USE_IL2CPP";

        private static string s_errorText = "Internal error.";

        /// <summary>
        /// Language codes supported for loading string resources (PKHeX codes).
        /// </summary>
        public static string[] LanguageCodesAsStringArray = { "ja", "en", "fr", "it", "de", "es", "es-419", "ko", "zh-Hans", "zh-Hant" };

        public static string L_AError
        {
            get
            {
                return s_errorText;
            }
            set
            {
                s_errorText = value;
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("PKHex4Unity/Show Version")]
#endif
        public static void ShowVersion()
        {
#if UNITY_EDITOR
            string version = "1.0.0";
            string path = Application.dataPath + "/PKHex/version.txt";
            if (File.Exists(path))
            {
                version = File.ReadAllText(path);
            }
            UnityEditor.EditorUtility.DisplayDialog("PKHex For Unity", "Version: " + version, "Ok");
#endif
        }

        public static bool IsIL2CPPEnabled()
        {
#if UNITY_EDITOR
            return UnityEditor.PlayerSettings.GetScriptingBackend(UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup) == UnityEditor.ScriptingImplementation.IL2CPP;
#elif USE_IL2CPP
            return true;
#else
            return false;
#endif
        }

#if UNITY_EDITOR
        public static void AddUseIL2CPP()
        {
            string definesString = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup);
            List<string> allDefines = definesString.Split(';').ToList();
            if (!allDefines.Contains(IL2CPP_DEFINE))
            {
                allDefines.Add(IL2CPP_DEFINE);
            }
            UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines.ToArray()));
        }

        public static void RemoveUseIL2CPP()
        {
            string definesString = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup);
            List<string> allDefines = definesString.Split(';').ToList();
            if (allDefines.Contains(IL2CPP_DEFINE))
            {
                allDefines.Remove(IL2CPP_DEFINE);
            }
            UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines.ToArray()));
        }

        [UnityEditor.MenuItem("PLGU/PKHex/Set IL2CPP Define Symbol")]
        public static void AddUseIL2CPPIfEnabled()
        {
            if (IsIL2CPPEnabled())
            {
                AddUseIL2CPP();
            }
            else
            {
                RemoveUseIL2CPP();
            }
        }
#endif

        // ------------------------------------------------------------------ Games and generations

        /// <summary> Representative single game for each <see cref="PokemonGeneration"/> value. </summary>
        public static GameVersion GetGameVersion(PokemonGeneration _generation)
        {
            switch (_generation)
            {
                case PokemonGeneration.Kanto:
                case PokemonGeneration.Stadium:
                case PokemonGeneration.StadiumJapan:
                    return GameVersion.YW;
                case PokemonGeneration.Johto:
                case PokemonGeneration.Stadium2:
                    return GameVersion.C;
                case PokemonGeneration.Hoenn:
                    return GameVersion.E;
                case PokemonGeneration.Box:
                    return GameVersion.R;
                case PokemonGeneration.Colosseum:
                case PokemonGeneration.XD:
                    return GameVersion.CXD;
                case PokemonGeneration.Sinnoh:
                case PokemonGeneration.Ranch:
                    return GameVersion.D;
                case PokemonGeneration.SinnohPt:
                case PokemonGeneration.BattleRevolution:
                    return GameVersion.Pt;
                case PokemonGeneration.HGSS:
                    return GameVersion.HG;
                case PokemonGeneration.Tesselia:
                    return GameVersion.B;
                case PokemonGeneration.TesseliaBW2:
                    return GameVersion.B2;
                case PokemonGeneration.Kalos:
                    return GameVersion.X;
                case PokemonGeneration.ORAS:
                case PokemonGeneration.ORASDemo:
                    return GameVersion.OR;
                case PokemonGeneration.Alola:
                    return GameVersion.SN;
                case PokemonGeneration.USUM:
                    return GameVersion.US;
                case PokemonGeneration.KantoPLGPE:
                    return GameVersion.GP;
                case PokemonGeneration.Galar:
                    return GameVersion.SW;
                case PokemonGeneration.BDSP:
                    return GameVersion.BD;
                case PokemonGeneration.LegendsArceus:
                    return GameVersion.PLA;
                case PokemonGeneration.ScarletViolet:
                    return GameVersion.SL;
                case PokemonGeneration.LegendsZA:
                    return GameVersion.ZA;
                default:
                    return GameVersion.SL;
            }
        }

        public static EntityContext GetContext(PokemonGeneration _generation)
        {
            return RuntimeGameData.GetContext(GetGameVersion(_generation));
        }

        /// <summary> Highest species ID available in the game(s) of <paramref name="_generation"/>. </summary>
        public static int GetDexCount(PokemonGeneration _generation = PokemonGeneration.ScarletViolet)
        {
            return RuntimeGameData.GetMaxSpeciesID(GetGameVersion(_generation));
        }

        // ------------------------------------------------------------------ Species data (per game)

        /// <summary> Full species data (stats, types, abilities, learnsets, evolutions) in a game. </summary>
        public static RuntimeSpeciesData GetSpeciesData(ushort _species, byte _form, PokemonGeneration _generation, SystemLanguage _language = SystemLanguage.English)
        {
            return RuntimeGameData.GetSpeciesData(_species, _form, GetGameVersion(_generation), GetLanguageCode(_language));
        }

        /// <summary> Full species data using the most complete game of a numeric generation (1-9). </summary>
        public static RuntimeSpeciesData GetSpeciesDataByGeneration(ushort _species, byte _form, int _generation, SystemLanguage _language = SystemLanguage.English)
        {
            return RuntimeGameData.GetSpeciesDataByGeneration(_species, _form, _generation, GetLanguageCode(_language));
        }

        public static RuntimeSpeciesData GetSpeciesData(string _species, PokemonGeneration _generation, SystemLanguage _language = SystemLanguage.English)
        {
            int id = GetSpeciesID(_species, _language);
            if (id <= 0)
            {
                Debug.LogError("[PKHex] Unknown species name: " + _species);
                return null;
            }
            return GetSpeciesData((ushort)id, 0, _generation, _language);
        }

        // ------------------------------------------------------------------ Runtime generation

        /// <summary>
        /// Generates a Pokémon for the game of <paramref name="_generation"/> (legal when the species has an encounter there).
        /// </summary>
        public static PKM GeneratePokemon(ushort _species, byte _form, byte _level, PokemonGeneration _generation, ITrainerInfo _trainer = null, bool _shiny = false)
        {
            return RuntimeGenerator.Generate(_species, _form, _level, GetGameVersion(_generation), _trainer, _shiny);
        }

        public static PKM GeneratePokemon(string _species, byte _level, PokemonGeneration _generation, SystemLanguage _language = SystemLanguage.English, ITrainerInfo _trainer = null)
        {
            int id = GetSpeciesID(_species, _language);
            if (id <= 0)
            {
                Debug.LogError("[PKHex] Unknown species name: " + _species);
                return null;
            }
            return GeneratePokemon((ushort)id, 0, _level, _generation, _trainer, false);
        }

        /// <summary> Generates a Pokémon from Showdown set text (e.g. "Pikachu @ Light Ball\nLevel: 50\n- Thunderbolt"). </summary>
        public static PKM GeneratePokemonFromShowdown(string _setText, PokemonGeneration _generation, ITrainerInfo _trainer = null)
        {
            return RuntimeGenerator.GenerateFromShowdown(_setText, GetGameVersion(_generation), _trainer);
        }

        /// <summary> Empty entity of the generation's format with only the species set. </summary>
        public static PKM GetPokemonFromSpecies(string _species, PokemonGeneration _generation)
        {
            PKM pk = EntityBlank.GetBlank(GetContext(_generation), LanguageID.English);
            pk.Species = (ushort)Mathf.Max(0, GetSpeciesInt(_species));
            return pk;
        }

        public static RuntimeEntityData ConvertPKMToData(PKM _pkm, SystemLanguage _language = SystemLanguage.English, bool _checkLegality = false)
        {
            return RuntimeEntityData.FromPKM(_pkm, GetLanguageCode(_language), _checkLegality);
        }

        // ------------------------------------------------------------------ Dex / species names

        public static Dictionary<string, string> GetFullDexNamesAsDictionary(SystemLanguage _language = SystemLanguage.English, GameStrings _gameStrings = null)
        {
            Dictionary<string, string> dex = new Dictionary<string, string>();
            List<string> english = GetFullDexNames(SystemLanguage.English);
            List<string> other = GetFullDexNames(_language, _gameStrings);
            for (int i = 0; i < english.Count && i < other.Count; i++)
            {
                if (!dex.ContainsKey(english[i]))
                {
                    dex.Add(english[i], other[i]);
                }
            }
            return dex;
        }

        public static List<string> GetFullDexNames(LanguageID _language, GameStrings _gameStrings = null)
        {
            GameStrings strings = _gameStrings != null ? _gameStrings : GetLocalizedTexts(_language);
            return new List<string>(strings.specieslist);
        }

        public static List<string> GetFullDexNames(SystemLanguage _language = SystemLanguage.English, GameStrings _gameStrings = null)
        {
            GameStrings strings = _gameStrings != null ? _gameStrings : GetLocalizedTexts(_language);
            return new List<string>(strings.specieslist);
        }

        public static int GetFullDexCount()
        {
            return GetFullDexNames().Count - 1; // Index 0 is "Egg"/none.
        }

        public static string GetTeratypeFromPokemon(PK9 _pokemon, SystemLanguage _language = SystemLanguage.English, GameStrings _gameStrings = null)
        {
            GameStrings strings = _gameStrings != null ? _gameStrings : GetLocalizedTexts(_language);
            int tera = (int)_pokemon.TeraType;
            return tera < strings.types.Length ? strings.types[tera] : NONE_TEXT;
        }

        public static int GetDexNumber(string _species, SystemLanguage _language = SystemLanguage.English, GameStrings _strings = null)
        {
            return GetFullDexNames(_language, _strings).IndexOf(_species);
        }

        public static int GetDexNumber(string _species, LanguageID _language, GameStrings _strings = null)
        {
            return GetFullDexNames(_language, _strings).IndexOf(_species);
        }

        /// <summary> PKHeX does not contain Pokédex flavor texts. </summary>
        public static string GetDexEntry(int _species, SystemLanguage _language, SaveFile _savefile = null, GameStrings _strings = null)
        {
            return "No Entry Found";
        }

        public static string GetDexEntry(int _species, LanguageID _language, SaveFile _savefile = null, GameStrings _strings = null)
        {
            return "No Entry Found";
        }

        public static int GetSpeciesInt(string _species)
        {
            return GetSpeciesID(_species);
        }

        public static int GetSpeciesID(string _species, SystemLanguage _language = SystemLanguage.English)
        {
            return RuntimeStrings.GetSpeciesID(_species, GetLanguageCode(_language));
        }

        public static string GetSpeciesFromID(int _species, SystemLanguage _language = SystemLanguage.English)
        {
            return RuntimeStrings.GetSpeciesName((ushort)_species, GetLanguageCode(_language));
        }

        public static string GetSpeciesFromID(int _species, LanguageID _language)
        {
            return RuntimeStrings.GetSpeciesName((ushort)_species, GetLanguageCode(_language));
        }

        public static IEnumerable<string> GetSpecies(SystemLanguage _language = SystemLanguage.English, GameStrings _gameStrings = null)
        {
            GameStrings strings = _gameStrings != null ? _gameStrings : GetLocalizedTexts(_language);
            return strings.specieslist;
        }

        public static IEnumerable<string> SpeciesStrings(LanguageID _language)
        {
            return RuntimeStrings.GetSpeciesNames(GetLanguageCode(_language));
        }

        public static IEnumerable<string> SpeciesStrings(SystemLanguage _language)
        {
            return RuntimeStrings.GetSpeciesNames(GetLanguageCode(_language));
        }

        public static IList<string> SpeciesStrings(string _lang = DEFAULT_LANGUAGE_CODE)
        {
            return RuntimeStrings.GetSpeciesNames(_lang);
        }

        public static string GetPokemonSpeciesNameInLanguage(PKM _pokemon, SystemLanguage _language)
        {
            return RuntimeStrings.GetSpeciesName(_pokemon.Species, GetLanguageCode(_language));
        }

        public static string GetPokemonSpeciesNameInLanguage(PKM _pokemon, LanguageID _language)
        {
            return RuntimeStrings.GetSpeciesName(_pokemon.Species, GetLanguageCode(_language));
        }

        /// <param name="_language"><see cref="LanguageID"/> value (as stored in saves).</param>
        public static string GetPokemonSpeciesNameInLanguage(PKM _pokemon, int _language)
        {
            return SpeciesName.GetSpeciesName(_pokemon.Species, _language);
        }

        // ------------------------------------------------------------------ Types / abilities / natures

        public static IEnumerable<string> GetPokemonTypesOfSpecies(string _species, PokemonGeneration _generation = PokemonGeneration.ScarletViolet, SystemLanguage _language = SystemLanguage.English, GameStrings _gameStrings = null)
        {
            RuntimeSpeciesData data = GetSpeciesData(_species, _generation, _language);
            if (data == null)
            {
                return new string[0];
            }
            return data.Type1 == data.Type2 ? new[] { data.Type1Name } : new[] { data.Type1Name, data.Type2Name };
        }

        public static IEnumerable<string> GetPokemonTypes(PKM _pkm, SystemLanguage _language = SystemLanguage.English, GameStrings _gameStrings = null)
        {
            PersonalInfo personalInfo = _pkm.PersonalInfo;
            List<string> types = new List<string>(GetTypes(_language, _gameStrings));
            // Gen 1-2 store GB internal type IDs; convert them to the modern index used by the type names.
            byte type1 = RuntimeGameData.GetModernType(personalInfo.Type1, _pkm.Context);
            byte type2 = RuntimeGameData.GetModernType(personalInfo.Type2, _pkm.Context);
            List<string> result = new List<string>();
            result.Add(types[type1]);
            if (type2 != type1)
            {
                result.Add(types[type2]);
            }
            return result;
        }

        public static List<string> GetPokemonTypesInLanguage(PKM _pokemon, SystemLanguage _language, GameStrings _gameStrings = null)
        {
            return new List<string>(GetPokemonTypes(_pokemon, _language, _gameStrings));
        }

        public static IEnumerable<string> GetPokemonAbilities(PKM _pkm, SystemLanguage _language = SystemLanguage.English, GameStrings _gameStrings = null)
        {
            PersonalInfo personalInfo = _pkm.PersonalInfo;
            List<string> abilities = new List<string>(GetAbilities(_language, _gameStrings));
            List<string> result = new List<string>();
            for (int i = 0; i < personalInfo.AbilityCount; i++)
            {
                int ability = personalInfo.GetAbilityAtIndex(i);
                if (ability >= 0 && ability < abilities.Count)
                {
                    result.Add(abilities[ability]);
                }
            }
            return result;
        }

        /// <summary> Last ability slot (the hidden ability in games that have one). </summary>
        public static IEnumerable<string> GetPokemonHiddenAbilities(PKM _pkm, SystemLanguage _language = SystemLanguage.English, GameStrings _gameStrings = null)
        {
            List<string> abilities = new List<string>(GetPokemonAbilities(_pkm, _language, _gameStrings));
            if (abilities.Count == 0)
            {
                return new List<string> { GetAbilities(_language, _gameStrings).ElementAt(0) };
            }
            return new List<string> { abilities[abilities.Count - 1] };
        }

        public static string GetTypeInLanguage(string _type, SystemLanguage _language = SystemLanguage.English, GameStrings _gameStrings = null)
        {
            if (_language == SystemLanguage.English)
            {
                return _type;
            }
            List<string> originalTypes = new List<string>(GetTypes(SystemLanguage.English));
            int index = originalTypes.IndexOf(_type);
            return index < 0 ? _type : GetTypes(_language, _gameStrings).ElementAt(index);
        }

        public static int IndexOf<T>(this IEnumerable<T> _enumerable, T _element, IEqualityComparer<T> _comparer = null)
        {
            int i = 0;
            IEqualityComparer<T> comparer = _comparer != null ? _comparer : EqualityComparer<T>.Default;
            foreach (T currentElement in _enumerable)
            {
                if (comparer.Equals(currentElement, _element))
                {
                    return i;
                }
                i++;
            }
            return -1;
        }

        public static IEnumerable<string> GetNatureNames(SystemLanguage _language = SystemLanguage.English, GameStrings _gameStrings = null)
        {
            GameStrings strings = _gameStrings != null ? _gameStrings : GetLocalizedTexts(_language);
            return strings.natures;
        }

        public static IEnumerable<string> GetBallNames(SystemLanguage _language = SystemLanguage.English, GameStrings _gameStrings = null)
        {
            GameStrings strings = _gameStrings != null ? _gameStrings : GetLocalizedTexts(_language);
            return strings.balllist;
        }

        public static IEnumerable<string> GetTypes(SystemLanguage _language = SystemLanguage.English, GameStrings _gameStrings = null)
        {
            GameStrings strings = _gameStrings != null ? _gameStrings : GetLocalizedTexts(_language);
            return strings.types;
        }

        public static IEnumerable<string> GetAbilities(SystemLanguage _language = SystemLanguage.English, GameStrings _gameStrings = null)
        {
            GameStrings strings = _gameStrings != null ? _gameStrings : GetLocalizedTexts(_language);
            return strings.abilitylist;
        }

        public static IEnumerable<string> GetCharacteristincsNames(SystemLanguage _language = SystemLanguage.English, GameStrings _gameStrings = null)
        {
            GameStrings strings = _gameStrings != null ? _gameStrings : GetLocalizedTexts(_language);
            return strings.characteristics;
        }

        public static string GetCharacteristic(int _chara, SystemLanguage _language = SystemLanguage.English)
        {
            return _chara < 0 ? string.Empty : GetCharacteristincsNames(_language).ElementAt(_chara);
        }

        public static string GetNatureName(Nature _nature, SystemLanguage _language = SystemLanguage.English, GameStrings _gameStrings = null)
        {
            return GetNatureNames(_language, _gameStrings).ElementAt((int)_nature);
        }

        public static string GetPokemonNatureInLanguage(PKM _pokemon, SystemLanguage _language, GameStrings _gameStrings = null)
        {
            return GetNatureName(_pokemon.Nature, _language, _gameStrings);
        }

        public static string GetBallName(int _ball, SystemLanguage _language = SystemLanguage.English, GameStrings _gameStrings = null)
        {
            return GetBallNames(_language, _gameStrings).ElementAt(_ball);
        }

        public static string GetAbilityName(int _ability, SystemLanguage _language = SystemLanguage.English, GameStrings _gameStrings = null)
        {
            return GetAbilities(_language, _gameStrings).ElementAt(_ability);
        }

        // ------------------------------------------------------------------ Items / locations / moves

        /// <param name="_generation">Generation number (1-9), mapped to that generation's main format.</param>
        public static IEnumerable<string> GetItemNames(int _generation, GameVersion _version = GameVersion.Any, SystemLanguage _language = SystemLanguage.English, GameStrings _gameStrings = null)
        {
            return GetItemNames((EntityContext)_generation, _version, _language, _gameStrings);
        }

        public static IEnumerable<string> GetItemNames(EntityContext _context, GameVersion _version = GameVersion.Any, SystemLanguage _language = SystemLanguage.English, GameStrings _gameStrings = null)
        {
            GameStrings strings = _gameStrings != null ? _gameStrings : GetLocalizedTexts(_language);
            return strings.GetItemStrings(_context, _version);
        }

        public static IEnumerable<string> GetItemNames(SystemLanguage _language = SystemLanguage.English, GameStrings _gameStrings = null)
        {
            return GetItemNames(EntityContext.Gen9, GameVersion.Any, _language, _gameStrings);
        }

        public static IEnumerable<string> GetItemNames(SaveFile _sav, SystemLanguage _language = SystemLanguage.English, GameStrings _gameStrings = null)
        {
            GameStrings strings = _gameStrings != null ? _gameStrings : GetLocalizedTexts(_language);
            return strings.GetItemStrings(_sav.Context, _sav.Version);
        }

        public static string GetItemName(SaveFile _sav, int _item)
        {
            return _item > -1 ? GetItemNames(_sav).ElementAt(_item) : NONE_TEXT;
        }

        public static string GetItemName(int _item, int _generation, GameVersion _version = GameVersion.Any, SystemLanguage _language = SystemLanguage.English, GameStrings _gameStrings = null)
        {
            return _item > -1 ? GetItemNames(_generation, _version, _language, _gameStrings).ElementAt(_item) : NONE_TEXT;
        }

        public static string GetMetLocation(bool _isEggLocation, int _location, int _format, int _generation, GameVersion _version = GameVersion.Any, SystemLanguage _language = SystemLanguage.English, GameStrings _gameStrings = null)
        {
            GameStrings strings = _gameStrings != null ? _gameStrings : GetLocalizedTexts(_language);
            return strings.GetLocationName(_isEggLocation, (ushort)_location, (byte)_format, (byte)_generation, _version);
        }

        public static string GetMetLocation(bool _isEggLocation, int _location, int _format, int _generation, int _version, SystemLanguage _language = SystemLanguage.English, GameStrings _gameStrings = null)
        {
            return GetMetLocation(_isEggLocation, _location, _format, _generation, (GameVersion)_version, _language, _gameStrings);
        }

        public static IEnumerable<string> MoveStrings(LanguageID _language)
        {
            return RuntimeStrings.GetMoveNames(GetLanguageCode(_language));
        }

        public static IEnumerable<string> MoveStrings(SystemLanguage _language)
        {
            return RuntimeStrings.GetMoveNames(GetLanguageCode(_language));
        }

        public static IList<string> MoveStrings(string _lang = DEFAULT_LANGUAGE_CODE)
        {
            return RuntimeStrings.GetMoveNames(_lang);
        }

        public static IEnumerable<string> GetMoveNames(IEnumerable<ushort> _moves, string _lang = DEFAULT_LANGUAGE_CODE)
        {
            IList<string> names = RuntimeStrings.GetStrings(_lang).movelist;
            List<string> result = new List<string>();
            foreach (ushort move in _moves)
            {
                result.Add(move < names.Count ? names[move] : L_AError);
            }
            return result;
        }

        public static IEnumerable<string> GetMoveNames(IEnumerable<ushort> _moves, SystemLanguage _lang)
        {
            return GetMoveNames(_moves, GetLanguageCode(_lang));
        }

        public static IEnumerable<string> GetMoveNames(IEnumerable<ushort> _moves, LanguageID _lang)
        {
            return GetMoveNames(_moves, GetLanguageCode(_lang));
        }

        // ------------------------------------------------------------------ Languages

        /// <summary> Maps the game's <see cref="LanguageID"/> values; index = (int)LanguageID. </summary>
        public static List<string> GetSpeciesLanguageList()
        {
            return new List<string> { "ja", "ja", "en", "fr", "it", "de", "es", "es", "ko", "zh-Hans", "zh-Hant", "es-419" };
        }

        /// <summary> Value expected by <see cref="SpeciesName.GetSpeciesName"/> (a <see cref="LanguageID"/>). </summary>
        public static int GetSpeciesLanguageListIndex(SystemLanguage _language = SystemLanguage.English)
        {
            return (int)GetLanguageID(_language);
        }

        public static int GetSpeciesLanguageListIndex(LanguageID _language)
        {
            return (int)_language;
        }

        public static int GetLanguageIndex(LanguageID _language)
        {
            return GameLanguage.GetLanguageIndex(GetLanguageCode(_language));
        }

        public static int GetLanguageIndex(SystemLanguage _language = SystemLanguage.English)
        {
            return GameLanguage.GetLanguageIndex(GetLanguageCode(_language));
        }

        public static GameStrings GetLocalizedTexts(SystemLanguage _language = SystemLanguage.English)
        {
            return RuntimeStrings.GetStrings(GetLanguageCode(_language));
        }

        public static GameStrings GetLocalizedTexts(LanguageID _language)
        {
            return RuntimeStrings.GetStrings(GetLanguageCode(_language));
        }

        public static string GetLanguageCode(LanguageID _language)
        {
            return RuntimeStrings.GetLanguageCode(_language);
        }

        public static string GetLanguageCode(SystemLanguage _language = SystemLanguage.English)
        {
            return RuntimeStrings.GetLanguageCode(GetLanguageID(_language));
        }

        public static int GetLanguageCodeIndex(LanguageID _language)
        {
            switch (_language)
            {
                case LanguageID.Japanese: return (int)LanguageCodes.ja;
                case LanguageID.French: return (int)LanguageCodes.fr;
                case LanguageID.Italian: return (int)LanguageCodes.it;
                case LanguageID.German: return (int)LanguageCodes.de;
                case LanguageID.Spanish:
                case LanguageID.SpanishL:
                    return (int)LanguageCodes.es;
                case LanguageID.Korean: return (int)LanguageCodes.ko;
                case LanguageID.ChineseS: return (int)LanguageCodes.zh;
                case LanguageID.ChineseT: return (int)LanguageCodes.zh2;
                default: return (int)LanguageCodes.en;
            }
        }

        public static int GetLanguageCodeIndex(SystemLanguage _language = SystemLanguage.English)
        {
            return GetLanguageCodeIndex(GetLanguageID(_language));
        }

        public static string GetLanguageIDAsString(SystemLanguage _language)
        {
            return GetLanguageID(_language).ToString();
        }

        public static LanguageID GetLanguageID(SystemLanguage _language)
        {
            switch (_language)
            {
                case SystemLanguage.Japanese: return LanguageID.Japanese;
                case SystemLanguage.French: return LanguageID.French;
                case SystemLanguage.Italian: return LanguageID.Italian;
                case SystemLanguage.German: return LanguageID.German;
                case SystemLanguage.Spanish: return LanguageID.Spanish;
                case SystemLanguage.Korean: return LanguageID.Korean;
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                    return LanguageID.ChineseS;
                case SystemLanguage.ChineseTraditional: return LanguageID.ChineseT;
                default: return LanguageID.English;
            }
        }

        /// <summary> Save language as <see cref="LanguageID"/> value, English when the save does not store one. </summary>
        public static int GetSafeLanguage(SaveFile _sav)
        {
            if (_sav == null || _sav.Language <= 0)
            {
                return (int)LanguageID.English;
            }
            return _sav.Language;
        }

        // ------------------------------------------------------------------ Saves and entity files

        public static SaveFile GetDefaultSaveFromPKM(PKM _pk, LanguageID _language)
        {
            return BlankSaveFile.Get(GetSaveVersionForEntity(_pk), _pk.OriginalTrainerName, _language);
        }

        public static SaveFile GetDefaultSaveFromPKM(PKM _pk)
        {
            LanguageID language = _pk.Language > 0 ? (LanguageID)_pk.Language : LanguageID.English;
            return BlankSaveFile.Get(GetSaveVersionForEntity(_pk), _pk.OriginalTrainerName, language);
        }

        private static GameVersion GetSaveVersionForEntity(PKM _pk)
        {
            if (_pk.Format == 1 && _pk.Japanese)
            {
                return GameVersion.BU;
            }
            return _pk.Context.GetSingleGameVersion();
        }

        public static SaveFile GetDefaultSave(GameVersion _version = GameVersion.SL, SaveFile _current = null)
        {
            return BlankSaveFile.Get(_version, _current);
        }

        /// <summary> Replacement of the old dynamic generic loader: returns a typed loader behind an interface. </summary>
        public static IPKHexSavLoader InitializeGenericSavLoader(string _sav)
        {
            return PKHexSavLoaderFactory.CreateFromPath(_sav);
        }

        public static KeyValuePair<bool, PKM> LoadPKMFromPath(string _file)
        {
            if (SaveFileExists(_file))
            {
                return LoadPKM(File.ReadAllBytes(_file), Path.GetExtension(_file));
            }
            return new KeyValuePair<bool, PKM>(false, null);
        }

        public static KeyValuePair<bool, PKM> LoadPKM(byte[] _data, string _ext)
        {
            PKM pk = RuntimeSaveData.LoadEntity(_data, _ext);
            return new KeyValuePair<bool, PKM>(pk != null, pk);
        }

        public static SaveFile GetSaveFileData(byte[] _data)
        {
            return RuntimeSaveData.LoadSaveFile(_data);
        }

        public static GameVersion GetGameVersion(byte[] _data)
        {
            SaveFile sav = RuntimeSaveData.LoadSaveFile(_data);
            return sav != null ? sav.Version : GameVersion.Invalid;
        }

        /// <summary>
        /// Loads a save. <see cref="PokemonGeneration.None"/> auto-detects the game; other values also check the save type.
        /// </summary>
        public static SaveFile LoadSaveFile(string _sav, PokemonGeneration _gen = PokemonGeneration.None)
        {
            switch (_gen)
            {
                case PokemonGeneration.Kanto: return PKHexGen1SavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.Stadium: return PKHexGen1StadiumSavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.StadiumJapan: return PKHexGen1StadiumJSavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.Johto: return PKHexGen2SavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.Stadium2: return PKHexGen2StadiumSavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.Hoenn: return PKHexGen3SavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.Box: return PKHexGen3BoxSavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.Colosseum: return PKHexGen3ColosseumSavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.XD: return PKHexGen3XDSavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.Sinnoh: return PKHexGenDPSavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.SinnohPt: return PKHexGenPTSavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.HGSS: return PKHexGenHGSSSavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.BattleRevolution: return PKHexGen4BRSavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.Ranch: return PKHexGen4RanchSavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.Tesselia: return PKHexGen5BWSavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.TesseliaBW2: return PKHexGen5BW2SavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.Kalos: return PKHexGen6XYSavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.ORAS: return PKHexGen6ORASSavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.ORASDemo: return PKHexGen6ORASDemoSavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.Alola: return PKHexGen7SUMOSavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.USUM: return PKHexGen7USUMSavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.KantoPLGPE: return PKHexGen8PLGPESavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.Galar: return PKHexGen8ShSwSavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.BDSP: return PKHexGen8BSSavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.LegendsArceus: return PKHexGen8LASavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.ScarletViolet: return PKHexGen9SCVISavLoader.LoadSaveFileFromPath(_sav);
                case PokemonGeneration.LegendsZA: return PKHexGen9ZASavLoader.LoadSaveFileFromPath(_sav);
                default: return PKHexSavLoaderFactory.LoadSaveFileFromPath(_sav);
            }
        }

        public static bool SaveFileExists(string _sav)
        {
            return File.Exists(_sav);
        }

        public static PKM GetPokemonInParty(SaveFile _savefile, int _slot)
        {
            return _savefile.GetPartySlotAtIndex(_slot);
        }

        public static string GetPokemonNameOfPartyInIndex(SaveFile _savefile, int _slot = 0, PokemonGeneration _generation = PokemonGeneration.None)
        {
            return GetPokemonInParty(_savefile, _slot).Nickname;
        }
    }
}
#endif
