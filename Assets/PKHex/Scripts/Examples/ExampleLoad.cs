// Needs PKHeX.Core.dll: excluded from data-only player builds (see PKHexExportWindow).
#if !PKHEX_DATA_ONLY || UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PKHeX.Core;
using UnityEngine;

namespace PKHexForUnity
{
    /// <summary>
    /// Example: loads a save file of any supported game and logs its data.
    /// Add it to a GameObject, set the save path in the inspector and press Play.
    /// </summary>
    public class ExampleLoad : MonoBehaviour
    {
        // Unity truncates very long console messages: the report is logged in sections.
        private const int MAX_LOG_SECTION_LENGTH = 12000;

        [SerializeField]
        [Tooltip("Absolute path, or relative to the project folder / StreamingAssets.")]
        private string m_saveFilePath = string.Empty;

        [SerializeField]
        private SystemLanguage m_language = SystemLanguage.English;

        [SerializeField]
        private bool m_logBoxes = true;

        [SerializeField]
        private int m_maxBoxPokemonToLog = 30;

        [SerializeField]
        [Tooltip("Runs PKHeX's legality analysis on every logged Pokémon (slower).")]
        private bool m_checkLegality = false;

        [SerializeField]
        private bool m_logSpeciesDataExample = true;

        [SerializeField]
        private bool m_logRuntimeGenerationExample = true;

        private void Start()
        {
            string path = ResolvePath(m_saveFilePath);
            List<string> sections = BuildReport(path, m_language, m_logBoxes, m_maxBoxPokemonToLog, m_checkLegality,
                m_logSpeciesDataExample, m_logRuntimeGenerationExample);
            for (int i = 0; i < sections.Count; i++)
            {
                LogSection(sections[i]);
            }
        }

        /// <summary>
        /// Builds the whole report as text sections (static so it can also be used from editor tools or tests).
        /// </summary>
        public static List<string> BuildReport(string _path, SystemLanguage _language, bool _logBoxes, int _maxBoxPokemon,
            bool _checkLegality, bool _logSpeciesData, bool _logGeneration)
        {
            List<string> sections = new List<string>();
            string language = PKHexUtils.GetLanguageCode(_language);

            IPKHexSavLoader loader = PKHexSavLoaderFactory.CreateFromPath(_path);
            if (loader == null)
            {
                sections.Add("[PKHex] Could not load a save file from: " + _path);
                return sections;
            }
            SaveFile sav = loader.CurrentSaveFile;

            // 1) Save / trainer data.
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[PKHex] ===== SAVE FILE: " + Path.GetFileName(_path) + " =====");
            sb.AppendLine("Loader: " + loader.GetType().Name);
            sb.AppendLine(loader.GetSummary(_language));
            sb.AppendLine("Player: " + loader.PlayerName + " | Badges: " + loader.Badges + " | Money: " + loader.Money + "/" + loader.MaxMoney
                + " | Coins: " + loader.Coins + " | TID: " + loader.DisplayTID
                + " | Time: " + loader.PlayedHours + "h " + loader.PlayedMinutes + "m " + loader.PlayedSeconds + "s");
            sections.Add(sb.ToString());

            // 2) Party.
            PKM[] party = loader.GetParty();
            sb = new StringBuilder();
            sb.AppendLine("[PKHex] ===== PARTY (" + party.Length + ") =====");
            for (int i = 0; i < party.Length; i++)
            {
                AppendPokemon(sb, "Party " + (i + 1), loader, party[i], _language, _checkLegality);
            }
            sections.Add(sb.ToString());

            // 3) Boxes.
            if (_logBoxes)
            {
                PKM[] boxes = loader.GetBoxPokemon(true);
                sb = new StringBuilder();
                sb.AppendLine("[PKHex] ===== BOXES (" + boxes.Length + " stored, logging up to " + _maxBoxPokemon + ") =====");
                int count = Math.Min(boxes.Length, Math.Max(0, _maxBoxPokemon));
                for (int i = 0; i < count; i++)
                {
                    AppendPokemon(sb, "Box slot " + (i + 1), loader, boxes[i], _language, _checkLegality);
                    if (sb.Length > MAX_LOG_SECTION_LENGTH)
                    {
                        sections.Add(sb.ToString());
                        sb = new StringBuilder();
                    }
                }
                sections.Add(sb.ToString());
            }

            // Species used for the data/generation examples: first party member, else Pikachu.
            ushort species = 25;
            byte form = 0;
            if (party.Length > 0 && party[0].Species != 0)
            {
                species = party[0].Species;
                form = party[0].Form;
            }

            // 4) Species data of the save's game, by generation, and localized strings.
            if (_logSpeciesData)
            {
                sections.Add(BuildSpeciesSection(species, form, sav.Version, sav.Generation, language));
            }

            // 5) Runtime generation with the save as trainer.
            if (_logGeneration)
            {
                sections.Add(BuildGenerationSection(species, form, sav, _language));
            }
            return sections;
        }

        private static void AppendPokemon(StringBuilder _sb, string _label, IPKHexSavLoader _loader, PKM _pk, SystemLanguage _language, bool _checkLegality)
        {
            try
            {
                RuntimeEntityData data = _loader.GetPokemonData(_pk, _language, _checkLegality);
                _sb.AppendLine(_label + ": " + data);
            }
            catch (Exception ex)
            {
                // Per-entity isolation: one corrupt slot must not hide the rest of the save.
                _sb.AppendLine(_label + ": ERROR reading species " + _pk.Species + " (" + _pk.GetType().Name + "): " + ex.Message);
            }
        }

        private static string BuildSpeciesSection(ushort _species, byte _form, GameVersion _version, byte _generation, string _language)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[PKHex] ===== SPECIES DATA =====");
            try
            {
                RuntimeSpeciesData data = RuntimeGameData.GetSpeciesData(_species, _form, GetDataVersion(_version, _generation), _language);
                AppendSpeciesData(sb, data);

                // Same species in every generation where it exists.
                for (int generation = 1; generation <= 9; generation++)
                {
                    GameVersion version = RuntimeGameData.GetDefaultVersion(generation);
                    if (_species > RuntimeGameData.GetMaxSpeciesID(version))
                    {
                        continue;
                    }
                    RuntimeSpeciesData byGeneration = RuntimeGameData.GetSpeciesDataByGeneration(_species, 0, generation, _language);
                    sb.AppendLine("  Gen " + generation + " (" + version + "): " + byGeneration
                        + (byGeneration.IsPresentInGame ? string.Empty : " [not in game]"));
                }

                sb.AppendLine("Strings (" + _language + "): move 1 = " + RuntimeStrings.GetMoveName(1, _language)
                    + " | item 1 = " + RuntimeStrings.GetItemName(1, EntityContext.Gen9, _language)
                    + " | ability 1 = " + RuntimeStrings.GetAbilityName(1, _language)
                    + " | nature 0 = " + RuntimeStrings.GetNatureName(Nature.Hardy, _language)
                    + " | species in SV = " + RuntimeGameData.GetSpeciesInGame(GameVersion.SL).Length);
            }
            catch (Exception ex)
            {
                sb.AppendLine("ERROR reading species " + _species + ": " + ex);
            }
            return sb.ToString();
        }

        private static GameVersion GetDataVersion(GameVersion _version, byte _generation)
        {
            // Saves may report a group/unknown version; fall back to the generation's main game.
            for (int i = 0; i < RuntimeGameData.SUPPORTED_GAMES.Length; i++)
            {
                if (RuntimeGameData.SUPPORTED_GAMES[i] == _version)
                {
                    return _version;
                }
            }
            return RuntimeGameData.GetDefaultVersion(Mathf.Clamp(_generation, 1, 9));
        }

        private static void AppendSpeciesData(StringBuilder _sb, RuntimeSpeciesData _data)
        {
            _sb.AppendLine(_data.ToString());
            _sb.AppendLine("  Abilities: " + string.Join(", ", _data.AbilityNames)
                + " | Gender ratio: " + _data.GenderRatio + " | Catch rate: " + _data.CatchRate
                + " | Growth: " + _data.ExperienceGrowth + " | Forms: " + _data.FormCount);
            StringBuilder levelUp = new StringBuilder();
            for (int i = 0; i < _data.LevelUpMoves.Length; i++)
            {
                if (i != 0)
                {
                    levelUp.Append(", ");
                }
                levelUp.Append(_data.LevelUpLevels[i]).Append(": ").Append(_data.LevelUpMoveNames[i]);
            }
            _sb.AppendLine("  Level-up: " + levelUp);
            _sb.AppendLine("  Egg moves: " + string.Join(", ", _data.EggMoveNames));
            _sb.AppendLine("  TM/TR: " + _data.MachineMoves.Length + " | Tutors: " + _data.TutorMoves.Length
                + " | Evolves to: " + (_data.EvolvesToNames.Length == 0 ? "-" : string.Join(", ", _data.EvolvesToNames)));
        }

        private static string BuildGenerationSection(ushort _species, byte _form, SaveFile _sav, SystemLanguage _language)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[PKHex] ===== RUNTIME GENERATION =====");
            try
            {
                GameVersion version = GetDataVersion(_sav.Version, _sav.Generation);
                RuntimeGenerationResult result = RuntimeGenerator.GenerateDetailed(_species, _form, 50, version, _sav, false);
                sb.AppendLine("Generated for " + version + " (from encounter: " + result.FromEncounter
                    + (result.FromEncounter ? ", " + result.EncounterName : string.Empty) + "):");
                sb.AppendLine(RuntimeEntityData.FromPKM(result.Entity, PKHexUtils.GetLanguageCode(_language), true).ToString());

                PKM fromSet = RuntimeGenerator.GenerateFromShowdown("Pikachu @ Light Ball\nAbility: Static\nLevel: 50\n- Thunderbolt\n- Quick Attack", version, _sav);
                sb.AppendLine("From Showdown text:");
                sb.AppendLine(RuntimeEntityData.FromPKM(fromSet, PKHexUtils.GetLanguageCode(_language), false).ToString());
            }
            catch (Exception ex)
            {
                sb.AppendLine("ERROR generating species " + _species + ": " + ex);
            }
            return sb.ToString();
        }

        private static string ResolvePath(string _path)
        {
            if (string.IsNullOrEmpty(_path) || Path.IsPathRooted(_path))
            {
                return _path;
            }
            string streaming = Path.Combine(Application.streamingAssetsPath, _path);
            if (File.Exists(streaming))
            {
                return streaming;
            }
            return Path.Combine(Path.GetDirectoryName(Application.dataPath), _path);
        }

        private static void LogSection(string _text)
        {
            if (_text.StartsWith("[PKHex] Could not") || _text.Contains("ERROR"))
            {
                Debug.LogWarning(_text);
            }
            else
            {
                Debug.Log(_text);
            }
        }
    }
}
#endif
