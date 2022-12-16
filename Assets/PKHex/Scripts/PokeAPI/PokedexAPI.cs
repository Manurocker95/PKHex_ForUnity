using PKHexForUnity.PokeAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PKHexForUnity.PokeAPI
{
    [System.Serializable]
    public class PokedexAPI
    {
        public string Version; 
        public string PokemonGeneration; 
        public int TotalPokemon; 
        public List<PokeAPIPokemonData> Pokemon = new List<PokeAPIPokemonData>();
    }
}
