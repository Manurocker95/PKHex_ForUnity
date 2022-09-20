using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHeX.Core;
using System.IO;
using System.Linq;

namespace PKHexForUnity
{
    public class PKHexSavLoader<T> where T : SaveFile
    {

        public T m_saveFile;

        public virtual string PlayerName
        {
            get
            {
                return m_saveFile != null ? m_saveFile.OT : "Trainer";
            }
        }

        public virtual int Badges
        {
            get
            {
                return 0;
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
                return m_saveFile != null ? m_saveFile.TID : 0;
            }
        }

        public virtual int DisplayTID
        {
            get
            {
                return m_saveFile != null ? m_saveFile.DisplayTID : 0;
            }
        }  
        
        public virtual int MaxCoins
        {
            get
            {
                return m_saveFile != null ? m_saveFile.MaxCoins : 0;
            }
        }

        public virtual uint Coins
        {
            get
            {
                return 0;
            }
        }

        public virtual void SetSaveFile(SaveFile saveFile)
        {
            m_saveFile = saveFile as T;
        }

        public virtual bool SaveFileExists(string sav)
        {
            return File.Exists(sav);
        }

        public virtual T LoadSaveFile(string sav)
        {
            return default(T);
        }


        public virtual T0 GetPKM<T0>(T savefile, byte[] data)
        {
            return default(T0);
        }

        public virtual bool HasParty()
        {
            return m_saveFile != null ? m_saveFile.HasParty : false;
        }

        public virtual bool IsSurfingPikachuInPartySaveData(string sav)
        {
            T savefile = LoadSaveFile(sav);
            return IsThereAPokemonInPartyWithMove(savefile, "Surf");
        }

        public virtual bool IsSurfingPikachuInPartySaveData(T savefile)
        {
            return IsThereAPokemonInPartyWithMove(savefile, "Surf");
        }

        public virtual bool IsThereAPokemonInPartyWithMoveInCurrentFile(string move, string _pokemon)
        {
            return m_saveFile != null ? m_saveFile.HasParty && GetPokemonInPartyWithMove(m_saveFile, move, _pokemon) > -1 : false;
        }        
        
        public virtual bool IsThereAPokemonInPartyWithMoveInCurrentFile(string move)
        {
            return m_saveFile != null ? m_saveFile.HasParty && GetPokemonInPartyWithMove(m_saveFile, move) > -1 : false;
        }

        public virtual bool IsThereAPokemonInPartyWithMove(string sav, string move)
        {
            T savefile = LoadSaveFile(sav);
            return savefile.HasParty && GetPokemonInPartyWithMove(savefile, move) > -1;
        }

        public virtual bool IsThereAPokemonInPartyWithMove(T savefile, string move, string _pokemon = "")
        {
            return savefile.HasParty && GetPokemonInPartyWithMove(savefile, move, _pokemon) > -1;
        }


        public virtual int GetPokemonInPartyWithMove(T savefile, string _moveName, string _pokemon = "", SystemLanguage _lang = SystemLanguage.English)
        {
            var party = savefile.PartyData;

            int indexInParty = 0;
            foreach (PKM pokemon in party)
            {
                List<string> moves = new List<string>(GetPokemonMoves(pokemon, _lang));
                if (moves.Contains(_moveName))
                {
                    if (string.IsNullOrEmpty(_pokemon))
                    {
                        Debug.Log($"Yes, party contains the move {_moveName} in pokémon {pokemon.Nickname}  ");
                        return indexInParty;
                    }
                    else
                    {
                        if (PKHexUtils.SpeciesStrings(_lang).Contains(_pokemon))
                        {
                            Debug.Log($"Yes, a {pokemon.Nickname} has {_moveName}");
                            return indexInParty;
                        }
                    }
                }

                indexInParty++;
            }

            return -1;
        }


        public virtual int GetPokemonInPartyWithMoveInCurrentFile(string _moveName, string _pokemon = "", SystemLanguage _lang = SystemLanguage.English)
        {
            if (m_saveFile == null)
                return -1;

            var party = m_saveFile.PartyData;

            int indexInParty = 0;
            foreach (PKM pokemon in party)
            {
                List<string> moves = new List<string>(GetPokemonMoves(pokemon));
                if (moves.Contains(_moveName))
                {
                    if (string.IsNullOrEmpty(_pokemon))
                    {
                        Debug.Log($"Yes, party contains the move {_moveName} in pokémon {pokemon.Nickname}  ");
                        return indexInParty;
                    }
                    else
                    {
                        if (PKHexUtils.SpeciesStrings(_lang).Contains(_pokemon))
                        {
                            Debug.Log($"Yes, a {pokemon.Nickname} has {_moveName}");
                            return indexInParty;
                        }
                    }
                }

                indexInParty++;
            }

            return -1;
        }

        public virtual int GetLevelOfPokemonInPartyInSlotInCurrentFile(int slot)
        {
            return GetPokemonInParty(m_saveFile, slot).CurrentLevel;
        }

        public virtual string GetPokemonNameInPartyInSlotInCurrentFile(int slot)
        {
            return GetPokemonInParty(m_saveFile, slot).Nickname;
        }


        public virtual string GetPokemonNameOfPartyIndexInCurrentFile(int slot)
        {
            return GetPokemonInParty(m_saveFile, slot).Nickname;
        }

        public virtual PKM GetPokemonInPartyInCurrentFile(int slot)
        {
            var party = m_saveFile.PartyData;
            return party[slot];
        }

        public virtual int GetLevelOfPokemonInPartyInSlot(T savefile, int slot)
        {
            return GetPokemonInParty(savefile, slot).CurrentLevel;
        }

        public virtual string GetPokemonNameInPartyInSlot(T savefile, int slot)
        {
            return GetPokemonInParty(savefile, slot).Nickname;
        }


        public virtual string GetPokemonNameOfPartyIndex(T savefile, int slot)
        {
            return GetPokemonInParty(savefile, slot).Nickname;
        }

        public virtual System.Type PKMNType
        {
            get
            {
                return m_saveFile.PKMType;
            }
            
        }

        public virtual string GetPokemonSpeciesName(PKM pokemon, T saveFile)
        {
            return SpeciesName.GetSpeciesName(pokemon.Species, saveFile.Language);
        }

        public virtual string GetPokemonSpeciesNameInCurrentFile(PKM pokemon)
        {
            return SpeciesName.GetSpeciesName(pokemon.Species, m_saveFile.Language);
        }

        public virtual string GetPokemonSpeciesNameInLanguage(PKM pokemon, SystemLanguage _language)
        {
            return SpeciesName.GetSpeciesName(pokemon.Species, PKHexUtils.GetLanguageCodeIndex(_language));
        }

        public virtual string GetPokemonSpeciesNameInLanguage(PKM pokemon, LanguageID _language)
        {
            return SpeciesName.GetSpeciesName(pokemon.Species, PKHexUtils.GetLanguageCodeIndex(_language));
        }

        public virtual string GetPokemonSpeciesNameInLanguage(PKM pokemon, int _language)
        {
            return SpeciesName.GetSpeciesName(pokemon.Species, _language);
        }

        public virtual KeyValuePair<bool, PKM> LoadPKMFromPath(string file)
        {
            if (SaveFileExists(file))
            {
                return LoadPKM(File.ReadAllBytes(file));
            }

            return new KeyValuePair<bool, PKM>(false, null);
        }

        public virtual KeyValuePair<bool, PKM> LoadPKM(byte[] data)
        {
            if (FileUtil.TryGetPKM(data, out PKM pk, "." + PKMNType))
            {
                return new KeyValuePair<bool, PKM>(true, pk);
            }

            return new KeyValuePair<bool, PKM>(false, null);
        }

        public virtual PKM GetPokemonInParty(T savefile, int slot)
        {
            var party = savefile.PartyData;
            return party[slot];
        }        
        
        public virtual PKM GetPokemonInSlot(T savefile, int slot)
        {
            var party = savefile.PartyData;
            return party[slot];
        }

        public virtual IEnumerable<string> GetPokemonMoves(PKM pokemon, SystemLanguage _language = SystemLanguage.English)
        {
            return pokemon != null ? PKHexUtils.GetMoveNames(pokemon.Moves, _language) : new string[1] { PKHexUtils.L_AError };
        } 
        
        public virtual int GetPokemonLevel(PKM pokemon)
        {
            return pokemon != null ? pokemon.CurrentLevel : -1;
        }

        public virtual IEnumerable<string> GetPartyPokemonMoves(T savFile, int slot)
        {
            return GetPokemonMoves(GetPokemonInParty(savFile, slot));
        }
        
        public virtual IEnumerable<string> GetPartyPokemonMovesInCurrentFile(int slot)
        {
            return GetPokemonMoves(GetPokemonInPartyInCurrentFile(slot));
        }
    }

}
