using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace PKHexForUnity.PokeAPI
{
    public enum PokemonVersions
    {
        red,
        blue,
        yellow,
        gold,
        silver,
        crystal,
        ruby,
        sapphire,
        emerald,
        diamond,
        pearl,
        platinum,
        heartgold,
        soulsilver,
        black,
        white,
        black2,
        white2,
        x,
        y,
        omegaruby,
        alphasapphire,
        sun,
        moon,
        ultrasun,
        ultramoon,
        sword,
        shield,
        brilliantdiamond,
        shiningpearl,
        legendsarceus
    }

    public class PokeAPILoader : MonoBehaviour
    {
        public PokemonVersions Version = PokemonVersions.red;

        [SerializeField] protected PokedexAPI m_pokedex;

        public virtual string GetGameID(PokemonVersions generation)
        {
            switch (generation)
            {
                case PokemonVersions.black2:
                    return "black-2";
                case PokemonVersions.white2:
                    return "white-2";                
                case PokemonVersions.omegaruby:
                    return "omega-ruby";
                case PokemonVersions.alphasapphire:
                    return "alpha-sapphire";
                case PokemonVersions.ultramoon:
                    return "ultra-moon";
                case PokemonVersions.ultrasun:
                    return "ultra-sun";
                case PokemonVersions.brilliantdiamond:
                    return "brilliant-diamond";
                case PokemonVersions.shiningpearl:
                    return "shining-pearl";
                case PokemonVersions.legendsarceus:
                    return "legends-arceus";
            }

            return generation.ToString();
        }

        public virtual List<string> GetGameID(PokemonGeneration generation)
        {
            switch (generation)
            {
                case PokemonGeneration.Kanto:
                case PokemonGeneration.Stadium:
                    return new List<string>() { "red", "blue", "yellow"};
                case PokemonGeneration.Johto:
                case PokemonGeneration.Stadium2:
                    return new List<string>() { "gold", "silver", "crystal" };
                case PokemonGeneration.Hoenn:
                case PokemonGeneration.Colosseum:
                case PokemonGeneration.XD:
                case PokemonGeneration.Box:
                    return new List<string>() { "ruby", "sapphire", "emerald" };
                case PokemonGeneration.Sinnoh:
                case PokemonGeneration.SinnohPt:
                    return new List<string>() { "diamond", "pearl", "platinum" };
                case PokemonGeneration.HGSS:
                    return new List<string>() { "heartgold", "soulsilver" };
                case PokemonGeneration.Tesselia:
                    return new List<string>() { "black", "white" };
                case PokemonGeneration.TesseliaBW2:
                    return new List<string>() { "black-2", "white-2" };
                case PokemonGeneration.Kalos:
                    return new List<string>() { "x", "y" };
                case PokemonGeneration.ORAS:
                    return new List<string>() { "omega-ruby", "alpha-sapphire" };
                case PokemonGeneration.Alola:
                    return new List<string>() { "sun", "moon" };
                case PokemonGeneration.USUM:
                    return new List<string>() { "ultra-sun", "ultra-moon" };
                case PokemonGeneration.Galar:
                    return new List<string>() { "sword", "shield" };
                case PokemonGeneration.BDSP:
                    return new List<string>() { "brilliant-diamond", "shining-pearl" };
                case PokemonGeneration.LegendsArceus:
                    return new List<string>() { "legends-arceus" };
                default:
                    return new List<string>() { "red", "blue", "yellow" };
            }
        }

        public virtual void GetDexDescription(int _speciesNumber, string _generationID, SystemLanguage _language = SystemLanguage.English)
        {
            StartCoroutine(GetPokemonTextFromPokeAPI(_speciesNumber, (string _data) =>
            {
                PokeAPIPokemonData apiData = JsonUtility.FromJson<PokeAPIPokemonData>(_data);
                var dexData = apiData.flavor_text_entries;
                var languageCode = PKHexUtils.GetLanguageCode(_language);
                var possible = dexData.Where(x => x.version.name == _generationID && x.language.name == languageCode);

                foreach (var d in possible)
                {
                    var entry = d.flavor_text.Replace("\n", " ").Replace("\f", " ").Replace("- "," ");
                    Debug.Log(entry);
                }
            }));
        }

        public virtual void GetPokemonFromAPI(int _speciesNumber, bool _saveIt = true)
        {
            StartCoroutine(GetPokemonTextFromPokeAPI(_speciesNumber, (string _data) =>
            {
                PokeAPIPokemonData apiData = JsonUtility.FromJson<PokeAPIPokemonData>(_data);
                if (_saveIt)
                    System.IO.File.WriteAllText(Application.dataPath + "/DefaultPokemon.json", _data);
                else
                    Debug.Log(_data);
            }));
        }

        public virtual void FillDexFromAPI()
        {
            m_pokedex = new PokedexAPI();

            StartCoroutine(FillDexFromAPICoroutine(m_pokedex));
        }

        public virtual IEnumerator FillDexFromAPICoroutine(PokedexAPI dex)
        {
            int length = PKHexUtils.GetDexCount(PokemonGeneration.LegendsArceus);
            for (int i = 1; i <= length; i++)
            {
                yield return GetPokemonTextFromPokeAPI(i, (string _data) =>
                {
                    PokeAPIPokemonData pkmn = JsonUtility.FromJson<PokeAPIPokemonData>(_data);
                    AddToDex(dex, pkmn, i == length);
                });

            }
        }

        public virtual void AddToDex(PokedexAPI dex, PokeAPIPokemonData pokemon, bool _isLast)
        {
            Debug.Log("Parsed " + pokemon.name);
            dex.Pokemon.Add(pokemon);

            if (_isLast)
            {
                System.IO.File.WriteAllText(Application.dataPath + "/dex.json", JsonUtility.ToJson(dex));
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif
                Debug.Log("Saved Dex");
            }
        }

        public IEnumerator GetPokemonTextFromPokeAPI(int _speciesNumber, UnityAction<string> onLoad = null)
        {
            UnityWebRequest www = UnityWebRequest.Get("https://pokeapi.co/api/v2/pokemon-species/" + _speciesNumber);
            yield return www.SendWebRequest();

            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError(www.error);
                onLoad?.Invoke("No Data");
            }
            else
            {
                // Show results as text
                onLoad?.Invoke(www.downloadHandler.text);
            }
        }
    }
}
