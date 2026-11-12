using System.Collections.Generic;
using System.Text;

namespace PKHexForUnity
{
    /// <summary>
    /// Builds readable text from the exported data. Used by the exporter for the .txt files and usable at runtime.
    /// </summary>
    public static class PKHexDataText
    {
        public const string NONE_TEXT = "-";
        private const string SEPARATOR = ", ";

        /// <summary> "ID&lt;tab&gt;Name" per line. </summary>
        public static string FormatIdList(string[] _names)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _names.Length; i++)
            {
                sb.Append(i).Append('\t').Append(_names[i]).Append('\n');
            }
            return sb.ToString();
        }

        /// <summary> "Species&lt;tab&gt;Form&lt;tab&gt;SpeciesName&lt;tab&gt;FormName" per line. </summary>
        public static string FormatFormList(PKHexGameStrings _game, PKHexStringTable _strings)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _game.Forms.Length; i++)
            {
                PKHexFormName form = _game.Forms[i];
                sb.Append(form.Species).Append('\t').Append(form.Form).Append('\t')
                    .Append(_strings.GetSpeciesName(form.Species)).Append('\t').Append(form.Name).Append('\n');
            }
            return sb.ToString();
        }

        /// <summary> "ID&lt;tab&gt;Name&lt;tab&gt;Type&lt;tab&gt;PP" per line. </summary>
        public static string FormatMoveList(PKHexGameData _game, PKHexStringTable _strings)
        {
            StringBuilder sb = new StringBuilder();
            List<PKHexMoveEntry> moves = _game.Moves;
            for (int i = 0; i < moves.Count; i++)
            {
                PKHexMoveEntry move = moves[i];
                sb.Append(move.Move).Append('\t').Append(_strings.GetMoveName(move.Move)).Append('\t')
                    .Append(_strings.GetTypeName(move.Type)).Append('\t').Append(move.PP).Append('\n');
            }
            return sb.ToString();
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
