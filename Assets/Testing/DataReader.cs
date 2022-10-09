#if PKHEX_FOR_UNITY
using PKHeX.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHexForUnity.PokeAPI;

namespace PKHexForUnity
{
    public class DataReader : MonoBehaviour
    {

        public struct DataReaderPokemon
        {
            public string ID;
            public string Species;
            public int PokemonDexNumber;
            public string PokemonDex;
        }

        public int PokemonIndex = 1;
        public LanguageID Language = LanguageID.English;

        public PokeAPILoader PokeAPILoader;

        protected DataReaderPokemon m_currentMonster;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            GenerateMonster();
        }

#if ENABLE_LEGACY_INPUT_MANAGER
        protected virtual void Update()
        {

            if (Input.GetKeyDown(KeyCode.G))
            {
                GenerateMonster();
            }
        }
#endif

        public virtual void GenerateMonster()
        {
            m_currentMonster = GetPokemonData();

            Debug.Log(string.Format("ID: {0} - Species: {1} - PokemonDexNumber: {2} - PokemonDex: {3}", 
                m_currentMonster.ID, 
                m_currentMonster.Species, 
                m_currentMonster.PokemonDexNumber,
                m_currentMonster.PokemonDex));
        }

        public virtual void ShowFullDexList()
        {
            var names = PKHexUtils.GetFullDexNames(Language);
            int idx = 0;
            foreach (string name in names)
            {
                string dexIdx = "" + idx;
                idx++;
                Debug.Log(dexIdx + " - " + name);
            }
        }

        public virtual DataReaderPokemon GetPokemonData()
        {
            var species = PKHexUtils.GetSpeciesFromID(PokemonIndex, Language);
            var dexNumber = PKHexUtils.GetDexNumber(species, Language);
            var dex = PKHexUtils.GetDexEntry(species, Language);
            var id = species.ToUpper();
           
            DataReaderPokemon pkmn = new DataReaderPokemon()
            {
                ID = id,
                Species = species,
                PokemonDexNumber = dexNumber,
                PokemonDex = dex
            };

            return pkmn;
        } 
    }
}
#endif