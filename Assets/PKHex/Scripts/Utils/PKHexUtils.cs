using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using PKHeX.Core;
using System.Linq;
using PokemonLetsGoUnity;
using System;
using System.Reflection;
using static PKHexForUnity.PokeAPI.PokeAPIPokemonData;

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
		XD = 22
    }

    public static class PKHexUtils
	{

#if UNITY_EDITOR
        [UnityEditor.MenuItem("PKHex4Unity/Show Version")]
#endif
        public static void ShowVersion()
        {
#if UNITY_EDITOR
            string version = "1.0.0";
            string path = Application.dataPath + "/PKHex/version.txt";
            if (System.IO.File.Exists(path))
            {
                version = System.IO.File.ReadAllText(path);
            }
            UnityEditor.EditorUtility.DisplayDialog("PKHex For Unity", "Version: " + version, "Ok");
#endif
        }

		public static bool IsIL2CPPEnabled()
		{
#if UNITY_EDITOR
			return UnityEditor.PlayerSettings.GetScriptingBackend(UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup) == UnityEditor.ScriptingImplementation.IL2CPP;
#else
#if USE_IL2CPP
			return true;
#else
			return false;
#endif
#endif
		}

		public static Dictionary<string, string> GetFullDexNamesAsDictionary(SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
		{
			Dictionary<string, string> dex = new Dictionary<string, string>();
			List<string> english = GetFullDexNames(SystemLanguage.English);
			List<string> other = GetFullDexNames(_language, gameStrings);

			for (int i = 0; i < english.Count; i++)
			{
				dex.Add(english[i], other[i]);
            }

			return dex;
		}

        public static List<string> GetFullDexNames(LanguageID _language, GameStrings gameStrings = null)
        {
            if (_language != LanguageID.English)
            {

                var strings = gameStrings != null ? gameStrings : GetLocalizedTexts(_language);

                return new List<string>(strings.specieslist);
            }

            return new List<string>(GameInfo.Strings.specieslist);
        }

        public static int GetFullDexCount()
        {
            return GetFullDexNames().Count;
        }

        public static int GetDexCount(PokemonGeneration generation = PokemonGeneration.LegendsArceus)
        {
            switch (generation)
            {
                case PokemonGeneration.Kanto:
                case PokemonGeneration.Stadium:
                    return 151;
                case PokemonGeneration.Johto:
                case PokemonGeneration.Stadium2:
                    return 251;
                case PokemonGeneration.Hoenn:
                case PokemonGeneration.Colosseum:
                case PokemonGeneration.XD:
                case PokemonGeneration.Box:
                    return 386;
                case PokemonGeneration.Sinnoh:
                case PokemonGeneration.SinnohPt:
                case PokemonGeneration.HGSS:
                    return 493;
                case PokemonGeneration.Tesselia:
                case PokemonGeneration.TesseliaBW2:
                    return 649;
                case PokemonGeneration.Kalos:
                case PokemonGeneration.ORAS:
                    return 721;
                case PokemonGeneration.Alola:
                    return 800;
                case PokemonGeneration.USUM:
                    return 809;
                case PokemonGeneration.Galar:
                case PokemonGeneration.BDSP:
                    return 898;
                case PokemonGeneration.LegendsArceus:
                    return 905;
                case PokemonGeneration.ScarletViolet:
                    return 1010;
                default:
                    return 1010;
            }
        }

        public static string GetTeratypeFromPokemon(PK9 _pokemon, SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
        {
            return GetTypeInLanguage(_pokemon.TeraType.ToString());
        }

        public static List<string> GetFullDexNames(SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
		{
            if (_language != SystemLanguage.English)
            {

                var strings = gameStrings != null ? gameStrings : GetLocalizedTexts(_language);

                return new List<string>(strings.specieslist);
            }

            return new List<string>(GameInfo.Strings.specieslist);
		}

		public static int GetDexNumber(string _species, SystemLanguage _language = SystemLanguage.English, GameStrings strings = null)
		{
			return GetFullDexNames(_language, strings).IndexOf(_species);
		}

        public static int GetDexNumber(string _species, LanguageID _language, GameStrings strings = null)
        {
            return GetFullDexNames(_language, strings).IndexOf(_species);
        }

        public static string GetDexEntry(int _species, SystemLanguage _language, SaveFile savefile = null, GameStrings strings = null)
        {
            return "No Entry Found";
        }

        public static string GetDexEntry(int _species, LanguageID _language, SaveFile savefile = null, GameStrings strings = null)
        {
            return "No Entry Found";
        }

        public static string GetDexEntry(string _species, SystemLanguage _language, GameVersion _version = GameVersion.SWSH, GameStrings strings = null, SaveFile file = null)
        {
            var save = GetDefaultSave(_version, file);
            return GetDexEntry(GetSpeciesInt(_species), _language, save, strings);
        }

        public static string GetDexEntry(string _species, LanguageID _language, GameVersion _version = GameVersion.SWSH, GameStrings strings = null, SaveFile file = null)
        {
            var save = GetDefaultSave(_version, file);
            int speciesIndex = GetSpeciesInt(_species);

            Zukan8Index idx = new Zukan8Index(Zukan8Type.Galar, (ushort)speciesIndex);

            return GetDexEntry(speciesIndex, _language, save, strings);
        }

        public static int GetSpeciesInt(string _species)
		{
			return GetSpeciesID(_species);
		}

        public static SaveFile GetDefaultSaveFromPKM(PKM pk, LanguageID _language)
        {
            var ctx = pk.Context;
            var ver = ctx.GetSingleGameVersion();
            if (pk is { Format: 1, Japanese: true })
                ver = GameVersion.BU;

            return SaveUtil.GetBlankSAV(ver, pk.OT_Name, _language);
        }

        public static SaveFile GetDefaultSaveFromPKM(PKM pk)
        {
            var ctx = pk.Context;
            var ver = ctx.GetSingleGameVersion();
            if (pk is { Format: 1, Japanese: true })
                ver = GameVersion.BU;

            return SaveUtil.GetBlankSAV(ver, pk.OT_Name, (LanguageID)pk.Language);
        }

        public static SaveFile GetDefaultSave(GameVersion version = GameVersion.PLA, SaveFile current = null)
        {
            var lang = SaveUtil.GetSafeLanguage(current);
            var tr = SaveUtil.GetSafeTrainerName(current, lang);
            var sav = SaveUtil.GetBlankSAV(version, tr, lang);
            
            if (sav.Version == GameVersion.Invalid) // will fail to load
                sav = SaveUtil.GetBlankSAV((GameVersion)GameInfo.VersionDataSource.Max(z => z.Value), tr, lang);

            return sav;
        }

		public static PKM GetPokemonFromSpecies(string _species, PokemonGeneration generation)
        {
			switch (generation)
            {
				case PokemonGeneration.Kanto:
				case PokemonGeneration.Stadium:
					PK1 pkmn = new PK1() { Species = (ushort)GetSpeciesInt(_species) };
					return pkmn;
				case PokemonGeneration.Johto:
				case PokemonGeneration.Stadium2:
					PK2 pkmn2 = new PK2() { Species = (ushort)GetSpeciesInt(_species) };
					return pkmn2;
				case PokemonGeneration.Hoenn:
				case PokemonGeneration.Colosseum:
				case PokemonGeneration.XD:
				case PokemonGeneration.Box:
					PK3 pkmn3 = new PK3() { Species = (ushort)GetSpeciesInt(_species) };
					return pkmn3;
				case PokemonGeneration.Sinnoh:
				case PokemonGeneration.SinnohPt:
				case PokemonGeneration.HGSS:
					PK4 pkmn4 = new PK4() { Species = (ushort)GetSpeciesInt(_species) };
					return pkmn4;
				case PokemonGeneration.Tesselia:
				case PokemonGeneration.TesseliaBW2:
					PK5 pkmn5 = new PK5() { Species = (ushort)GetSpeciesInt(_species) };
					return pkmn5;
				case PokemonGeneration.Kalos:
				case PokemonGeneration.ORAS:
					PK6 pkmn6 = new PK6() { Species = (ushort)GetSpeciesInt(_species) };
					return pkmn6;
				case PokemonGeneration.Alola:
				case PokemonGeneration.USUM:
					PK7 pkmn7 = new PK7() { Species = (ushort)GetSpeciesInt(_species) };
					return pkmn7;
				case PokemonGeneration.Galar:
				case PokemonGeneration.BDSP:
				case PokemonGeneration.LegendsArceus:
					PK8 pkmn8 = new PK8() { Species = (ushort)GetSpeciesInt(_species) };
					return pkmn8;

				default:
					PK8 pkmndef = new PK8() { Species = (ushort)GetSpeciesInt(_species) };
					return pkmndef;
			}
        }

		public static IEnumerable<string> GetPokemonTypesOfSpecies(string _species, PokemonGeneration _gen = PokemonGeneration.LegendsArceus, SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
		{
			PK8 pkmn = new PK8() { Species = (ushort)GetSpeciesInt(_species) };
			return GetPokemonTypes(pkmn, _language);
		}

		public static IEnumerable<string> GetPokemonHiddenAbilities(PKM _pkm, SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
		{
			var abilities = new List<string>(GetPokemonAbilities(_pkm, _language, gameStrings));

			if (abilities.Count == 0)
				return new List<string>() { GetAbilities(_language, gameStrings).ElementAt(0) };

			return new List<string>() { abilities[abilities.Count - 1] };
		}

		public static IEnumerable<string> GetPokemonAbilities(PKM _pkm, SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
        {
			var personalInfo = _pkm.PersonalInfo;
			List<string> abilities = new List<string>(GetAbilities(_language, gameStrings));
			List<string> pkmnAbilities = new List<string>();

            var array = new int[3];
            var abilitySpan = new Span<int>(array);
			personalInfo.GetAbilities(abilitySpan);

            foreach (var ab in abilitySpan)
            {
				pkmnAbilities.Add(abilities[ab]);
			}
			return pkmnAbilities;
		}

        public static string GetTypeInLanguage(string _type, SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
        {
            if (_language == SystemLanguage.English)
                return _type;

            var originalTypes = GetTypes(SystemLanguage.English);
            var types = GetTypes(_language, gameStrings);

            return types.ElementAt(originalTypes.IndexOf(_type));
        }

        public static int IndexOf<T>(this IEnumerable<T> enumerable, T element, IEqualityComparer<T> comparer = null)
        {
            int i = 0;
            comparer = comparer ?? EqualityComparer<T>.Default;
            foreach (var currentElement in enumerable)
            {
                if (comparer.Equals(currentElement, element))
                {
                    return i;
                }

                i++;
            }

            return -1;
        }

        public static IEnumerable<string> GetPokemonTypes(PKM _pkm, SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
		{
			var personalInfo = _pkm.PersonalInfo;
			int type1 = personalInfo.Type1;
			int type2 = personalInfo.Type2;

			var types = GetTypes(_language, gameStrings); 

            List<string> pkmnTypes = new List<string>();
			pkmnTypes.Add(types.ElementAt(type1));

			if (type2 >= 0)
				pkmnTypes.Add(types.ElementAt(type2));

			return pkmnTypes;
		}

		public static int GetSpeciesID(string _species, SystemLanguage _language = SystemLanguage.English)
		{
            var langIDx = GetSpeciesLanguageListIndex(_language);
            return SpeciesName.GetSpeciesID(_species, langIDx);
		}

        public static string GetSpeciesFromID(int _species, SystemLanguage _language = SystemLanguage.English)
        {
            var langIDx = GetSpeciesLanguageListIndex(_language);
            return SpeciesName.GetSpeciesName((ushort)_species, langIDx);
        }

        public static string GetSpeciesFromID(int _species, LanguageID _language = LanguageID.English)
        {
            var langIDx = GetSpeciesLanguageListIndex(_language);
            return SpeciesName.GetSpeciesName((ushort)_species, langIDx);
        }



#if UNITY_EDITOR
        public static void AddUseIL2CPP()
        {
			string use = "USE_IL2CPP";
			string definesString = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup);
			List<string> allDefines = definesString.Split(';').ToList();
			if (!allDefines.Contains(use))
            {
				allDefines.Add(use);
			}
			UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines.ToArray()));
		}

		public static void RemoveUseIL2CPP()
		{
			string use = "USE_IL2CPP";
			string definesString = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup);
			List<string> allDefines = definesString.Split(';').ToList();
			if (allDefines.Contains(use))
			{
				allDefines.Remove(use);
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

		public static PLGU_PKHexPokemonData ConvertPKMToBattleData(PKHeX.Core.PKM _pkm, SystemLanguage _language = SystemLanguage.English)
		{
			List<string> mvs = new List<string>();
			foreach (string st in GetMoveNames(_pkm.Moves, _language))
			{
				if (!st.Contains("—"))
					mvs.Add(st);
			}

			List<string> eggmvs = new List<string>();
			foreach (string st in GetMoveNames(_pkm.RelearnMoves, _language))
			{
				if (!st.Contains("—"))
					eggmvs.Add(st);
			}

			int gender = _pkm.Gender;
			int trgen = _pkm.OT_Gender;
			int obtain = 0;

			if (_pkm.FatefulEncounter)
			{
				obtain = 3;
			}
			else if (_pkm.WasEgg)
			{
				obtain = 1;
			}
			else if (_pkm.IsTradedEgg)
			{
				obtain = 2;
			}

			int stage =0;

			if (_pkm.PKRS_Cured)
			{
				stage = 2;
			}
			else if (_pkm.PKRS_Infected)
			{
				stage = 1;
			}
#if !USE_IL2CPP
			int generation = _pkm.Generation;
			int form = _pkm.Form;
#else
			int generation = 8;
			int form = 0;
#endif
			int version = _pkm.Version;

			PLGU_PKHexPokemonData pokemonData = new PLGU_PKHexPokemonData()
			{
				m_species = GetPokemonSpeciesNameInLanguage(_pkm, Application.systemLanguage),
				m_currentLevel = _pkm.CurrentLevel,
				m_form = form,
				m_gender = gender,
				m_nature = GetNatureName(_pkm.Nature),
				m_isShiny = _pkm.IsShiny,
				PID = (int)_pkm.PID,
				m_ivs = new List<int>() { _pkm.IV_HP, _pkm.IV_ATK, _pkm.IV_DEF, _pkm.IV_SPE, _pkm.IV_SPA, _pkm.IV_SPD },
				m_evs = new List<int>() { _pkm.EV_HP, _pkm.EV_ATK, _pkm.EV_DEF, _pkm.EV_SPE, _pkm.EV_SPA, _pkm.EV_SPD },
				m_moves = mvs,
				m_item = GetItemName(_pkm.HeldItem, generation),
				m_nickname = _pkm.Nickname,
				m_ability = GetAbilityName(_pkm.Ability),
				m_trainerName = _pkm.OT_Name,
				TID = (int)_pkm.TID,
				STID = (int)_pkm.TrainerSID7,
				m_trainerGender = trgen,
				m_ball = GetBallName(_pkm.Ball),
				m_obtainMode = obtain,
				m_msg = GetCharacteristic(_pkm.Characteristic),
				m_happiness = _pkm.CurrentFriendship,
				m_location = GetMetLocation(_pkm.WasEgg, _pkm.Met_Location, _pkm.Format, generation, version),
				m_status = "NORMAL",
				m_hasPokerus = !_pkm.PKRS_Cured && !_pkm.PKRS_Infected,
				m_stagePokerus = stage,
				m_daysPokerus = _pkm.PKRS_Days,
				m_strainPokerus = _pkm.PKRS_Strain,
				m_relearnMoves = eggmvs,
				m_marks = new List<bool>(){
					_pkm.GetMarking(_pkm.IV_HP) == 1 ? true : false,
					_pkm.GetMarking(_pkm.IV_ATK) == 1 ? true : false,
					_pkm.GetMarking(_pkm.IV_DEF) == 1 ? true : false,
					_pkm.GetMarking(_pkm.IV_SPA) == 1 ? true : false,
					_pkm.GetMarking(_pkm.IV_SPD) == 1 ? true : false,
					_pkm.GetMarking(_pkm.IV_SPE) == 1 ? true : false,
				},
				m_currentExperience = (int)_pkm.EXP
			};

			return pokemonData;
		}

		public static string GetCharacteristic(int _chara)
        {
			return GetCharacteristincsNames().ElementAt(_chara);

		}

		public static IEnumerable<string> GetCharacteristincsNames(SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
		{
            if (_language != SystemLanguage.English)
            {

                var strings = gameStrings != null ? gameStrings : GetLocalizedTexts(_language);

                return strings.characteristics;
            }
            return GameInfo.Strings.characteristics;
		}

		public static IEnumerable<string> GetStatusNames(SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
		{
            if (_language != SystemLanguage.English)
            {
                var strings = gameStrings != null ? gameStrings : GetLocalizedTexts(_language);

                return strings.characteristics;
            }
            return GameInfo.Strings.characteristics;
		}

        public static List<string> GetSpeciesLanguageList()
        {
            return new List<string>()
            {
                "ja",
                "ja",
                "en",
                "fr",
                "it",
                "de",
                "es",
                "es",
                "ko",
                "zh",
                "zh2"
            };
        }

        public static int GetSpeciesLanguageListIndex(SystemLanguage _language = SystemLanguage.English)
        {
            return GetSpeciesLanguageList().IndexOf(GetLanguageCode(_language));
        }

        public static int GetSpeciesLanguageListIndex(LanguageID _language = LanguageID.English)
        {
            return GetSpeciesLanguageList().IndexOf(GetLanguageCode(_language));
        }

        public static int GetLanguageIndex (LanguageID _language = LanguageID.English)
		{
            string languagCode = GetLanguageCode(_language);
            return GameLanguage.GetLanguageIndex(languagCode);
        }

		public static int GetLanguageIndex (SystemLanguage _language = SystemLanguage.English)
		{
            string languagCode = GetLanguageCode(_language);
            return GameLanguage.GetLanguageIndex(languagCode);
		}

		public static GameStrings GetLocalizedTexts(SystemLanguage _language = SystemLanguage.English)
		{
            return GameInfo.GetStrings(GetLanguageCode(_language));
        }	
        
        public static GameStrings GetLocalizedTexts(LanguageID _language = LanguageID.English)
		{
            return GameInfo.GetStrings(GetLanguageCode(_language));
        }

        public static string GetLanguageCode(LanguageID _language = LanguageID.English)
        {
            switch (_language)
            {
                case LanguageID.English:
                    return "en";
                case LanguageID.Spanish:
                    return "es";
                case LanguageID.Italian:
                    return "it";
                case LanguageID.French:
                    return "fr";
                case LanguageID.Japanese:
                    return "ja";
                case LanguageID.German:
                    return "de";
                case LanguageID.Korean:
                    return "ko";
                case LanguageID.ChineseS:
                    return "zh";
                case LanguageID.ChineseT:
                    return "zh2";
            }

            return "en";
        }

        public static int GetLanguageCodeIndex(LanguageID _language = LanguageID.English)
        {
            LanguageCodes code = LanguageCodes.en;
            switch (_language)
            {
                case LanguageID.English:
                    code = LanguageCodes.en;
                    break;
                case LanguageID.Spanish:
                    code = LanguageCodes.es;
                    break;
                case LanguageID.Italian:
                    code = LanguageCodes.it;
                    break;
                case LanguageID.French:
                    code = LanguageCodes.fr;
                    break;
                case LanguageID.Japanese:
                    code = LanguageCodes.ja;
                    break;
                case LanguageID.German:
                    code = LanguageCodes.de;
                    break;
                case LanguageID.Korean:
                    code = LanguageCodes.ko;
                    break;
                case LanguageID.ChineseS:
                    code = LanguageCodes.zh;
                    break;
                case LanguageID.ChineseT:
                    code = LanguageCodes.zh2;
                    break;
            }

            return (int)code;
        }

        public static int GetLanguageCodeIndex(SystemLanguage _language = SystemLanguage.English)
		{
			LanguageCodes code = LanguageCodes.en;
            switch (_language)
            {
                case SystemLanguage.English:
                    code = LanguageCodes.en;
					break;
                case SystemLanguage.Spanish:
                    code = LanguageCodes.es;
                    break;
                case SystemLanguage.Italian:
                    code = LanguageCodes.it;
                    break;
                case SystemLanguage.French:
                    code = LanguageCodes.fr;
                    break;
                case SystemLanguage.Japanese:
                    code = LanguageCodes.ja;
                    break;
                case SystemLanguage.German:
                    code = LanguageCodes.de;
                    break;
                case SystemLanguage.Korean:
                    code = LanguageCodes.ko;
                    break;
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.Chinese:
                    code = LanguageCodes.zh;
                    break;
                case SystemLanguage.ChineseTraditional:
                    code = LanguageCodes.zh2;
                    break;
            }

            return (int)code;
        }

        public static string GetLanguageCode(SystemLanguage _language = SystemLanguage.English)
		{
			switch (_language)
			{
				case SystemLanguage.English:
					return "en";
				case SystemLanguage.Spanish:
					return "es";
				case SystemLanguage.Italian:
					return "it";
				case SystemLanguage.French:
					return "fr";
				case SystemLanguage.Japanese:
					return "ja";
				case SystemLanguage.German:
					return "de";
				case SystemLanguage.Korean:
					return "ko";
				case SystemLanguage.ChineseSimplified:
				case SystemLanguage.Chinese:
					return "zh";
				case SystemLanguage.ChineseTraditional:
					return "zh2";
            }

            return "en";
        }

        /// <summary>
        /// Language codes supported for loading string resources
        /// </summary>
        /// <see cref="ProgramLanguage"/>
        public static string[] LanguageCodesAsStringArray = { "ja", "en", "fr", "it", "de", "es", "ko", "zh", "zh2" };

        public static IEnumerable<string> GetNatureNames(SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
		{
			if (_language != SystemLanguage.English)
			{	
				var strings = gameStrings != null ? gameStrings : GetLocalizedTexts(_language);

				return strings.Natures;
            }

            return GameInfo.Strings.Natures;
		}
		public static IEnumerable<string> GetBallNames(SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
		{
            if (_language != SystemLanguage.English)
			{
                var strings = gameStrings != null ? gameStrings : GetLocalizedTexts(_language);

                return strings.balllist;
            }

            return GameInfo.Strings.balllist;
		}

		public static IEnumerable<string> GetTypes(SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
		{
            if (_language != SystemLanguage.English)
            {
                var strings = gameStrings != null ? gameStrings : GetLocalizedTexts(_language);

                return strings.Types;
            }
            return GameInfo.Strings.Types;
        }

		public static IEnumerable<string> GetAbilities(SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
		{
            if (_language != SystemLanguage.English)
            {
                var strings = gameStrings != null ? gameStrings : GetLocalizedTexts(_language);

                return strings.Ability;
            }
            return GameInfo.Strings.Ability;
		}	
        
        public static IEnumerable<string> GetSpecies(SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
		{
            if (_language != SystemLanguage.English)
            {
                var langIDx = GetSpeciesLanguageListIndex(_language);
                return SpeciesName.SpeciesLang[langIDx];
            }

            if (gameStrings != null)
                return gameStrings.Species;

            return GameInfo.Strings.Species;
		}
		
		public static IEnumerable<string> GetItemNames(int _generation, GameVersion _version = GameVersion.Any, SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
		{
            if (_language != SystemLanguage.English)
            {
                var strings = gameStrings != null ? gameStrings : GetLocalizedTexts(_language);

                return strings.GetItemStrings((EntityContext)_generation, _version);
            }
            return GameInfo.Strings.GetItemStrings((EntityContext)_generation, _version);
		}

        public static IEnumerable<string> GetItemNames(EntityContext _generation, GameVersion _version = GameVersion.Any, SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
        {
            if (_language != SystemLanguage.English)
            {
                var strings = gameStrings != null ? gameStrings : GetLocalizedTexts(_language);

                return strings.GetItemStrings(_generation, _version);
            }
            return GameInfo.Strings.GetItemStrings(_generation, _version);
        }

        public static IEnumerable<string> GetItemNames(SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
        {
            if (_language != SystemLanguage.English)
            {
                var strings = gameStrings != null ? gameStrings : GetLocalizedTexts(_language);

                return strings.GetItemStrings(EntityContext.Gen8, GameVersion.Any);
            }
            return GameInfo.Strings.GetItemStrings(EntityContext.Gen8, GameVersion.Any);
        }

        public static IEnumerable<string> GetItemNames(SaveFile _sav, SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
		{
            if (_language != SystemLanguage.English)
            {
                var strings = gameStrings != null ? gameStrings : GetLocalizedTexts(_language);

                return strings.GetItemStrings(_sav.Context);
            }

            return GameInfo.Strings.GetItemStrings(_sav.Context);
		}

		public static string GetItemName(SaveFile _sav, int _item)
		{
			return _item > -1 ? GetItemNames(_sav).ElementAt(_item) : "NONE";
		}
		
		public static string GetMetLocation(bool _isEggLocation, int location, int _format, int _generation, GameVersion _version = GameVersion.Any, SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
		{
            if (_language != SystemLanguage.English)
            {
                var strings = gameStrings != null ? gameStrings : GetLocalizedTexts(_language);

                return strings.GetLocationName(_isEggLocation, location, _format, _generation, _version);
            }

            return GameInfo.Strings.GetLocationName(_isEggLocation, location, _format, _generation, _version);
		}
		
		public static string GetMetLocation(bool _isEggLocation, int location, int _format, int _generation, int _version, SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
        {
            if (_language != SystemLanguage.English)
            {
                var strings = gameStrings != null ? gameStrings : GetLocalizedTexts(_language);

                return strings.GetLocationName(_isEggLocation, location, _format, _generation, (GameVersion)_version);
            }
            return GameInfo.Strings.GetLocationName(_isEggLocation, location, _format, _generation, (GameVersion)_version);
		}
		
		public static string GetStatusName(int _status, SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
		{
			return GetStatusNames(_language, gameStrings).ElementAt(_status);
		}
		
		public static string GetBallName(int _ball, SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
		{
			return GetBallNames(_language, gameStrings).ElementAt(_ball);
		}
		public static string GetAbilityName(int _ability, SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
		{
			return GetAbilities(_language, gameStrings).ElementAt(_ability);
		}
		
		public static string GetNatureName(int _nature, SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
		{
			return GetNatureNames(_language, gameStrings).ElementAt(_nature);
		}
		
		public static string GetItemName(int _item, int _generation, PKHeX.Core.GameVersion _version = GameVersion.Any, SystemLanguage _language = SystemLanguage.English, GameStrings gameStrings = null)
		{
			return _item > -1 ? GetItemNames(_generation, _version, _language, gameStrings).ElementAt(_item) : "NONE";
		}
    	
        public static dynamic InitializeGenericSavLoader(string sav)
        {
            if (SaveFileExists(sav))
            {
                byte[] bytess = File.ReadAllBytes(sav);
                SaveFile savefile = GetSaveFileData(bytess);

                var type = typeof(PKHexSavLoader<>).MakeGenericType(savefile.GetType());
                dynamic savLoader = System.Activator.CreateInstance(type);
#if NET_4_6
				savLoader.SetSaveFile(savefile);
#else
				Debug.LogError("Can't init generic savefile because you are not using .NET 4.x. Please, change the framework in Project Settings.");
#endif
				return savLoader;
            }

            return null;
        }

        public static KeyValuePair<bool, PKM> LoadPKMFromPath(string file)
        {
            if (SaveFileExists(file))
            {
                return LoadPKM(File.ReadAllBytes(file), Path.GetExtension(file));
            }

            return new KeyValuePair<bool, PKM>(false, null);
        }

		public static string GetLanguageIDAsString(SystemLanguage _language)
		{
			return GetLanguageID(_language).ToString();
        }

		public static LanguageID GetLanguageID(SystemLanguage _language)
		{
            string lang = _language.ToString();
            LanguageID l = LanguageID.English;
            System.Enum.TryParse(lang, out l);

			return l;
        }

		public static List<string> GetPokemonTypesInLanguage(PKM pokemon, SystemLanguage _language, GameStrings gameStrings = null)
		{
            return new List<string>(GetPokemonTypes(pokemon, _language, gameStrings));
		}

		public static string GetPokemonNatureInLanguage(PKM pokemon, SystemLanguage _language, GameStrings gameStrings = null)
		{
			return GetNatureName(pokemon.Nature, _language, gameStrings);
		}

		public static string GetPokemonSpeciesNameInLanguage(PKM pokemon, SystemLanguage _language)
        {
            var langIDx = GetSpeciesLanguageListIndex(_language);
            return GetPokemonSpeciesNameInLanguage(pokemon, langIDx);
        }

        public static string GetPokemonSpeciesNameInLanguage(PKM pokemon, LanguageID _language)
        {
            var langIDx = GetSpeciesLanguageListIndex(_language);
            return SpeciesName.GetSpeciesName(pokemon.Species, langIDx);
        }

        public static string GetPokemonSpeciesNameInLanguage(PKM pokemon, int _language)
        {
            return SpeciesName.GetSpeciesName(pokemon.Species, _language);
        }


        public static KeyValuePair<bool, PKM> LoadPKM(byte[] data, string ext)
        {
            if (FileUtil.TryGetPKM(data, out PKM pk, ext))
            {
                return new KeyValuePair<bool, PKM>(true, pk);
            }

            return new KeyValuePair<bool, PKM>(false, null);
        }


        public static SaveFile GetSaveFileData(byte[] data)
        {
            return SaveUtil.GetVariantSAV(data);
        }

        public static GameVersion GetGameVersion(byte[] data)
        {
            var sav = SaveUtil.GetVariantSAV(data);
            return sav.Version;
        }


        public static SaveFile LoadSaveFile (string sav, PokemonGeneration gen = PokemonGeneration.Kanto)
        {
            switch (gen)
            {
                case PokemonGeneration.Kanto:
                    return PKHexGen1SavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.Johto:
                    return PKHexGen2SavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.Hoenn:
                    return PKHexGen3SavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.Sinnoh:
                    return PKHexGenDPSavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.SinnohPt:
                    return PKHexGenPTSavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.HGSS:
                    return PKHexGenHGSSSavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.Tesselia:
                    return PKHexGen5BWSavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.TesseliaBW2:
                    return PKHexGen5BW2SavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.Kalos:
                    return PKHexGen6XYSavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.ORAS:
                    return PKHexGen6ORASSavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.Alola:
                    return PKHexGen7SUMOSavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.USUM:
                    return PKHexGen7USUMSavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.Galar:
                    return PKHexGen8ShSwSavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.KantoPLGPE:
                    return PKHexGen8PLGPESavLoader.LoadSaveFileFromPath(sav);
				case PokemonGeneration.BDSP:
					return PKHexGen8BSSavLoader.LoadSaveFileFromPath(sav);
				case PokemonGeneration.LegendsArceus:
					return PKHexGen8LASavLoader.LoadSaveFileFromPath(sav);
				case PokemonGeneration.ScarletViolet:
					return PKHexGen9SCVISavLoader.LoadSaveFileFromPath(sav);
				case PokemonGeneration.Stadium:
					return PKHexGen1StadiumSavLoader.LoadSaveFileFromPath(sav);
				case PokemonGeneration.Stadium2:
					return PKHexGen2StadiumSavLoader.LoadSaveFileFromPath(sav);
				default:
                    return PKHexGen1SavLoader.LoadSaveFileFromPath(sav);
            }
        }

        public static bool  SaveFileExists(string sav)
        {
            return File.Exists(sav);
        }

        public static PKM GetPokemonInParty(SaveFile savefile, int slot)
        {
            var party = savefile.PartyData;
            return party[slot];
        }

        public static string GetPokemonNameOfPartyInIndex(SaveFile savefile, int slot = 0, PokemonGeneration generation = PokemonGeneration.Kanto)
        {

            var type = typeof(PKHexSavLoader<>).MakeGenericType(savefile.GetType());
            dynamic savLoader = System.Activator.CreateInstance(type);

#if NET_4_6
			return savLoader.GetPokemonNameInPartyInSlot(savefile, slot);
#else
			Debug.LogError("Can't get data from generic savefile because you are not using .NET 4.x. Please, change the framework in Project Settings.");
			return "Pikachu";
#endif
        }

        public static IEnumerable<string> MoveStrings(LanguageID _language)
        {
            return Util.GetMovesList(GetLanguageCode(_language));
        }

        public static IEnumerable<string> MoveStrings(SystemLanguage _language)
        {
            return Util.GetMovesList(GetLanguageCode(_language));
        }

        public static IEnumerable<string> SpeciesStrings(LanguageID _language)
        {
            return Util.GetSpeciesList(GetLanguageCode(_language));
        }

        public static IEnumerable<string> SpeciesStrings(SystemLanguage _language)
		{
            return Util.GetSpeciesList(GetLanguageCode(_language));
        }

        public static string L_AError { get; set; } = "Internal error.";
        public static IReadOnlyList<string> MoveStrings(string _lang = GameLanguage.DefaultLanguage) => Util.GetMovesList(_lang);
        public static IReadOnlyList<string> SpeciesStrings (string _lang = GameLanguage.DefaultLanguage) => Util.GetSpeciesList(_lang);
        public static IEnumerable<string> GetMoveNames(IEnumerable<ushort> moves, string _lang = GameLanguage.DefaultLanguage) => moves.Select(m => m >= MoveStrings(_lang).Count ? LegalityCheckStrings.L_AError : MoveStrings(_lang)[m]);
        public static IEnumerable<string> GetMoveNames(IEnumerable<ushort> moves, SystemLanguage _lang) => moves.Select(m => m >= MoveStrings(GetLanguageCode(_lang)).Count ? LegalityCheckStrings.L_AError : MoveStrings(GetLanguageCode(_lang))[m]);
        public static IEnumerable<string> GetMoveNames(IEnumerable<ushort> moves, LanguageID _lang) => moves.Select(m => m >= MoveStrings(GetLanguageCode(_lang)).Count ? LegalityCheckStrings.L_AError : MoveStrings(GetLanguageCode(_lang))[m]); 
	}
}
