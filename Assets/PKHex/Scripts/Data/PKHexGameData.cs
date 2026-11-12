using System.Collections.Generic;
using UnityEngine;

namespace PKHexForUnity
{
    /// <summary>
    /// Language-independent Pokémon database of one game (or pair of versions), exported from PKHeX.
    /// Resources path: <c>Data/GenN/Games/&lt;GameKey&gt;</c>. Names come from <see cref="PKHexStringTable"/>.
    /// </summary>
    public class PKHexGameData : ScriptableObject
    {
        [SerializeField]
        private int m_generation;

        [SerializeField]
        private string m_gameKey = string.Empty;

        /// <summary> PKHeX GameVersion names of the versions covered by this data (e.g. "SL", "VL"). </summary>
        [SerializeField]
        private string[] m_versions = new string[0];

        /// <summary> PKHeX EntityContext name (e.g. "Gen9"). </summary>
        [SerializeField]
        private string m_context = string.Empty;

        [SerializeField]
        private int m_maxSpeciesID;

        [SerializeField]
        private int m_maxMoveID;

        [SerializeField]
        private int m_maxItemID;

        [SerializeField]
        private int m_maxAbilityID;

        [SerializeField]
        private List<PKHexSpeciesEntry> m_species = new List<PKHexSpeciesEntry>();

        [SerializeField]
        private List<PKHexMoveEntry> m_moves = new List<PKHexMoveEntry>();

        private Dictionary<int, PKHexSpeciesEntry> m_speciesLookup;

        public int Generation { get { return m_generation; } }
        public string GameKey { get { return m_gameKey; } }
        public string[] Versions { get { return m_versions; } }
        public string Context { get { return m_context; } }
        public int MaxSpeciesID { get { return m_maxSpeciesID; } }
        public int MaxMoveID { get { return m_maxMoveID; } }
        public int MaxItemID { get { return m_maxItemID; } }
        public int MaxAbilityID { get { return m_maxAbilityID; } }

        /// <summary> Every species-form of the game's data tables, ordered by species then form. </summary>
        public List<PKHexSpeciesEntry> Species { get { return m_species; } }

        /// <summary> Move data indexed by position; use <see cref="GetMove"/> to look up by move ID. </summary>
        public List<PKHexMoveEntry> Moves { get { return m_moves; } }

        /// <summary> Gets a species-form, or null when it is not in the exported data. </summary>
        public PKHexSpeciesEntry GetSpecies(int _species, int _form)
        {
            if (m_speciesLookup == null)
            {
                BuildLookup();
            }
            PKHexSpeciesEntry entry;
            return m_speciesLookup.TryGetValue(GetKey(_species, _form), out entry) ? entry : null;
        }

        /// <summary> Gets all forms of a species (empty when the species is not in the data). </summary>
        public List<PKHexSpeciesEntry> GetForms(int _species)
        {
            List<PKHexSpeciesEntry> result = new List<PKHexSpeciesEntry>();
            PKHexSpeciesEntry baseForm = GetSpecies(_species, 0);
            if (baseForm == null)
            {
                return result;
            }
            int count = Mathf.Max(1, baseForm.FormCount);
            for (int form = 0; form < count; form++)
            {
                PKHexSpeciesEntry entry = GetSpecies(_species, form);
                if (entry != null)
                {
                    result.Add(entry);
                }
            }
            return result;
        }

        /// <summary> Gets a move's data, or null when the move ID is out of range for this game. </summary>
        public PKHexMoveEntry GetMove(int _move)
        {
            // Moves are exported contiguously from ID 0, so the index is the ID; checked in case of manual edits.
            if (_move >= 0 && _move < m_moves.Count && m_moves[_move].Move == _move)
            {
                return m_moves[_move];
            }
            for (int i = 0; i < m_moves.Count; i++)
            {
                if (m_moves[i].Move == _move)
                {
                    return m_moves[i];
                }
            }
            return null;
        }

        /// <summary> Used by the Editor exporter to fill the asset. </summary>
        public void SetData(int _generation, string _gameKey, string[] _versions, string _context, int[] _limits,
            List<PKHexSpeciesEntry> _species, List<PKHexMoveEntry> _moves)
        {
            m_generation = _generation;
            m_gameKey = _gameKey;
            m_versions = _versions;
            m_context = _context;
            m_maxSpeciesID = _limits[0];
            m_maxMoveID = _limits[1];
            m_maxItemID = _limits[2];
            m_maxAbilityID = _limits[3];
            m_species = _species;
            m_moves = _moves;
            m_speciesLookup = null;
        }

        protected virtual void OnEnable()
        {
            // Serialized data may have been replaced (asset reimport); rebuild lazily.
            m_speciesLookup = null;
        }

        private void BuildLookup()
        {
            m_speciesLookup = new Dictionary<int, PKHexSpeciesEntry>(m_species.Count);
            for (int i = 0; i < m_species.Count; i++)
            {
                PKHexSpeciesEntry entry = m_species[i];
                m_speciesLookup[GetKey(entry.Species, entry.Form)] = entry;
            }
        }

        private static int GetKey(int _species, int _form)
        {
            return (_species << 8) | (_form & 0xFF);
        }
    }
}
