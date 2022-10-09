using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace PKHexForUnity.PokeAPI
{
    public class PokeAPILoader : MonoBehaviour
    {
        [SerializeField] protected PokedexAPI m_pokedex;

        public virtual void GetPokemonFromAPI(int _speciesNumber, bool _isLast)
        {
            StartCoroutine(GetPokemonTextFromPokeAPI(_speciesNumber, (string _data) =>
            {
                PokeAPIPokemonData apiData = JsonUtility.FromJson<PokeAPIPokemonData>(_data);
                System.IO.File.WriteAllText(Application.dataPath + "/DefaultPokemon.json", _data);
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
