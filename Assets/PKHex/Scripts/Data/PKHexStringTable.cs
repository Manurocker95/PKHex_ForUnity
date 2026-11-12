using System.Collections.Generic;
using UnityEngine;

namespace PKHexForUnity
{
    /// <summary>
    /// Localized strings of one generation in one language, exported from PKHeX.
    /// Resources path: <c>Data/GenN/&lt;Language&gt;/PKHexStrings</c>.
    /// </summary>
    /// <remarks>
    /// Species, move and ability lists are cut to the highest ID of the generation. Item and form names differ between
    /// games of the same generation, so they are stored per game (<see cref="PKHexGameStrings"/>).
    /// </remarks>
    public class PKHexStringTable : ScriptableObject
    {
        public const string INVALID_TEXT = "???";

        [SerializeField]
        private int m_generation;

        [SerializeField]
        private string m_languageCode = string.Empty;

        [SerializeField]
        private string m_languageFolder = string.Empty;

        [SerializeField]
        private string[] m_species = new string[0];

        [SerializeField]
        private string[] m_moves = new string[0];

        [SerializeField]
        private string[] m_abilities = new string[0];

        [SerializeField]
        private string[] m_natures = new string[0];

        [SerializeField]
        private string[] m_types = new string[0];

        [SerializeField]
        private string[] m_balls = new string[0];

        [SerializeField]
        private List<PKHexGameStrings> m_games = new List<PKHexGameStrings>();

        private Dictionary<string, Dictionary<int, string>> m_formLookup;

        public int Generation { get { return m_generation; } }
        public string LanguageCode { get { return m_languageCode; } }
        public string LanguageFolder { get { return m_languageFolder; } }
        public string[] SpeciesNames { get { return m_species; } }
        public string[] MoveNames { get { return m_moves; } }
        public string[] AbilityNames { get { return m_abilities; } }
        public string[] NatureNames { get { return m_natures; } }
        public string[] TypeNames { get { return m_types; } }
        public string[] BallNames { get { return m_balls; } }
        public List<PKHexGameStrings> Games { get { return m_games; } }

        public string GetSpeciesName(int _species) { return GetSafe(m_species, _species); }
        public string GetMoveName(int _move) { return GetSafe(m_moves, _move); }
        public string GetAbilityName(int _ability) { return GetSafe(m_abilities, _ability); }
        public string GetNatureName(int _nature) { return GetSafe(m_natures, _nature); }
        public string GetTypeName(int _type) { return GetSafe(m_types, _type); }
        public string GetBallName(int _ball) { return GetSafe(m_balls, _ball); }

        /// <summary> Strings of one game of the generation, or null when that game was not exported. </summary>
        public PKHexGameStrings GetGameStrings(string _gameKey)
        {
            for (int i = 0; i < m_games.Count; i++)
            {
                if (m_games[i].GameKey == _gameKey)
                {
                    return m_games[i];
                }
            }
            return null;
        }

        public string GetGameName(string _gameKey)
        {
            PKHexGameStrings game = GetGameStrings(_gameKey);
            return game != null ? game.GameName : INVALID_TEXT;
        }

        public string GetItemName(string _gameKey, int _item)
        {
            PKHexGameStrings game = GetGameStrings(_gameKey);
            return game != null ? GetSafe(game.Items, _item) : INVALID_TEXT;
        }

        /// <summary> Form name, or an empty string for species without named alternate forms. </summary>
        public string GetFormName(string _gameKey, int _species, int _form)
        {
            if (m_formLookup == null)
            {
                BuildFormLookup();
            }
            Dictionary<int, string> forms;
            if (_gameKey == null || !m_formLookup.TryGetValue(_gameKey, out forms))
            {
                return string.Empty;
            }
            string name;
            return forms.TryGetValue((_species << 8) | (_form & 0xFF), out name) ? name : string.Empty;
        }

        /// <summary> Species name with the form name in parentheses when it has one. </summary>
        public string GetSpeciesFormName(string _gameKey, int _species, int _form)
        {
            string form = GetFormName(_gameKey, _species, _form);
            string name = GetSpeciesName(_species);
            return string.IsNullOrEmpty(form) ? name : name + " (" + form + ")";
        }

        /// <summary> Finds a species ID by its name (case-insensitive); -1 when not found. </summary>
        public int FindSpecies(string _name)
        {
            return FindIndex(m_species, _name);
        }

        /// <summary> Finds a move ID by its name (case-insensitive); -1 when not found. </summary>
        public int FindMove(string _name)
        {
            return FindIndex(m_moves, _name);
        }

        /// <summary> Used by the Editor exporter to fill the asset. </summary>
        public void SetData(int _generation, string _languageCode, string _languageFolder, string[] _species, string[] _moves,
            string[] _abilities, string[] _natures, string[] _types, string[] _balls, List<PKHexGameStrings> _games)
        {
            m_generation = _generation;
            m_languageCode = _languageCode;
            m_languageFolder = _languageFolder;
            m_species = _species;
            m_moves = _moves;
            m_abilities = _abilities;
            m_natures = _natures;
            m_types = _types;
            m_balls = _balls;
            m_games = _games;
            m_formLookup = null;
        }

        protected virtual void OnEnable()
        {
            m_formLookup = null;
        }

        private void BuildFormLookup()
        {
            m_formLookup = new Dictionary<string, Dictionary<int, string>>();
            for (int i = 0; i < m_games.Count; i++)
            {
                PKHexGameStrings game = m_games[i];
                Dictionary<int, string> forms = new Dictionary<int, string>();
                for (int j = 0; j < game.Forms.Length; j++)
                {
                    PKHexFormName form = game.Forms[j];
                    forms[(form.Species << 8) | (form.Form & 0xFF)] = form.Name;
                }
                m_formLookup[game.GameKey] = forms;
            }
        }

        private static string GetSafe(string[] _list, int _index)
        {
            return _list != null && _index >= 0 && _index < _list.Length ? _list[_index] : INVALID_TEXT;
        }

        private static int FindIndex(string[] _list, string _name)
        {
            if (_list == null || string.IsNullOrEmpty(_name))
            {
                return -1;
            }
            for (int i = 0; i < _list.Length; i++)
            {
                if (string.Equals(_list[i], _name, System.StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
