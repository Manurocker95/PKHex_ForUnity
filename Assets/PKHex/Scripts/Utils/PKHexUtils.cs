using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using PKHeX.Core;
using System.Linq;
using PokemonLetsGoUnity;

namespace PKHexForUnity
{
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

		public static List<string> GetFullDexNames()
		{
			return new List<string>(GameInfo.Strings.specieslist);
		}

		public static int GetDexNumber(string _species)
		{
			return GetFullDexNames().IndexOf(_species);
		}

		public static int GetSpeciesInt(string _species)
		{
			return GetDexID(_species);
		}

		public static PKM GetPokemonFromSpecies(string _species, PokemonGeneration generation)
        {
			switch (generation)
            {
				case PokemonGeneration.Kanto:
				case PokemonGeneration.Stadium:
					PK1 pkmn = new PK1() { Species = GetSpeciesInt(_species) };
					return pkmn;
				case PokemonGeneration.Johto:
				case PokemonGeneration.Stadium2:
					PK2 pkmn2 = new PK2() { Species = GetSpeciesInt(_species) };
					return pkmn2;
				case PokemonGeneration.Hoenn:
				case PokemonGeneration.Colosseum:
				case PokemonGeneration.XD:
				case PokemonGeneration.Box:
					PK3 pkmn3 = new PK3() { Species = GetSpeciesInt(_species) };
					return pkmn3;
				case PokemonGeneration.Sinnoh:
				case PokemonGeneration.SinnohPt:
				case PokemonGeneration.HGSS:
					PK4 pkmn4 = new PK4() { Species = GetSpeciesInt(_species) };
					return pkmn4;
				case PokemonGeneration.Tesselia:
				case PokemonGeneration.TesseliaBW2:
					PK5 pkmn5 = new PK5() { Species = GetSpeciesInt(_species) };
					return pkmn5;
				case PokemonGeneration.Kalos:
				case PokemonGeneration.ORAS:
					PK6 pkmn6 = new PK6() { Species = GetSpeciesInt(_species) };
					return pkmn6;
				case PokemonGeneration.Alola:
				case PokemonGeneration.USUM:
					PK7 pkmn7 = new PK7() { Species = GetSpeciesInt(_species) };
					return pkmn7;
				case PokemonGeneration.Galar:
				case PokemonGeneration.BDSP:
				case PokemonGeneration.LegendsArceus:
					PK8 pkmn8 = new PK8() { Species = GetSpeciesInt(_species) };
					return pkmn8;

				default:
					PK8 pkmndef = new PK8() { Species = GetSpeciesInt(_species) };
					return pkmndef;
			}
        }

		public static IEnumerable<string> GetPokemonTypesOfSpecies(string _species, PokemonGeneration _gen = PokemonGeneration.LegendsArceus)
		{
			PK8 pkmn = new PK8() { Species = GetSpeciesInt(_species) };
			return GetPokemonTypes(pkmn);
		}

		public static IEnumerable<string> GetPokemonHiddenAbilities(PKM _pkm)
		{
			var abilities = new List<string>(GetPokemonAbilities(_pkm));

			if (abilities.Count == 0)
				return new List<string>() { GetAbilities().ElementAt(0) };

			return new List<string>() { abilities[abilities.Count - 1] };
		}

		public static IEnumerable<string> GetPokemonAbilities(PKM _pkm)
        {
			var personalInfo = _pkm.PersonalInfo;
			List<string> abilities = new List<string>(GetAbilities());
			List<string> pkmnAbilities = new List<string>();

			foreach (var ab in personalInfo.Abilities)
            {
				pkmnAbilities.Add(abilities[ab]);
			}
			return pkmnAbilities;
		}

		public static IEnumerable<string> GetPokemonTypes(PKM _pkm)
		{
			var personalInfo = _pkm.PersonalInfo;
			int type1 = personalInfo.Type1;
			int type2 = personalInfo.Type2;
			
			var types = GameInfo.Strings.Types;

			List<string> pkmnTypes = new List<string>();
			pkmnTypes.Add(types.ElementAt(type1));

			if (type2 >= 0)
				pkmnTypes.Add(types.ElementAt(type2));

			return pkmnTypes;
		}

		public static int GetDexID(string _species, int _language = 2)
		{
			return SpeciesName.GetSpeciesID(_species, _language);
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

		public static PLGU_PKHexPokemonData ConvertPKMToBattleData(PKHeX.Core.PKM _pkm)
		{
			List<string> mvs = new List<string>();
			foreach (string st in GetMoveNames(_pkm.Moves))
			{
				if (!st.Contains("—"))
					mvs.Add(st);
			}

			List<string> eggmvs = new List<string>();
			foreach (string st in GetMoveNames(_pkm.RelearnMoves))
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
				TID = _pkm.TrainerID7,
				STID = _pkm.TrainerSID7,
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

		public static IEnumerable<string> GetCharacteristincsNames()
		{
			return GameInfo.Strings.characteristics;
		}

		public static IEnumerable<string> GetStatusNames()
		{
			return GameInfo.Strings.characteristics;
		}
		
		public static IEnumerable<string> GetNatureNames()
		{
			return GameInfo.Strings.Natures;
		}
		public static IEnumerable<string> GetBallNames()
		{
			return GameInfo.Strings.balllist;
		}
		
		public static IEnumerable<string> GetAbilities()
		{
			return GameInfo.Strings.Ability;
		}
		
		public static IEnumerable<string> GetItemNames(int _generation, PKHeX.Core.GameVersion _version = GameVersion.Any)
		{
			return GameInfo.Strings.GetItemStrings(_generation, _version);
		}

		public static IEnumerable<string> GetItemNames(SaveFile _sav)
		{
			return GameInfo.Strings.GetItemStrings(_sav.Generation);
		}

		public static string GetItemName(SaveFile _sav, int _item)
		{
			return _item > -1 ? GetItemNames(_sav).ElementAt(_item) : "NONE";
		}
		
		public static string GetMetLocation(bool _isEggLocation, int location, int _format, int _generation, GameVersion _version = GameVersion.Any)
		{
			return GameInfo.Strings.GetLocationName(_isEggLocation, location, _format, _generation, _version);
		}
		
		public static string GetMetLocation(bool _isEggLocation, int location, int _format, int _generation, int _version)
		{
			return GameInfo.Strings.GetLocationName(_isEggLocation, location, _format, _generation, (GameVersion)_version);
		}
		
		public static string GetStatusName(int _status)
		{
			return GetStatusNames().ElementAt(_status);
		}
		
		public static string GetBallName(int _ball)
		{
			return GetBallNames().ElementAt(_ball);
		}
		public static string GetAbilityName(int _ability)
		{
			return GetAbilities().ElementAt(_ability);
		}
		
		public static string GetNatureName(int _nature)
		{
			return GetNatureNames().ElementAt(_nature);
		}
		
		public static string GetItemName(int _item, int _generation, PKHeX.Core.GameVersion _version = GameVersion.Any)
		{
			return _item > -1 ? GetItemNames(_generation, _version).ElementAt(_item) : "NONE";
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

		public static IEnumerable<string> GetPokemonTypeList(LanguageID _language = LanguageID.English)
		{
			return GameInfo.Strings.Types;
		}

		public static List<string> GetPokemonTypesInLanguage(PKM pokemon, SystemLanguage _language)
		{
			string lang = _language.ToString();
			LanguageID l = LanguageID.English;
			System.Enum.TryParse(lang, out l);

			return new List<string>(GetPokemonTypes(pokemon));
		}

		public static string GetPokemonNatureInLanguage(PKM pokemon, SystemLanguage _language)
		{

			string lang = _language.ToString();
			LanguageID l = LanguageID.English;
			System.Enum.TryParse(lang, out l);

			return GetNatureName(pokemon.Nature);
		}

		public static string GetPokemonSpeciesNameInLanguage(PKM pokemon, SystemLanguage _language)
        {

            string lang = _language.ToString();
            LanguageID l = LanguageID.English;
            System.Enum.TryParse(lang, out l);

            return SpeciesName.GetSpeciesName(pokemon.Species, (int)l);
        }

        public static string GetPokemonSpeciesNameInLanguage(PKM pokemon, LanguageID _language)
        {
            return SpeciesName.GetSpeciesName(pokemon.Species, (int)_language);
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
					return PKHexGen8LASavLoader.LoadSaveFileFromPath(sav);
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

        public static string L_AError { get; set; } = "Internal error.";
        public static string[] MoveStrings { internal get; set; } = Util.GetMovesList(GameLanguage.DefaultLanguage);
        public static IEnumerable<string> GetMoveNames(IEnumerable<int> moves) => moves.Select(m => (uint)m >= MoveStrings.Length ? L_AError : MoveStrings[m]);
        public static string[] SpeciesStrings { internal get; set; } = Util.GetSpeciesList(GameLanguage.DefaultLanguage);
    }
}
