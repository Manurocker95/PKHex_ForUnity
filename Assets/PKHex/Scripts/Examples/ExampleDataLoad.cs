using System.Collections;
using UnityEngine;

namespace PKHexForUnity
{
    /// <summary>
    /// Loads the exported data asynchronously from Resources and logs one Pokémon. Does not use PKHeX.Core.dll, so it
    /// also works in data-only builds (export the data first with Window/PKHeX/Data Exporter).
    /// </summary>
    public class ExampleDataLoad : MonoBehaviour
    {
        [SerializeField]
        private int m_generation = 9;

        [SerializeField]
        private string m_gameKey = "SV";

        [SerializeField]
        private string m_languageFolder = "Spanish";

        [SerializeField]
        private int m_species = 25;

        [SerializeField]
        private int m_form = 0;

        [SerializeField]
        private bool m_logTextFile = true;

        private PKHexDataIndex m_index;
        private PKHexGameData m_game;
        private PKHexStringTable m_strings;
        private TextAsset m_textFile;

        protected virtual IEnumerator Start()
        {
            yield return StartCoroutine(PKHexDatabase.LoadIndexAsync(OnIndexLoaded));
            if (m_index == null)
            {
                yield break;
            }
            if (!m_index.HasGame(m_generation, m_gameKey) || !m_index.HasLanguage(m_generation, m_languageFolder))
            {
                Debug.LogError("[PKHex] Gen " + m_generation + " " + m_gameKey + " / " + m_languageFolder + " was not exported.");
                yield break;
            }

            // Both requests run at the same time.
            Coroutine gameLoad = StartCoroutine(PKHexDatabase.LoadGameAsync(m_generation, m_gameKey, OnGameLoaded));
            Coroutine stringsLoad = StartCoroutine(PKHexDatabase.LoadStringsAsync(m_generation, m_languageFolder, OnStringsLoaded));
            yield return gameLoad;
            yield return stringsLoad;
            if (m_game == null || m_strings == null)
            {
                yield break;
            }

            PKHexSpeciesEntry entry = m_game.GetSpecies(m_species, m_form);
            if (entry == null)
            {
                Debug.LogError("[PKHex] Species " + m_species + " form " + m_form + " is not in " + m_gameKey + ".");
            }
            else
            {
                Debug.Log("[PKHex] " + m_strings.GetGameName(m_gameKey) + "\n" + PKHexDataText.FormatSpecies(entry, m_gameKey, m_strings));
            }

            if (m_logTextFile)
            {
                yield return StartCoroutine(PKHexDatabase.LoadTextAsync(m_generation, m_languageFolder, "Natures", OnTextLoaded));
                if (m_textFile != null)
                {
                    Debug.Log("[PKHex] Natures.txt:\n" + m_textFile.text);
                }
            }
        }

        private void OnIndexLoaded(PKHexDataIndex _index)
        {
            m_index = _index;
        }

        private void OnGameLoaded(PKHexGameData _game)
        {
            m_game = _game;
        }

        private void OnStringsLoaded(PKHexStringTable _strings)
        {
            m_strings = _strings;
        }

        private void OnTextLoaded(TextAsset _text)
        {
            m_textFile = _text;
        }
    }
}
