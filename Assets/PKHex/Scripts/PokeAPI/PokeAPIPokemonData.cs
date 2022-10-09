using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PKHexForUnity.PokeAPI
{
    [System.Serializable]
    public class PokeAPIPokemonData
    {
        public int? base_happiness = null;
        public int capture_rate = 3;
        public Color color;
        public List<EggGroup> egg_groups;
        public EvolutionChain evolution_chain;
        public EvolutionFromSpecies evolves_from_species;
        public List<FlavourData> flavor_text_entries;

        public bool forms_switchable;
        public int gender_rate;
        public List<PokemonNameData> genera;
        public Generation generation;
        public GrowthRate growth_rate;
        public Habitat habitat;
        public bool has_gender_differences;
        public int? hatch_counter = null;
        public int id;
        public bool is_baby;
        public bool is_legendary;
        public bool is_mythical;
        public string name;
        public List<NameData> names;
        public int order;
        public List<PalParkEncounters> pal_park_encounters;
        public List<PokedexNumbers> pokedex_numbers;
        public ShapeData shape;
        public List<VarietyData> varieties;

        [System.Serializable]
        public class PokemonFormData
        {
            public string name;
            public string url;
        }

        [System.Serializable]
        public class VarietyData
        {
            public bool is_default;
            public PokemonFormData pokemon;
        }

        [System.Serializable]
        public class ShapeData
        {
            public string name;
            public string url;
        }

        [System.Serializable]
        public class Pokedexdata
        {
            public string name;
            public string url;
        }

        [System.Serializable]
        public class PokedexNumbers
        {
            public int entry_number;
            public Pokedexdata pokedex;
        }

        [System.Serializable]
        public class PalParkAreaData
        {
            public string name;
            public string url;
        }


        [System.Serializable]
        public class PalParkEncounters
        {
            public PalParkAreaData area;
            public int base_score;
            public int rate;
        }

        [System.Serializable]
        public class NameData
        {
            public Language language;
            public string name;
        }

        [System.Serializable]
        public class Habitat
        {
            public string name;
            public string url;
        }

        [System.Serializable]
        public class GrowthRate
        {
            public string name;
            public string url;
        }


        [System.Serializable]
        public class Generation
        {
            public string name;
            public string url;
        }

        [System.Serializable]
        public class PokemonNameData
        {
            public string genus;
            public Language language;
        }


        [System.Serializable]
        public class FlavourData
        {
            public string flavor_text;
            public Language language;
            public FlavourVersion version;
        }

        [System.Serializable]
        public class Language
        {
            public string name;
            public string url;
        }
        
        [System.Serializable]
        public class FlavourVersion
        {
            public string name;
            public string url;
        }

        [System.Serializable]
        public class Color
        {
            public string name;
            public string url;
        }

        [System.Serializable]
        public class EggGroup
        {
            public string name;
            public string url;
        }

        [System.Serializable]
        public class EvolutionChain
        {
            public string url;
        }  
        
        [System.Serializable]
        public class EvolutionFromSpecies
        {
            public string name;
            public string url;
        }
    }
}
