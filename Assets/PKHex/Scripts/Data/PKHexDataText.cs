using System.Collections.Generic;
using System.Text;

namespace PKHexForUnity
{
    /// <summary> Layout of the exported list .txt files. </summary>
    public enum PKHexTextFormat
    {
        /// <summary> No ID column: one name per line (line N, counting from 0, is ID N). Extra columns comma-separated. </summary>
        NamesOnly,

        /// <summary> Comma-separated with the ID first: "6,Fire". </summary>
        CommaSeparated,
    }

    /// <summary>
    /// Builds readable text and CSV from the exported data. Used by the exporter and usable at runtime.
    /// </summary>
    /// <remarks>
    /// CSV fields follow RFC 4180: a field containing a comma, quote or line break is quoted and its quotes doubled.
    /// </remarks>
    public static class PKHexDataText
    {
        public const string NONE_TEXT = "-";
        private const string SEPARATOR = ", ";
        private const char CSV_SEPARATOR = ',';

        /// <summary> One entry per line: "Name" or "ID,Name". </summary>
        public static string FormatIdList(string[] _names, PKHexTextFormat _format)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _names.Length; i++)
            {
                if (_format == PKHexTextFormat.CommaSeparated)
                {
                    sb.Append(i).Append(CSV_SEPARATOR).Append(EscapeCsv(_names[i]));
                }
                else
                {
                    sb.Append(_names[i]);
                }
                sb.Append('\n');
            }
            return sb.ToString();
        }

        /// <summary> "SpeciesName,FormName" or "Species,Form,SpeciesName,FormName" per line. </summary>
        public static string FormatFormList(PKHexGameStrings _game, PKHexStringTable _strings, PKHexTextFormat _format)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _game.Forms.Length; i++)
            {
                PKHexFormName form = _game.Forms[i];
                if (_format == PKHexTextFormat.CommaSeparated)
                {
                    sb.Append(form.Species).Append(CSV_SEPARATOR).Append(form.Form).Append(CSV_SEPARATOR);
                }
                sb.Append(EscapeCsv(_strings.GetSpeciesName(form.Species))).Append(CSV_SEPARATOR).Append(EscapeCsv(form.Name)).Append('\n');
            }
            return sb.ToString();
        }

        /// <summary> "Name,Type,PP" or "ID,Name,Type,PP" per line. </summary>
        public static string FormatMoveList(PKHexGameData _game, PKHexStringTable _strings, PKHexTextFormat _format)
        {
            StringBuilder sb = new StringBuilder();
            List<PKHexMoveEntry> moves = _game.Moves;
            for (int i = 0; i < moves.Count; i++)
            {
                PKHexMoveEntry move = moves[i];
                if (_format == PKHexTextFormat.CommaSeparated)
                {
                    sb.Append(move.Move).Append(CSV_SEPARATOR);
                }
                sb.Append(EscapeCsv(_strings.GetMoveName(move.Move))).Append(CSV_SEPARATOR)
                    .Append(EscapeCsv(_strings.GetTypeName(move.Type))).Append(CSV_SEPARATOR).Append(move.PP).Append('\n');
            }
            return sb.ToString();
        }

        /// <summary>
        /// CSV with one row per ID and one column per language: "ID,English,Spanish" header, then "6,Fire,Fuego".
        /// Rows run to the longest column; shorter columns are left empty.
        /// </summary>
        public static string FormatCsvIdTable(string[] _languages, string[][] _columns, bool _header)
        {
            StringBuilder sb = new StringBuilder();
            if (_header)
            {
                sb.Append("ID");
                for (int c = 0; c < _languages.Length; c++)
                {
                    sb.Append(CSV_SEPARATOR).Append(EscapeCsv(_languages[c]));
                }
                sb.Append('\n');
            }
            int rows = 0;
            for (int c = 0; c < _columns.Length; c++)
            {
                rows = System.Math.Max(rows, _columns[c].Length);
            }
            for (int r = 0; r < rows; r++)
            {
                sb.Append(r);
                for (int c = 0; c < _columns.Length; c++)
                {
                    sb.Append(CSV_SEPARATOR);
                    if (r < _columns[c].Length)
                    {
                        sb.Append(EscapeCsv(_columns[c][r]));
                    }
                }
                sb.Append('\n');
            }
            return sb.ToString();
        }

        /// <summary>
        /// CSV of the form names of one game: "Species,Form,English,Spanish..." per row.
        /// Rows come from the first table (English when ordered by the exporter).
        /// </summary>
        public static string FormatCsvFormTable(string _gameKey, PKHexStringTable[] _tables, bool _header)
        {
            StringBuilder sb = new StringBuilder();
            if (_header)
            {
                sb.Append("Species").Append(CSV_SEPARATOR).Append("Form");
                for (int c = 0; c < _tables.Length; c++)
                {
                    sb.Append(CSV_SEPARATOR).Append(EscapeCsv(_tables[c].LanguageFolder));
                }
                sb.Append('\n');
            }
            PKHexGameStrings rows = _tables.Length != 0 ? _tables[0].GetGameStrings(_gameKey) : null;
            if (rows == null)
            {
                return sb.ToString();
            }
            for (int r = 0; r < rows.Forms.Length; r++)
            {
                PKHexFormName form = rows.Forms[r];
                sb.Append(form.Species).Append(CSV_SEPARATOR).Append(form.Form);
                for (int c = 0; c < _tables.Length; c++)
                {
                    sb.Append(CSV_SEPARATOR).Append(EscapeCsv(_tables[c].GetFormName(_gameKey, form.Species, form.Form)));
                }
                sb.Append('\n');
            }
            return sb.ToString();
        }

        /// <summary> Quotes a CSV field when needed (comma, quote or line break). </summary>
        public static string EscapeCsv(string _value)
        {
            if (string.IsNullOrEmpty(_value))
            {
                return string.Empty;
            }
            if (_value.IndexOfAny(new char[] { CSV_SEPARATOR, '"', '\n', '\r' }) < 0)
            {
                return _value;
            }
            return "\"" + _value.Replace("\"", "\"\"") + "\"";
        }

        /// <summary> Full readable dump of every species-form of a game. </summary>
        public static string FormatPokedex(PKHexGameData _game, PKHexStringTable _strings)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(_strings.GetGameName(_game.GameKey)).Append(" [").Append(_game.GameKey).Append("] Gen ")
                .Append(_game.Generation).Append(" (").Append(_strings.LanguageCode).Append(")\n");
            sb.Append("Species: ").Append(_game.MaxSpeciesID).Append(" | Moves: ").Append(_game.MaxMoveID)
                .Append(" | Items: ").Append(_game.MaxItemID).Append(" | Abilities: ").Append(_game.MaxAbilityID).Append("\n\n");
            List<PKHexSpeciesEntry> species = _game.Species;
            for (int i = 0; i < species.Count; i++)
            {
                AppendSpecies(sb, species[i], _game.GameKey, _strings);
                sb.Append('\n');
            }
            return sb.ToString();
        }

        /// <summary> Readable block of one species-form. </summary>
        public static string FormatSpecies(PKHexSpeciesEntry _entry, string _gameKey, PKHexStringTable _strings)
        {
            StringBuilder sb = new StringBuilder();
            AppendSpecies(sb, _entry, _gameKey, _strings);
            return sb.ToString();
        }

        private static void AppendSpecies(StringBuilder _sb, PKHexSpeciesEntry _entry, string _gameKey, PKHexStringTable _strings)
        {
            _sb.Append('#').Append(_entry.Species.ToString("000")).Append(' ')
                .Append(_strings.GetSpeciesFormName(_gameKey, _entry.Species, _entry.Form))
                .Append(" [form ").Append(_entry.Form).Append('/').Append(_entry.FormCount).Append(']');
            if (!_entry.IsPresentInGame)
            {
                _sb.Append(" (not obtainable in this game)");
            }
            _sb.Append('\n');

            _sb.Append("  Types: ").Append(_strings.GetTypeName(_entry.Type1));
            if (_entry.Type2 != _entry.Type1)
            {
                _sb.Append(" / ").Append(_strings.GetTypeName(_entry.Type2));
            }
            _sb.Append('\n');

            int[] stats = _entry.BaseStats;
            if (stats != null && stats.Length == 6)
            {
                _sb.Append("  Base stats: HP ").Append(stats[0]).Append(" / Atk ").Append(stats[1]).Append(" / Def ").Append(stats[2])
                    .Append(" / SpA ").Append(stats[3]).Append(" / SpD ").Append(stats[4]).Append(" / Spe ").Append(stats[5])
                    .Append(" (BST ").Append(_entry.BaseStatTotal).Append(")\n");
            }

            _sb.Append("  Abilities: ");
            AppendNames(_sb, _entry.Abilities, _strings.AbilityNames);
            _sb.Append('\n');

            _sb.Append("  Gender ratio: ").Append(_entry.GenderRatio).Append(" | Catch rate: ").Append(_entry.CatchRate)
                .Append(" | Base EXP: ").Append(_entry.BaseExperience).Append(" | Growth: ").Append(_entry.ExperienceGrowth)
                .Append(" | Egg groups: ").Append(_entry.EggGroup1).Append('/').Append(_entry.EggGroup2)
                .Append(" | Height: ").Append(_entry.Height).Append(" | Weight: ").Append(_entry.Weight).Append('\n');

            _sb.Append("  Level-up: ");
            if (_entry.LevelUpMoves.Length == 0)
            {
                _sb.Append(NONE_TEXT);
            }
            for (int i = 0; i < _entry.LevelUpMoves.Length; i++)
            {
                if (i != 0)
                {
                    _sb.Append(SEPARATOR);
                }
                int level = i < _entry.LevelUpLevels.Length ? _entry.LevelUpLevels[i] : 0;
                _sb.Append("Lv").Append(level).Append(' ').Append(_strings.GetMoveName(_entry.LevelUpMoves[i]));
            }
            _sb.Append('\n');

            if (_entry.InitialMoves != null && _entry.InitialMoves.Length != 0)
            {
                _sb.Append("  Initial moves: ");
                AppendNames(_sb, _entry.InitialMoves, _strings.MoveNames);
                _sb.Append('\n');
            }

            _sb.Append("  Egg moves: ");
            AppendNames(_sb, _entry.EggMoves, _strings.MoveNames);
            _sb.Append("\n  Machines: ");
            AppendNames(_sb, _entry.MachineMoves, _strings.MoveNames);
            _sb.Append("\n  Tutors: ");
            AppendNames(_sb, _entry.TutorMoves, _strings.MoveNames);
            _sb.Append('\n');

            _sb.Append("  Evolutions: ");
            if (_entry.Evolutions.Length == 0)
            {
                _sb.Append(NONE_TEXT);
            }
            for (int i = 0; i < _entry.Evolutions.Length; i++)
            {
                PKHexEvolutionEntry evo = _entry.Evolutions[i];
                if (i != 0)
                {
                    _sb.Append(SEPARATOR);
                }
                _sb.Append(_strings.GetSpeciesFormName(_gameKey, evo.Species, evo.Form)).Append(" via ").Append(evo.MethodName);
                if (evo.Argument != 0)
                {
                    _sb.Append(" arg ").Append(evo.Argument);
                }
                if (evo.Level != 0)
                {
                    _sb.Append(" Lv").Append(evo.Level);
                }
            }
            _sb.Append('\n');
        }

        private static void AppendNames(StringBuilder _sb, int[] _ids, string[] _names)
        {
            if (_ids == null || _ids.Length == 0)
            {
                _sb.Append(NONE_TEXT);
                return;
            }
            for (int i = 0; i < _ids.Length; i++)
            {
                if (i != 0)
                {
                    _sb.Append(SEPARATOR);
                }
                int id = _ids[i];
                _sb.Append(id >= 0 && id < _names.Length ? _names[id] : PKHexStringTable.INVALID_TEXT);
            }
        }
    }
}
