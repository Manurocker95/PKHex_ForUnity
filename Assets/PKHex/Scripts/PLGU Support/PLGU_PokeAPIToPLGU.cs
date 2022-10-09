using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHexForUnity.PokeAPI;

namespace PokemonLetsGoUnity
{
    public static class PLGU_PokeAPIToPLGU 
    {
        public static PLGU_PKHexPokemonData GetDataFromPokeAPI(PokeAPIPokemonData _originalData)
        {
            PLGU_PKHexPokemonData data = new PLGU_PKHexPokemonData();

            data.m_species = _originalData.name;

            return data;
        }
    }
}
