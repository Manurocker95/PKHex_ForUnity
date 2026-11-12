using System.Collections.Generic;
using System.IO;
using PKHeX.Core;
using UnityEngine;

namespace PKHexForUnity
{
    /// <summary>
    /// Non-generic access to any save loader. Replaces the previous 'dynamic'/MakeGenericType usage,
    /// which is not available on Unity 5.6 (.NET 3.5) nor safe under AOT/IL2CPP.
    /// </summary>
    public interface IPKHexSavLoader
    {
        SaveFile CurrentSaveFile { get; }
        string PlayerName { get; }
        int Badges { get; }
        int MaxMoney { get; }
        uint Money { get; }
        int Gender { get; }
        int PlayedHours { get; }
        int PlayedMinutes { get; }
        int PlayedSeconds { get; }
        int TID { get; }
        int DisplayTID { get; }
        uint Coins { get; }

        void SetSaveFile(SaveFile _saveFile);
        bool HasParty();
        PKM[] GetParty();
        PKM[] GetBoxPokemon(bool _skipEmpty);
        string GetSummary(SystemLanguage _language);
        RuntimeEntityData GetPokemonData(PKM _pokemon, SystemLanguage _language, bool _checkLegality);
        string GetPokemonNameInPartyInSlotInCurrentFile(int _slot);
        IEnumerable<string> GetPartyPokemonMovesInCurrentFile(int _slot);
    }

    /// <summary>
    /// Base save loader. Works for every game PKHeX supports; derived loaders only add game specific helpers.
    /// </summary>
    public class PKHexSavLoader<T> : IPKHexSavLoader where T : SaveFile
    {
        public T m_saveFile;

        public SaveFile CurrentSaveFile
        {
            get
            {
                return m_saveFile;
            }
        }

        public virtual string PlayerName
        {
            get
            {
                return m_saveFile != null ? m_saveFile.OT : "Trainer";
            }
        }

        /// <summary> Number of obtained badges (0 when the game has none or PKHeX does not expose them). </summary>
        public virtual int Badges
        {
            get
            {
                return m_saveFile != null ? Mathf.Max(0, RuntimeSaveData.GetBadgeCount(m_saveFile)) : 0;
            }
        }

        public virtual int MaxMoney
        {
            get
            {
                return m_saveFile != null ? m_saveFile.MaxMoney : 0;
            }
        }

        public virtual uint Money
        {
            get
            {
                return m_saveFile != null ? m_saveFile.Money : 0;
            }
        }

        public virtual int Gender
        {
            get
            {
                return m_saveFile != null ? m_saveFile.Gender : 0;
            }
        }

        public virtual int PlayedHours
        {
            get
            {
                return m_saveFile != null ? m_saveFile.PlayedHours : 0;
            }
        }

        public virtual int PlayedMinutes
        {
            get
            {
                return m_saveFile != null ? m_saveFile.PlayedMinutes : 0;
            }
        }

        public virtual int PlayedSeconds
        {
            get
            {
                return m_saveFile != null ? m_saveFile.PlayedSeconds : 0;
            }
        }

        public virtual int TID
        {
            get
            {
                return m_saveFile != null ? m_saveFile.TID16 : 0;
            }
        }

        public virtual int DisplayTID
        {
            get
            {
                return m_saveFile != null ? (int)m_saveFile.DisplayTID : 0;
            }
        }

        public virtual int MaxCoins
        {
            get
            {
                return m_saveFile != null ? m_saveFile.MaxCoins : 0;
            }
        }

        /// <summary> Game Corner coins (Gen 1-4); 0 for games without coins. </summary>
        public virtual uint Coins
        {
            get
            {
                SaveFile sav = m_saveFile;
                if (sav is SAV1)
                {
                    return ((SAV1)sav).Coin;
                }
                if (sav is SAV2)
                {
                    return ((SAV2)sav).Coin;
                }
                if (sav is SAV3)
                {
                    return ((SAV3)sav).Coin;
                }
                if (sav is SAV4)
                {
                    return ((SAV4)sav).Coin;
                }
                return 0;
            }
        }

        public virtual void SetSaveFile(SaveFile _saveFile)
        {
            m_saveFile = _saveFile as T;
            if (_saveFile != null && m_saveFile == null)
            {
                Debug.LogError("[PKHex] " + _saveFile.GetType().Name + " cannot be used by a loader of " + typeof(T).Name + ".");
            }
        }

        public virtual bool SaveFileExists(string _sav)
        {
            return File.Exists(_sav);
        }

        /// <summary> Loads (and auto-detects) a save file; null if missing, unsupported or of another game. </summary>
        public virtual T LoadSaveFile(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<T>(_sav);
        }

        /// <summary> Loads the save file and keeps it as the current file. </summary>
        public virtual bool LoadCurrentSaveFile(string _sav)
        {
            m_saveFile = LoadSaveFile(_sav);
            return m_saveFile != null;
        }

        public virtual T0 GetPKM<T0>(T _savefile, byte[] _data)
        {
            return default(T0);
        }

        public virtual bool HasParty()
        {
            return m_saveFile != null && m_saveFile.HasParty;
        }

        public virtual PKM[] GetParty()
        {
            return m_saveFile != null ? RuntimeSaveData.GetParty(m_saveFile) : new PKM[0];
        }

        public virtual PKM[] GetBoxPokemon(bool _skipEmpty)
        {
            return m_saveFile != null ? RuntimeSaveData.GetBoxPokemon(m_saveFile, _skipEmpty) : new PKM[0];
        }

        public virtual string GetSummary(SystemLanguage _language)
        {
            return m_saveFile != null ? RuntimeSaveData.GetSummary(m_saveFile, PKHexUtils.GetLanguageCode(_language)) : "No save file loaded.";
        }

        public virtual RuntimeEntityData GetPokemonData(PKM _pokemon, SystemLanguage _language, bool _checkLegality)
        {
            return RuntimeEntityData.FromPKM(_pokemon, PKHexUtils.GetLanguageCode(_language), _checkLegality);
        }

        public virtual bool IsSurfingPikachuInPartySaveData(string _sav)
        {
            T savefile = LoadSaveFile(_sav);
            return savefile != null && IsThereAPokemonInPartyWithMove(savefile, "Surf", "Pikachu");
        }

        public virtual bool IsSurfingPikachuInPartySaveData(T _savefile)
        {
            return IsThereAPokemonInPartyWithMove(_savefile, "Surf", "Pikachu");
        }

        public virtual bool IsThereAPokemonInPartyWithMoveInCurrentFile(string _move, string _pokemon)
        {
            return m_saveFile != null && m_saveFile.HasParty && GetPokemonInPartyWithMove(m_saveFile, _move, _pokemon) > -1;
        }

        public virtual bool IsThereAPokemonInPartyWithMoveInCurrentFile(string _move)
        {
            return IsThereAPokemonInPartyWithMoveInCurrentFile(_move, string.Empty);
        }

        public virtual bool IsThereAPokemonInPartyWithMove(string _sav, string _move)
        {
            T savefile = LoadSaveFile(_sav);
            return savefile != null && savefile.HasParty && GetPokemonInPartyWithMove(savefile, _move) > -1;
        }

        public virtual bool IsThereAPokemonInPartyWithMove(T _savefile, string _move, string _pokemon = "")
        {
            return _savefile != null && _savefile.HasParty && GetPokemonInPartyWithMove(_savefile, _move, _pokemon) > -1;
        }

        /// <summary>
        /// Index of the first party member that knows <paramref name="_moveName"/> (and, if given, is of species
        /// <paramref name="_pokemon"/>), or -1. Names are compared in <paramref name="_lang"/>.
        /// </summary>
        public virtual int GetPokemonInPartyWithMove(T _savefile, string _moveName, string _pokemon = "", SystemLanguage _lang = SystemLanguage.English)
        {
            if (_savefile == null || !_savefile.HasParty)
            {
                return -1;
            }

            PKM[] party = RuntimeSaveData.GetParty(_savefile);
            for (int i = 0; i < party.Length; i++)
            {
                PKM pokemon = party[i];
                List<string> moves = new List<string>(GetPokemonMoves(pokemon, _lang));
                if (!moves.Contains(_moveName))
                {
                    continue;
                }
                if (string.IsNullOrEmpty(_pokemon))
                {
                    Debug.Log("[PKHex] Party contains the move " + _moveName + " in " + pokemon.Nickname);
                    return i;
                }
                if (string.Equals(PKHexUtils.GetSpeciesFromID(pokemon.Species, _lang), _pokemon, System.StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log("[PKHex] " + pokemon.Nickname + " has " + _moveName);
                    return i;
                }
            }
            return -1;
        }

        public virtual int GetPokemonInPartyWithMoveInCurrentFile(string _moveName, string _pokemon = "", SystemLanguage _lang = SystemLanguage.English)
        {
            return GetPokemonInPartyWithMove(m_saveFile, _moveName, _pokemon, _lang);
        }

        public virtual int GetLevelOfPokemonInPartyInSlotInCurrentFile(int _slot)
        {
            return GetPokemonInParty(m_saveFile, _slot).CurrentLevel;
        }

        public virtual string GetPokemonNameInPartyInSlotInCurrentFile(int _slot)
        {
            return GetPokemonInParty(m_saveFile, _slot).Nickname;
        }

        public virtual string GetPokemonNameOfPartyIndexInCurrentFile(int _slot)
        {
            return GetPokemonInParty(m_saveFile, _slot).Nickname;
        }

        public virtual PKM GetPokemonInPartyInCurrentFile(int _slot)
        {
            return GetPokemonInParty(m_saveFile, _slot);
        }

        public virtual int GetLevelOfPokemonInPartyInSlot(T _savefile, int _slot)
        {
            return GetPokemonInParty(_savefile, _slot).CurrentLevel;
        }

        public virtual string GetPokemonNameInPartyInSlot(T _savefile, int _slot)
        {
            return GetPokemonInParty(_savefile, _slot).Nickname;
        }

        public virtual string GetPokemonNameOfPartyIndex(T _savefile, int _slot)
        {
            return GetPokemonInParty(_savefile, _slot).Nickname;
        }

        public virtual System.Type PKMNType
        {
            get
            {
                return m_saveFile != null ? m_saveFile.PKMType : typeof(PKM);
            }
        }

        public virtual string GetPokemonSpeciesName(PKM _pokemon, T _saveFile)
        {
            return SpeciesName.GetSpeciesName(_pokemon.Species, PKHexUtils.GetSafeLanguage(_saveFile));
        }

        public virtual string GetPokemonSpeciesNameInCurrentFile(PKM _pokemon)
        {
            return SpeciesName.GetSpeciesName(_pokemon.Species, PKHexUtils.GetSafeLanguage(m_saveFile));
        }

        public virtual string GetPokemonSpeciesNameInLanguage(PKM _pokemon, SystemLanguage _language)
        {
            return PKHexUtils.GetPokemonSpeciesNameInLanguage(_pokemon, _language);
        }

        public virtual string GetPokemonSpeciesNameInLanguage(PKM _pokemon, LanguageID _language)
        {
            return PKHexUtils.GetPokemonSpeciesNameInLanguage(_pokemon, _language);
        }

        public virtual string GetPokemonSpeciesNameInLanguage(PKM _pokemon, int _language)
        {
            return SpeciesName.GetSpeciesName(_pokemon.Species, _language);
        }

        public virtual KeyValuePair<bool, PKM> LoadPKMFromPath(string _file)
        {
            if (SaveFileExists(_file))
            {
                return PKHexUtils.LoadPKM(File.ReadAllBytes(_file), Path.GetExtension(_file));
            }
            return new KeyValuePair<bool, PKM>(false, null);
        }

        /// <summary> Loads an entity file using this save's entity format as the preferred format. </summary>
        public virtual KeyValuePair<bool, PKM> LoadPKM(byte[] _data)
        {
            return PKHexUtils.LoadPKM(_data, "." + PKMNType.Name.ToLowerInvariant());
        }

        public virtual PKM GetPokemonInParty(T _savefile, int _slot)
        {
            return _savefile.GetPartySlotAtIndex(_slot);
        }

        public virtual PKM GetPokemonInSlot(T _savefile, int _slot)
        {
            return _savefile.GetPartySlotAtIndex(_slot);
        }

        public virtual IEnumerable<string> GetPokemonMoves(PKM _pokemon, SystemLanguage _language = SystemLanguage.English)
        {
            return _pokemon != null ? PKHexUtils.GetMoveNames(_pokemon.Moves, _language) : new string[] { PKHexUtils.L_AError };
        }

        public virtual int GetPokemonLevel(PKM _pokemon)
        {
            return _pokemon != null ? _pokemon.CurrentLevel : -1;
        }

        public virtual IEnumerable<string> GetPartyPokemonMoves(T _savFile, int _slot)
        {
            return GetPokemonMoves(GetPokemonInParty(_savFile, _slot));
        }

        public virtual IEnumerable<string> GetPartyPokemonMovesInCurrentFile(int _slot)
        {
            return GetPokemonMoves(GetPokemonInPartyInCurrentFile(_slot));
        }
    }
}
