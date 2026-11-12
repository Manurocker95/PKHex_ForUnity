using System.Collections.Generic;
using UnityEngine;

namespace PKHexForUnity
{
    /// <summary>
    /// Lists the generations, games and languages present in the exported data (Resources path <c>Data/PKHexDataIndex</c>).
    /// Rebuilt by the exporter from the files on disk after every export.
    /// </summary>
    public class PKHexDataIndex : ScriptableObject
    {
        [SerializeField]
        private List<PKHexExportedGeneration> m_generations = new List<PKHexExportedGeneration>();

        public List<PKHexExportedGeneration> Generations { get { return m_generations; } }

        public PKHexExportedGeneration GetGeneration(int _generation)
        {
            for (int i = 0; i < m_generations.Count; i++)
            {
                if (m_generations[i].Generation == _generation)
                {
                    return m_generations[i];
                }
            }
            return null;
        }

        public bool HasGame(int _generation, string _gameKey)
        {
            PKHexExportedGeneration generation = GetGeneration(_generation);
            return generation != null && System.Array.IndexOf(generation.GameKeys, _gameKey) >= 0;
        }

        public bool HasLanguage(int _generation, string _languageFolder)
        {
            PKHexExportedGeneration generation = GetGeneration(_generation);
            return generation != null && System.Array.IndexOf(generation.LanguageFolders, _languageFolder) >= 0;
        }

        /// <summary> Used by the Editor exporter to fill the asset. </summary>
        public void SetData(List<PKHexExportedGeneration> _generations)
        {
            m_generations = _generations;
        }
    }
}
