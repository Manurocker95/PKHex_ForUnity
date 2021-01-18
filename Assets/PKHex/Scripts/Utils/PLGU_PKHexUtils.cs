using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using PKHeX.Core;
using System.Linq;

namespace PokemonLetsGoUnity
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
    }

    public static class PLGU_PKHexUtils
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
				mvs.Add(st);
			}

			List<string> eggmvs = new List<string>();
			foreach (string st in GetMoveNames(_pkm.RelearnMoves))
			{
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
					_pkm.MarkCircle == 1 ? true : false,
					_pkm.MarkDiamond == 1 ? true : false,
					_pkm.MarkHeart == 1 ? true : false,
					_pkm.MarkSquare == 1 ? true : false,
					_pkm.MarkStar == 1 ? true : false,
					_pkm.MarkTriangle == 1 ? true : false,
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

                var type = typeof(PLGU_PKHexSavLoader<>).MakeGenericType(savefile.GetType());
                dynamic savLoader = System.Activator.CreateInstance(type);
                savLoader.SetSaveFile(savefile);
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
                    return PLGU_PKHexGen1SavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.Johto:
                    return PLGU_PKHexGen2SavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.Hoenn:
                    return PLGU_PKHexGen3SavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.Sinnoh:
                    return PLGU_PKHexGenDPSavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.SinnohPt:
                    return PLGU_PKHexGenPTSavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.HGSS:
                    return PLGU_PKHexGenHGSSSavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.Tesselia:
                    return PLGU_PKHexGen5BWSavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.TesseliaBW2:
                    return PLGU_PKHexGen5BW2SavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.Kalos:
                    return PLGU_PKHexGen6XYSavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.ORAS:
                    return PLGU_PKHexGen6ORASSavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.Alola:
                    return PLGU_PKHexGen7SUMOSavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.USUM:
                    return PLGU_PKHexGen7USUMSavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.Galar:
                    return PLGU_PKHexGen8ShSwSavLoader.LoadSaveFileFromPath(sav);
                case PokemonGeneration.KantoPLGPE:
                    return PLGU_PKHexGen8PLGPESavLoader.LoadSaveFileFromPath(sav);
                default:
                    return PLGU_PKHexGen1SavLoader.LoadSaveFileFromPath(sav);
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

            var type = typeof(PLGU_PKHexSavLoader<>).MakeGenericType(savefile.GetType());
            dynamic savLoader = System.Activator.CreateInstance(type);

            return savLoader.GetPokemonNameInPartyInSlot(savefile, slot);
        }

        public static string L_AError { get; set; } = "Internal error.";
        public static string[] MoveStrings { internal get; set; } = Util.GetMovesList(GameLanguage.DefaultLanguage);
        public static IEnumerable<string> GetMoveNames(IEnumerable<int> moves) => moves.Select(m => (uint)m >= MoveStrings.Length ? L_AError : MoveStrings[m]);
        public static string[] SpeciesStrings { internal get; set; } = Util.GetSpeciesList(GameLanguage.DefaultLanguage);
    }
}
