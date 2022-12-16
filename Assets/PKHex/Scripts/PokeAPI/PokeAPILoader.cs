using PKHeX.Core;
using SFB;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
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
        legendsarceus,
        scarlet,
        violet
    }

    public class PokeAPILoader : MonoBehaviour
    {
        public PokemonVersions Version = PokemonVersions.red;

        [SerializeField] PokeAPIPokemonData m_loadedMon;
        protected PokedexAPI m_pokedex;
        protected bool m_parsingDexFromAPI = false;
        protected Coroutine m_dexParseCoroutine;

        public virtual bool IsParsingDexFromAPI => m_parsingDexFromAPI;

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

        public virtual PokemonGeneration GetGameGenerationFromVersion(PokemonVersions version)
        {
            switch (version)
            {
                case PokemonVersions.red:
                case PokemonVersions.blue:
                case PokemonVersions.yellow:
                    return PokemonGeneration.Kanto;
                case PokemonVersions.gold:
                case PokemonVersions.silver:
                case PokemonVersions.crystal:
                    return PokemonGeneration.Johto;
                case PokemonVersions.ruby:
                case PokemonVersions.sapphire:
                case PokemonVersions.emerald:
                    return PokemonGeneration.Hoenn;
                case PokemonVersions.diamond:
                case PokemonVersions.pearl:
                    return PokemonGeneration.Sinnoh;
                case PokemonVersions.platinum:
                    return PokemonGeneration.SinnohPt;
                case PokemonVersions.heartgold:
                case PokemonVersions.soulsilver:
                    return PokemonGeneration.HGSS;
                case PokemonVersions.black:
                case PokemonVersions.white:
                    return PokemonGeneration.Tesselia;
                case PokemonVersions.black2:
                case PokemonVersions.white2:
                    return PokemonGeneration.TesseliaBW2;
                case PokemonVersions.x:
                case PokemonVersions.y:
                    return PokemonGeneration.Kalos;
                case PokemonVersions.omegaruby:
                case PokemonVersions.alphasapphire:
                    return PokemonGeneration.ORAS;
                case PokemonVersions.sun:
                case PokemonVersions.moon:
                case PokemonVersions.ultrasun:
                case PokemonVersions.ultramoon:
                    return PokemonGeneration.Alola;
                case PokemonVersions.sword:
                case PokemonVersions.shield:
                    return PokemonGeneration.Galar;
                case PokemonVersions.brilliantdiamond:
                case PokemonVersions.shiningpearl:
                    return PokemonGeneration.BDSP;
                case PokemonVersions.legendsarceus:
                    return PokemonGeneration.LegendsArceus;
                case PokemonVersions.scarlet:
                case PokemonVersions.violet:
                    return PokemonGeneration.ScarletViolet;
            }

            return LastGenerationSupported;
        }

        public PokemonGeneration LastGenerationSupported => PokemonGeneration.LegendsArceus;

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
                case PokemonGeneration.ScarletViolet:
                    return new List<string>() { "scarlet-violet" };
                default:
                    return new List<string>() { "red", "blue", "yellow" };
            }
        }

        public PokemonVersions LastGameSupported() => PokemonVersions.legendsarceus;

        public virtual void GetDexDescription(int _speciesNumber, string _generationID, LanguageID _language = LanguageID.English)
        {
            StartCoroutine(GetPokemonTextFromPokeAPI(_speciesNumber, (string _data) =>
            {
                PokeAPIPokemonData apiData = JsonUtility.FromJson<PokeAPIPokemonData>(_data);
                var dexData = apiData.flavor_text_entries;
                var languageCode = PKHexUtils.GetLanguageCode(_language);
                var possible = dexData.Where(x => x.version.name == _generationID && x.language.name == languageCode);
                foreach (var d in possible)
                {
                    var entry = d.flavor_text.Replace("\n", " ").Replace("\f", " ").Replace("- ", " ");
                    Debug.Log(entry);
                }
            }));
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

        public virtual void LoadPokemonFromAPIJSON(int target)
        {
            if (m_pokedex == null || m_pokedex.Pokemon.Count == 0)
            {
                LoadDexFromAPIJSON();
            }

            if (m_pokedex == null || m_pokedex.Pokemon.Count == 0)
                return;

            target = Mathf.Clamp(target, 0, m_pokedex.Pokemon.Count - 1);

            m_loadedMon = m_pokedex.Pokemon[target];
        }

        public virtual void LoadDexFromAPIJSON()
        {
            var paths = StandaloneFileBrowser.OpenFilePanel("Selected the JSON File", Application.dataPath, new ExtensionFilter[] { new ExtensionFilter() { Name = "JSON File", Extensions = new string[] { "json" } } }, false);
            if (paths != null && paths.Count > 0 && !string.IsNullOrEmpty(paths[0].Name) && System.IO.File.Exists(paths[0].Name))
            {
                m_pokedex = JsonUtility.FromJson<PokedexAPI>(System.IO.File.ReadAllText(paths[0].Name));
              
            }
        }

        public virtual void FillDexFromAPI()
        {
            m_pokedex = new PokedexAPI() { Version = Version.ToString(), Pokemon = new List<PokeAPIPokemonData>() };
            m_parsingDexFromAPI = true;
            m_dexParseCoroutine = StartCoroutine(FillDexFromAPICoroutine(m_pokedex));
        }

        public virtual void CancelFillDexFromAPI()
        {
            m_parsingDexFromAPI = false;
            StopCoroutine(m_dexParseCoroutine);
            m_dexParseCoroutine = null;

#if UNITY_EDITOR
            if (m_pokedex.Pokemon.Count > 0 && UnityEditor.EditorUtility.DisplayDialog("Save Dex from API?", "Do you want to svave the parsed PokeDex?", "Yes", "No"))
            {
                m_loadedMon = m_pokedex.Pokemon[0];
                SaveDexToJSON(m_pokedex);
            }

            m_pokedex.Pokemon.Clear();
#endif
        }

        public virtual PokemonVersions GetSelectedGeneration()
        {
            int numberSelected = (int)Version;
            int max = (int)LastGameSupported();

            if (numberSelected < max)
            {
                return Version;
            }

            return LastGameSupported();
        }

        public virtual IEnumerator FillDexFromAPICoroutine(PokedexAPI dex)
        {
            PokemonGeneration generation = GetGameGenerationFromVersion(GetSelectedGeneration());
            int length = PKHexUtils.GetDexCount(generation);
            Debug.Log("Parsing dex from generation: " + generation +". Count: "+length);

            dex.PokemonGeneration = generation.ToString();
            dex.TotalPokemon = length;

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
                m_loadedMon = m_pokedex.Pokemon[0];
                m_parsingDexFromAPI = false;
                SaveDexToJSON(dex);
            }
        }

        public virtual void SaveDexToJSON(PokedexAPI dex)
        {
            string path = Application.dataPath + "/PKHex/Data/Parsed Dex From API/";

            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            string file = dex.PokemonGeneration+"_"+dex.Version + "_dex.json";

            path += "/" + file;

#if UNITY_EDITOR
            path = AssetDatabase.GenerateUniqueAssetPath("Assets/Parsed Dex From API/" + file);
#endif
            System.IO.File.WriteAllText(path, JsonUtility.ToJson(dex));
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            Debug.Log("Saved Dex at " + path);
        }

        public IEnumerator GetPokemonTextFromPokeAPI(int _speciesNumber, UnityAction<string> onLoad = null)
        {
            UnityWebRequest www = UnityWebRequest.Get("https://pokeapi.co/api/v2/pokemon-species/" + _speciesNumber);
            yield return www.SendWebRequest();

            if (!string.IsNullOrEmpty(www.error) || www.downloadHandler.text.Contains("Not Found"))
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
