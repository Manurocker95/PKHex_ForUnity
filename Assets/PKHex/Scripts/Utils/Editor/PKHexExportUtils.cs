using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PKHeX.Core;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PKHexForUnity
{
    /// <summary>
    /// A set of games sharing the same PKHeX data tables (same LearnSource), exported as one <see cref="PKHexGameData"/>.
    /// </summary>
    public class PKHexGameGroup
    {
        public int Generation;
        public string Key;
        public GameVersion[] Versions;

        public PKHexGameGroup(int _generation, string _key, params GameVersion[] _versions)
        {
            Generation = _generation;
            Key = _key;
            Versions = _versions;
        }
    }

    /// <summary> What to export. Null/empty arrays mean "all". </summary>
    public class PKHexExportOptions
    {
        public int[] Generations;
        public string[] LanguageCodes;
        public string[] GameKeys;
        public bool ExportText = true;
        public bool ExportAssets = true;
        public string OutputRoot = PKHexDataPaths.DEFAULT_EXPORT_ROOT;
    }

    /// <summary> Result of an export; failures of single resources do not stop the export. </summary>
    public class PKHexExportReport
    {
        public int GamesExported;
        public int LanguagesExported;
        public int TextFilesWritten;
        public int AssetsWritten;
        public int SpeciesExported;
        public bool Cancelled;
        public List<string> Errors = new List<string>();

        public override string ToString()
        {
            return "[PKHex] Export " + (Cancelled ? "CANCELLED" : "finished") + ": " + GamesExported + " game(s), "
                + LanguagesExported + " generation-language set(s), " + SpeciesExported + " species-form(s), "
                + AssetsWritten + " asset(s), " + TextFilesWritten + " text file(s), " + Errors.Count + " error(s).";
        }
    }

    /// <summary>
    /// Exports PKHeX data (per generation and language) to TXT files and ScriptableObjects under a Resources folder,
    /// so runtime builds can use the data without PKHeX.Core.dll (see <see cref="PKHexDatabase"/>).
    /// </summary>
    public static class PKHexExportUtils
    {
        /// <summary>
        /// Game groups per generation. Grouping follows PKHeX's GameData.GetLearnSource: versions in a group share
        /// personal data and learnsets. Colosseum/XD use Emerald's data in PKHeX and are not exported separately.
        /// </summary>
        public static readonly PKHexGameGroup[] GAME_GROUPS =
        {
            new PKHexGameGroup(1, "RB", GameVersion.RD, GameVersion.GN, GameVersion.BU),
            new PKHexGameGroup(1, "YW", GameVersion.YW),
            new PKHexGameGroup(2, "GS", GameVersion.GD, GameVersion.SI),
            new PKHexGameGroup(2, "C", GameVersion.C),
            new PKHexGameGroup(3, "RS", GameVersion.R, GameVersion.S),
            new PKHexGameGroup(3, "E", GameVersion.E),
            new PKHexGameGroup(3, "FR", GameVersion.FR),
            new PKHexGameGroup(3, "LG", GameVersion.LG),
            new PKHexGameGroup(4, "DP", GameVersion.D, GameVersion.P),
            new PKHexGameGroup(4, "Pt", GameVersion.Pt),
            new PKHexGameGroup(4, "HGSS", GameVersion.HG, GameVersion.SS),
            new PKHexGameGroup(5, "BW", GameVersion.B, GameVersion.W),
            new PKHexGameGroup(5, "B2W2", GameVersion.B2, GameVersion.W2),
            new PKHexGameGroup(6, "XY", GameVersion.X, GameVersion.Y),
            new PKHexGameGroup(6, "ORAS", GameVersion.OR, GameVersion.AS),
            new PKHexGameGroup(7, "SM", GameVersion.SN, GameVersion.MN),
            new PKHexGameGroup(7, "USUM", GameVersion.US, GameVersion.UM),
            new PKHexGameGroup(7, "GG", GameVersion.GP, GameVersion.GE),
            new PKHexGameGroup(8, "SWSH", GameVersion.SW, GameVersion.SH),
            new PKHexGameGroup(8, "BDSP", GameVersion.BD, GameVersion.SP),
            new PKHexGameGroup(8, "PLA", GameVersion.PLA),
            new PKHexGameGroup(9, "SV", GameVersion.SL, GameVersion.VL),
            new PKHexGameGroup(9, "ZA", GameVersion.ZA),
        };

        public const string SPECIES_FILE = "Species";
        public const string MOVES_FILE = "Moves";
        public const string ABILITIES_FILE = "Abilities";
        public const string NATURES_FILE = "Natures";
        public const string TYPES_FILE = "Types";
        public const string BALLS_FILE = "Balls";
        public const string ITEMS_FILE_PREFIX = "Items_";
        public const string FORMS_FILE_PREFIX = "Forms_";
        public const string MOVE_DATA_FILE_PREFIX = "MoveData_";
        public const string POKEDEX_FILE_PREFIX = "Pokedex_";

        private const string PROGRESS_TITLE = "PKHeX Export";
        private const string RESOURCES_FOLDER = "/Resources";

        // Names in the language-independent game data are not used; English is the cheapest valid language to resolve.
        private const string DATA_LANGUAGE = "en";

        /// <summary> Exports every generation, game and language. </summary>
        public static PKHexExportReport ExportAll(bool _text, bool _assets, string _outputRoot)
        {
            PKHexExportOptions options = new PKHexExportOptions();
            options.ExportText = _text;
            options.ExportAssets = _assets;
            options.OutputRoot = _outputRoot;
            return Export(options);
        }

        /// <summary> Exports every game of one generation in the given languages (null = all languages). </summary>
        public static PKHexExportReport ExportGeneration(int _generation, string[] _languageCodes, bool _text, bool _assets, string _outputRoot)
        {
            PKHexExportOptions options = new PKHexExportOptions();
            options.Generations = new int[] { _generation };
            options.LanguageCodes = _languageCodes;
            options.ExportText = _text;
            options.ExportAssets = _assets;
            options.OutputRoot = _outputRoot;
            return Export(options);
        }

        /// <summary> Exports one language of every generation. </summary>
        public static PKHexExportReport ExportLanguage(string _languageCode, bool _text, bool _assets, string _outputRoot)
        {
            PKHexExportOptions options = new PKHexExportOptions();
            options.LanguageCodes = new string[] { _languageCode };
            options.ExportText = _text;
            options.ExportAssets = _assets;
            options.OutputRoot = _outputRoot;
            return Export(options);
        }

        /// <summary> Game groups of a generation. </summary>
        public static List<PKHexGameGroup> GetGameGroups(int _generation)
        {
            List<PKHexGameGroup> result = new List<PKHexGameGroup>();
            for (int i = 0; i < GAME_GROUPS.Length; i++)
            {
                if (GAME_GROUPS[i].Generation == _generation)
                {
                    result.Add(GAME_GROUPS[i]);
                }
            }
            return result;
        }

        /// <summary> Runs an export. Each game and each language is isolated: a failure is logged and the rest continues. </summary>
        public static PKHexExportReport Export(PKHexExportOptions _options)
        {
            PKHexExportReport report = new PKHexExportReport();
            string root = NormalizeRoot(_options.OutputRoot);
            if (!root.StartsWith("Assets/") && root != "Assets")
            {
                AddError(report, "Output root must be inside Assets: " + root);
                return report;
            }
            if (!root.EndsWith(RESOURCES_FOLDER))
            {
                Debug.LogWarning("[PKHex] Output root does not end with /Resources; PKHexDatabase will not find the data at runtime: " + root);
            }

            int[] generations = GetGenerations(_options.Generations);
            string[] languages = GetLanguages(_options.LanguageCodes);
            int totalSteps = CountSteps(generations, languages, _options.GameKeys);
            int step = 0;

            try
            {
                for (int g = 0; g < generations.Length && !report.Cancelled; g++)
                {
                    int generation = generations[g];
                    List<PKHexGameGroup> groups = FilterGroups(GetGameGroups(generation), _options.GameKeys);
                    if (groups.Count == 0)
                    {
                        continue;
                    }

                    List<PKHexGameGroup> exportedGroups = new List<PKHexGameGroup>();
                    List<PKHexGameData> games = new List<PKHexGameData>();
                    for (int i = 0; i < groups.Count; i++)
                    {
                        if (UpdateProgress(report, "Gen " + generation + ": game data " + groups[i].Key, ++step, totalSteps))
                        {
                            break;
                        }
                        try
                        {
                            PKHexGameData game = BuildGameData(groups[i], report);
                            games.Add(game);
                            exportedGroups.Add(groups[i]);
                        }
                        catch (Exception ex)
                        {
                            AddError(report, "Gen " + generation + " game " + groups[i].Key + " failed: " + ex);
                        }
                    }

                    for (int l = 0; l < languages.Length && !report.Cancelled && games.Count != 0; l++)
                    {
                        string code = languages[l];
                        if (UpdateProgress(report, "Gen " + generation + ": " + PKHexDataPaths.GetLanguageFolder(code), ++step, totalSteps))
                        {
                            break;
                        }
                        try
                        {
                            PKHexStringTable strings = BuildStringTable(generation, code, exportedGroups, games);
                            string folder = root + "/" + PKHexDataPaths.GetLanguagePath(generation, PKHexDataPaths.GetLanguageFolder(code));
                            if (_options.ExportText)
                            {
                                WriteTextFiles(folder, strings, games, report);
                            }
                            if (_options.ExportAssets)
                            {
                                SaveAsset(strings, folder, PKHexDataPaths.STRINGS_ASSET_NAME, report);
                            }
                            else
                            {
                                Object.DestroyImmediate(strings);
                            }
                            report.LanguagesExported++;
                        }
                        catch (Exception ex)
                        {
                            AddError(report, "Gen " + generation + " language " + code + " failed: " + ex);
                        }
                    }

                    // Game assets are saved last because the text export above reads the in-memory instances.
                    for (int i = 0; i < games.Count; i++)
                    {
                        if (_options.ExportAssets)
                        {
                            try
                            {
                                string folder = root + "/" + PKHexDataPaths.GetGenerationPath(generation) + "/" + PKHexDataPaths.GAMES_FOLDER;
                                SaveAsset(games[i], folder, games[i].GameKey, report);
                            }
                            catch (Exception ex)
                            {
                                AddError(report, "Gen " + generation + " game asset " + games[i].GameKey + " failed to save: " + ex);
                            }
                        }
                        else
                        {
                            Object.DestroyImmediate(games[i]);
                        }
                    }
                    report.GamesExported += games.Count;
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                if (_options.ExportAssets)
                {
                    RebuildIndex(root, report);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            if (report.Errors.Count != 0)
            {
                Debug.LogWarning(report.ToString());
            }
            else
            {
                Debug.Log(report.ToString());
            }
            return report;
        }

        /// <summary> Builds (in memory) the language-independent data of a game group. </summary>
        public static PKHexGameData BuildGameData(PKHexGameGroup _group, PKHexExportReport _report)
        {
            GameVersion version = _group.Versions[0];
            EntityContext context = RuntimeGameData.GetContext(version);
            int[] limits = GetLimits(version, _report);
            int maxSpecies = RuntimeGameData.GetMaxSpeciesID(version);

            List<PKHexSpeciesEntry> species = new List<PKHexSpeciesEntry>();
            for (int s = 1; s <= maxSpecies; s++)
            {
                int formCount = 1;
                try
                {
                    formCount = Math.Max(1, (int)RuntimeGameData.GetPersonalInfo((ushort)s, 0, version).FormCount);
                }
                catch (Exception ex)
                {
                    AddError(_report, _group.Key + " species " + s + ": personal data unavailable: " + ex.Message);
                    continue;
                }
                for (int f = 0; f < formCount; f++)
                {
                    try
                    {
                        RuntimeSpeciesData data = RuntimeGameData.GetSpeciesData((ushort)s, (byte)f, version, DATA_LANGUAGE);
                        species.Add(ToEntry(data));
                    }
                    catch (Exception ex)
                    {
                        AddError(_report, _group.Key + " species " + s + " form " + f + " failed: " + ex.Message);
                    }
                }
            }
            if (_report != null)
            {
                _report.SpeciesExported += species.Count;
            }

            List<PKHexMoveEntry> moves = new List<PKHexMoveEntry>(limits[1] + 1);
            for (int m = 0; m <= limits[1]; m++)
            {
                PKHexMoveEntry move = new PKHexMoveEntry();
                move.Move = m;
                move.Type = RuntimeGameData.GetMoveType((ushort)m, version);
                move.PP = RuntimeGameData.GetMovePP((ushort)m, version);
                moves.Add(move);
            }

            string[] versions = new string[_group.Versions.Length];
            for (int i = 0; i < versions.Length; i++)
            {
                versions[i] = _group.Versions[i].ToString();
            }

            PKHexGameData game = ScriptableObject.CreateInstance<PKHexGameData>();
            game.name = _group.Key;
            game.SetData(_group.Generation, _group.Key, versions, context.ToString(), limits, species, moves);
            return game;
        }

        /// <summary> Builds (in memory) the strings of a generation in one language for the given games. </summary>
        public static PKHexStringTable BuildStringTable(int _generation, string _languageCode, List<PKHexGameGroup> _groups, List<PKHexGameData> _games)
        {
            GameStrings strings = RuntimeStrings.GetStrings(_languageCode);
            int maxSpecies = 0;
            int maxMove = 0;
            int maxAbility = 0;
            for (int i = 0; i < _games.Count; i++)
            {
                maxSpecies = Math.Max(maxSpecies, _games[i].MaxSpeciesID);
                maxMove = Math.Max(maxMove, _games[i].MaxMoveID);
                maxAbility = Math.Max(maxAbility, _games[i].MaxAbilityID);
            }

            List<PKHexGameStrings> games = new List<PKHexGameStrings>();
            for (int i = 0; i < _groups.Count; i++)
            {
                games.Add(BuildGameStrings(_groups[i], _games[i], strings, _languageCode));
            }

            string folder = PKHexDataPaths.GetLanguageFolder(_languageCode);
            PKHexStringTable table = ScriptableObject.CreateInstance<PKHexStringTable>();
            table.name = PKHexDataPaths.STRINGS_ASSET_NAME;
            table.SetData(_generation, _languageCode, folder,
                Take(strings.specieslist, maxSpecies + 1),
                Take(strings.movelist, maxMove + 1),
                Take(strings.abilitylist, maxAbility + 1),
                Take(strings.natures, strings.natures.Length),
                Take(strings.types, strings.types.Length),
                Take(strings.balllist, strings.balllist.Length),
                games);
            return table;
        }

        /// <summary> Writes all .txt files of one generation-language folder. </summary>
        public static void WriteTextFiles(string _folder, PKHexStringTable _strings, List<PKHexGameData> _games, PKHexExportReport _report)
        {
            EnsureFolder(_folder);
            WriteText(_folder, SPECIES_FILE, PKHexDataText.FormatIdList(_strings.SpeciesNames), _report);
            WriteText(_folder, MOVES_FILE, PKHexDataText.FormatIdList(_strings.MoveNames), _report);
            WriteText(_folder, ABILITIES_FILE, PKHexDataText.FormatIdList(_strings.AbilityNames), _report);
            WriteText(_folder, NATURES_FILE, PKHexDataText.FormatIdList(_strings.NatureNames), _report);
            WriteText(_folder, TYPES_FILE, PKHexDataText.FormatIdList(_strings.TypeNames), _report);
            WriteText(_folder, BALLS_FILE, PKHexDataText.FormatIdList(_strings.BallNames), _report);
            for (int i = 0; i < _games.Count; i++)
            {
                PKHexGameData game = _games[i];
                PKHexGameStrings gameStrings = _strings.GetGameStrings(game.GameKey);
                try
                {
                    WriteText(_folder, ITEMS_FILE_PREFIX + game.GameKey, PKHexDataText.FormatIdList(gameStrings.Items), _report);
                    WriteText(_folder, FORMS_FILE_PREFIX + game.GameKey, PKHexDataText.FormatFormList(gameStrings, _strings), _report);
                    WriteText(_folder, MOVE_DATA_FILE_PREFIX + game.GameKey, PKHexDataText.FormatMoveList(game, _strings), _report);
                    WriteText(_folder, POKEDEX_FILE_PREFIX + game.GameKey, PKHexDataText.FormatPokedex(game, _strings), _report);
                }
                catch (Exception ex)
                {
                    AddError(_report, "Text files of " + game.GameKey + " in " + _folder + " failed: " + ex);
                }
            }
        }

        /// <summary> Rebuilds Data/PKHexDataIndex.asset from the exported files found under <paramref name="_outputRoot"/>. </summary>
        public static PKHexDataIndex RebuildIndex(string _outputRoot, PKHexExportReport _report)
        {
            string root = NormalizeRoot(_outputRoot);
            List<PKHexExportedGeneration> generations = new List<PKHexExportedGeneration>();
            for (int generation = PKHexDataPaths.MIN_GENERATION; generation <= PKHexDataPaths.MAX_GENERATION; generation++)
            {
                string generationFolder = root + "/" + PKHexDataPaths.GetGenerationPath(generation);
                if (!Directory.Exists(generationFolder))
                {
                    continue;
                }

                List<string> gameKeys = new List<string>();
                List<PKHexGameGroup> groups = GetGameGroups(generation);
                for (int i = 0; i < groups.Count; i++)
                {
                    string path = generationFolder + "/" + PKHexDataPaths.GAMES_FOLDER + "/" + groups[i].Key + PKHexDataPaths.ASSET_EXTENSION;
                    if (File.Exists(path))
                    {
                        gameKeys.Add(groups[i].Key);
                    }
                }

                List<string> languages = new List<string>();
                for (int i = 0; i < PKHexDataPaths.LANGUAGE_FOLDERS.Length; i++)
                {
                    string path = generationFolder + "/" + PKHexDataPaths.LANGUAGE_FOLDERS[i] + "/" + PKHexDataPaths.STRINGS_ASSET_NAME + PKHexDataPaths.ASSET_EXTENSION;
                    if (File.Exists(path))
                    {
                        languages.Add(PKHexDataPaths.LANGUAGE_FOLDERS[i]);
                    }
                }

                if (gameKeys.Count == 0 && languages.Count == 0)
                {
                    continue;
                }
                PKHexExportedGeneration entry = new PKHexExportedGeneration();
                entry.Generation = generation;
                entry.GameKeys = gameKeys.ToArray();
                entry.LanguageFolders = languages.ToArray();
                generations.Add(entry);
            }

            PKHexDataIndex index = ScriptableObject.CreateInstance<PKHexDataIndex>();
            index.name = PKHexDataPaths.INDEX_ASSET_NAME;
            index.SetData(generations);
            PKHexDataIndex saved = SaveAsset(index, root + "/" + PKHexDataPaths.DATA_FOLDER, PKHexDataPaths.INDEX_ASSET_NAME, _report) as PKHexDataIndex;
            AssetDatabase.SaveAssets();
            return saved;
        }

        private static PKHexGameStrings BuildGameStrings(PKHexGameGroup _group, PKHexGameData _game, GameStrings _strings, string _languageCode)
        {
            GameVersion version = _group.Versions[0];
            EntityContext context = RuntimeGameData.GetContext(version);

            PKHexGameStrings result = new PKHexGameStrings();
            result.GameKey = _group.Key;

            string[] names = new string[_group.Versions.Length];
            for (int i = 0; i < names.Length; i++)
            {
                int id = (int)_group.Versions[i];
                names[i] = id < _strings.gamelist.Length ? _strings.gamelist[id] : _group.Versions[i].ToString();
            }
            result.GameName = string.Join(" / ", names);

            string[] items = _strings.GetItemStrings(context, version);
            result.Items = Take(items, Math.Min(items.Length, _game.MaxItemID + 1));

            List<PKHexFormName> forms = new List<PKHexFormName>();
            for (int s = 1; s <= _game.MaxSpeciesID; s++)
            {
                string[] formNames = RuntimeStrings.GetFormNames((ushort)s, context, _languageCode);
                if (formNames.Length <= 1)
                {
                    continue;
                }
                for (int f = 0; f < formNames.Length; f++)
                {
                    PKHexFormName form = new PKHexFormName();
                    form.Species = s;
                    form.Form = f;
                    form.Name = formNames[f];
                    forms.Add(form);
                }
            }
            result.Forms = forms.ToArray();
            return result;
        }

        private static PKHexSpeciesEntry ToEntry(RuntimeSpeciesData _data)
        {
            PKHexSpeciesEntry entry = new PKHexSpeciesEntry();
            entry.Species = _data.Species;
            entry.Form = _data.Form;
            entry.FormCount = _data.FormCount;
            entry.IsPresentInGame = _data.IsPresentInGame;
            entry.Type1 = _data.Type1;
            entry.Type2 = _data.Type2;
            entry.BaseStats = (int[])_data.BaseStats.Clone();
            entry.BaseStatTotal = _data.BaseStatTotal;
            entry.Abilities = (int[])_data.Abilities.Clone();
            entry.GenderRatio = _data.GenderRatio;
            entry.CatchRate = _data.CatchRate;
            entry.BaseExperience = _data.BaseExperience;
            entry.ExperienceGrowth = _data.ExperienceGrowth;
            entry.EggGroup1 = _data.EggGroup1;
            entry.EggGroup2 = _data.EggGroup2;
            entry.Height = _data.Height;
            entry.Weight = _data.Weight;
            entry.LevelUpMoves = ToInt(_data.LevelUpMoves);
            entry.LevelUpLevels = ToInt(_data.LevelUpLevels);
            entry.InitialMoves = ToInt(_data.InitialMoves);
            entry.EggMoves = ToInt(_data.EggMoves);
            entry.MachineMoves = ToInt(_data.MachineMoves);
            entry.TutorMoves = ToInt(_data.TutorMoves);

            PKHexEvolutionEntry[] evolutions = new PKHexEvolutionEntry[_data.EvolutionSpecies.Length];
            for (int i = 0; i < evolutions.Length; i++)
            {
                PKHexEvolutionEntry evo = new PKHexEvolutionEntry();
                evo.Species = _data.EvolutionSpecies[i];
                evo.Form = _data.EvolutionForm[i];
                evo.Method = _data.EvolutionMethod[i];
                evo.MethodName = _data.EvolutionMethodName[i];
                evo.Argument = _data.EvolutionArgument[i];
                evo.Level = _data.EvolutionLevel[i];
                evo.RequiresLevelUp = _data.EvolutionRequiresLevelUp[i];
                evolutions[i] = evo;
            }
            entry.Evolutions = evolutions;
            return entry;
        }

        private static int[] GetLimits(GameVersion _version, PKHexExportReport _report)
        {
            try
            {
                return RuntimeGameData.GetLimits(_version);
            }
            catch (Exception ex)
            {
                // Fallback: species from the personal table, the rest from the English string lists (upper bounds).
                AddError(_report, "Limits of " + _version + " unavailable (" + ex.Message + "); using table sizes instead.");
                GameStrings strings = RuntimeStrings.GetStrings(DATA_LANGUAGE);
                EntityContext context = RuntimeGameData.GetContext(_version);
                return new int[]
                {
                    RuntimeGameData.GetMaxSpeciesID(_version), strings.movelist.Length - 1,
                    strings.GetItemStrings(context, _version).Length - 1, strings.abilitylist.Length - 1
                };
            }
        }

        private static ScriptableObject SaveAsset(ScriptableObject _asset, string _folder, string _name, PKHexExportReport _report)
        {
            EnsureFolder(_folder);
            string path = _folder + "/" + _name + PKHexDataPaths.ASSET_EXTENSION;
            _asset.name = _name;
            ScriptableObject existing = AssetDatabase.LoadAssetAtPath(path, _asset.GetType()) as ScriptableObject;
            ScriptableObject result;
            if (existing != null)
            {
                // Overwrite in place so the asset keeps its GUID and existing references stay valid.
                EditorUtility.CopySerialized(_asset, existing);
                EditorUtility.SetDirty(existing);
                Object.DestroyImmediate(_asset);
                result = existing;
            }
            else
            {
                AssetDatabase.CreateAsset(_asset, path);
                result = _asset;
            }
            if (_report != null)
            {
                _report.AssetsWritten++;
            }
            return result;
        }

        private static void WriteText(string _folder, string _fileName, string _content, PKHexExportReport _report)
        {
            File.WriteAllText(_folder + "/" + _fileName + PKHexDataPaths.TEXT_EXTENSION, _content, new UTF8Encoding(false));
            if (_report != null)
            {
                _report.TextFilesWritten++;
            }
        }

        /// <summary> Creates every missing folder of an "Assets/..." path through the AssetDatabase. </summary>
        private static void EnsureFolder(string _folder)
        {
            if (AssetDatabase.IsValidFolder(_folder))
            {
                return;
            }
            int slash = _folder.LastIndexOf('/');
            string parent = _folder.Substring(0, slash);
            EnsureFolder(parent);
            if (Directory.Exists(_folder))
            {
                // Exists on disk but was never imported (e.g. created outside Unity): import it instead of creating a
                // second folder, which Unity would name "<folder> 1".
                AssetDatabase.Refresh();
                if (AssetDatabase.IsValidFolder(_folder))
                {
                    return;
                }
            }
            AssetDatabase.CreateFolder(parent, _folder.Substring(slash + 1));
        }

        private static string NormalizeRoot(string _root)
        {
            string root = string.IsNullOrEmpty(_root) ? PKHexDataPaths.DEFAULT_EXPORT_ROOT : _root;
            return root.Replace('\\', '/').TrimEnd('/');
        }

        private static int[] GetGenerations(int[] _generations)
        {
            if (_generations != null && _generations.Length != 0)
            {
                return _generations;
            }
            int[] all = new int[PKHexDataPaths.MAX_GENERATION - PKHexDataPaths.MIN_GENERATION + 1];
            for (int i = 0; i < all.Length; i++)
            {
                all[i] = PKHexDataPaths.MIN_GENERATION + i;
            }
            return all;
        }

        private static string[] GetLanguages(string[] _languageCodes)
        {
            return _languageCodes != null && _languageCodes.Length != 0 ? _languageCodes : PKHexDataPaths.LANGUAGE_CODES;
        }

        private static List<PKHexGameGroup> FilterGroups(List<PKHexGameGroup> _groups, string[] _gameKeys)
        {
            if (_gameKeys == null || _gameKeys.Length == 0)
            {
                return _groups;
            }
            List<PKHexGameGroup> result = new List<PKHexGameGroup>();
            for (int i = 0; i < _groups.Count; i++)
            {
                if (Array.IndexOf(_gameKeys, _groups[i].Key) >= 0)
                {
                    result.Add(_groups[i]);
                }
            }
            return result;
        }

        private static int CountSteps(int[] _generations, string[] _languages, string[] _gameKeys)
        {
            int steps = 0;
            for (int i = 0; i < _generations.Length; i++)
            {
                int groups = FilterGroups(GetGameGroups(_generations[i]), _gameKeys).Count;
                if (groups != 0)
                {
                    steps += groups + _languages.Length;
                }
            }
            return Math.Max(1, steps);
        }

        /// <summary> Returns true (and marks the report) when the user cancels. </summary>
        private static bool UpdateProgress(PKHexExportReport _report, string _info, int _step, int _total)
        {
            if (EditorUtility.DisplayCancelableProgressBar(PROGRESS_TITLE, _info, (float)_step / _total))
            {
                _report.Cancelled = true;
            }
            return _report.Cancelled;
        }

        private static void AddError(PKHexExportReport _report, string _message)
        {
            Debug.LogError("[PKHex] " + _message);
            if (_report != null)
            {
                _report.Errors.Add(_message);
            }
        }

        private static string[] Take(IList<string> _list, int _count)
        {
            int count = Math.Min(_count, _list.Count);
            string[] result = new string[Math.Max(0, count)];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = _list[i];
            }
            return result;
        }

        private static int[] ToInt(ushort[] _values)
        {
            int[] result = new int[_values.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = _values[i];
            }
            return result;
        }

        private static int[] ToInt(byte[] _values)
        {
            int[] result = new int[_values.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = _values[i];
            }
            return result;
        }
    }
}
