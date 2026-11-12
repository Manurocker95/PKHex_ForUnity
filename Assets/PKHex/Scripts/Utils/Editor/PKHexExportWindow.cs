using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PKHexForUnity
{
    /// <summary>
    /// Editor window (Window/PKHeX/Data Exporter) to export PKHeX data per generation, game and language, and to set up
    /// data-only builds where PKHeX.Core.dll is not shipped (e.g. New Nintendo 3DS).
    /// </summary>
    public class PKHexExportWindow : EditorWindow
    {
        private const string MENU_PATH = "Window/PKHeX/Data Exporter";
        private const string PREFS_PREFIX = "PKHexExportWindow.";
        private const string CORE_DLL_NAME = "PKHeX.Core.dll";
        public const string DATA_ONLY_DEFINE = "PKHEX_DATA_ONLY";

        private string m_outputRoot = PKHexDataPaths.DEFAULT_EXPORT_ROOT;
        private bool m_exportText = true;
        private bool m_exportAssets = true;
        private bool m_exportCsv;
        private bool m_csvHeader = true;
        private PKHexTextFormat m_textFormat = PKHexTextFormat.NamesOnly;
        private bool[] m_generations;
        private bool[] m_generationFoldouts;
        private Dictionary<string, bool> m_games = new Dictionary<string, bool>();
        private bool[] m_languages;
        private int m_singleGeneration = PKHexDataPaths.MAX_GENERATION;
        private Vector2 m_scroll;
        private string m_lastReport = string.Empty;

        [MenuItem(MENU_PATH)]
        public static void Open()
        {
            GetWindow<PKHexExportWindow>(false, "PKHeX Export", true);
        }

        protected virtual void OnEnable()
        {
            int generationCount = PKHexDataPaths.MAX_GENERATION + 1;
            m_generations = new bool[generationCount];
            m_generationFoldouts = new bool[generationCount];
            m_languages = new bool[PKHexDataPaths.LANGUAGE_CODES.Length];

            m_outputRoot = EditorPrefs.GetString(PREFS_PREFIX + "OutputRoot", PKHexDataPaths.DEFAULT_EXPORT_ROOT);
            m_exportText = EditorPrefs.GetBool(PREFS_PREFIX + "ExportText", true);
            m_exportAssets = EditorPrefs.GetBool(PREFS_PREFIX + "ExportAssets", true);
            m_exportCsv = EditorPrefs.GetBool(PREFS_PREFIX + "ExportCsv", false);
            m_csvHeader = EditorPrefs.GetBool(PREFS_PREFIX + "CsvHeader", true);
            m_textFormat = (PKHexTextFormat)EditorPrefs.GetInt(PREFS_PREFIX + "TextFormat", (int)PKHexTextFormat.NamesOnly);
            m_singleGeneration = EditorPrefs.GetInt(PREFS_PREFIX + "SingleGeneration", PKHexDataPaths.MAX_GENERATION);
            for (int g = PKHexDataPaths.MIN_GENERATION; g <= PKHexDataPaths.MAX_GENERATION; g++)
            {
                m_generations[g] = EditorPrefs.GetBool(PREFS_PREFIX + "Gen" + g, g == PKHexDataPaths.MAX_GENERATION);
            }
            for (int i = 0; i < PKHexExportUtils.GAME_GROUPS.Length; i++)
            {
                string key = PKHexExportUtils.GAME_GROUPS[i].Key;
                m_games[key] = EditorPrefs.GetBool(PREFS_PREFIX + "Game." + key, true);
            }
            for (int i = 0; i < m_languages.Length; i++)
            {
                m_languages[i] = EditorPrefs.GetBool(PREFS_PREFIX + "Lang." + PKHexDataPaths.LANGUAGE_CODES[i], PKHexDataPaths.LANGUAGE_CODES[i] == "en");
            }
        }

        protected virtual void OnDisable()
        {
            EditorPrefs.SetString(PREFS_PREFIX + "OutputRoot", m_outputRoot);
            EditorPrefs.SetBool(PREFS_PREFIX + "ExportText", m_exportText);
            EditorPrefs.SetBool(PREFS_PREFIX + "ExportAssets", m_exportAssets);
            EditorPrefs.SetBool(PREFS_PREFIX + "ExportCsv", m_exportCsv);
            EditorPrefs.SetBool(PREFS_PREFIX + "CsvHeader", m_csvHeader);
            EditorPrefs.SetInt(PREFS_PREFIX + "TextFormat", (int)m_textFormat);
            EditorPrefs.SetInt(PREFS_PREFIX + "SingleGeneration", m_singleGeneration);
            for (int g = PKHexDataPaths.MIN_GENERATION; g <= PKHexDataPaths.MAX_GENERATION; g++)
            {
                EditorPrefs.SetBool(PREFS_PREFIX + "Gen" + g, m_generations[g]);
            }
            foreach (KeyValuePair<string, bool> pair in m_games)
            {
                EditorPrefs.SetBool(PREFS_PREFIX + "Game." + pair.Key, pair.Value);
            }
            for (int i = 0; i < m_languages.Length; i++)
            {
                EditorPrefs.SetBool(PREFS_PREFIX + "Lang." + PKHexDataPaths.LANGUAGE_CODES[i], m_languages[i]);
            }
        }

        protected virtual void OnGUI()
        {
            m_scroll = EditorGUILayout.BeginScrollView(m_scroll);

            EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            m_outputRoot = EditorGUILayout.TextField("Resources folder", m_outputRoot);
            if (GUILayout.Button("Default", GUILayout.Width(60)))
            {
                m_outputRoot = PKHexDataPaths.DEFAULT_EXPORT_ROOT;
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndHorizontal();
            m_exportText = EditorGUILayout.Toggle("Export .txt files", m_exportText);
            GUI.enabled = m_exportText;
            EditorGUI.indentLevel++;
            m_textFormat = (PKHexTextFormat)EditorGUILayout.EnumPopup("List format", m_textFormat);
            EditorGUI.indentLevel--;
            GUI.enabled = true;
            m_exportAssets = EditorGUILayout.Toggle("Export ScriptableObjects", m_exportAssets);
            m_exportCsv = EditorGUILayout.Toggle("Export CSV (all languages)", m_exportCsv);
            GUI.enabled = m_exportCsv;
            EditorGUI.indentLevel++;
            m_csvHeader = EditorGUILayout.Toggle("Header row", m_csvHeader);
            EditorGUI.indentLevel--;
            GUI.enabled = true;
            EditorGUILayout.HelpBox("Files go to <folder>/Data/GenN/<Language>/ (texts, strings), <folder>/Data/GenN/Games/ (Pokémon data) "
                + "and <folder>/Data/GenN/CSV/ (one CSV per category: ID,English,<selected languages>).\n"
                + "List format: NamesOnly = one name per line (line N from 0 = ID N); CommaSeparated = \"ID,Name\".", MessageType.None);

            EditorGUILayout.Space();
            DrawGenerations();
            EditorGUILayout.Space();
            DrawLanguages();
            EditorGUILayout.Space();
            DrawExportButtons();
            EditorGUILayout.Space();
            DrawDataOnlySetup();

            if (!string.IsNullOrEmpty(m_lastReport))
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(m_lastReport, MessageType.Info);
            }
            EditorGUILayout.EndScrollView();
        }

        protected virtual void DrawGenerations()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Generations and games", EditorStyles.boldLabel);
            if (GUILayout.Button("All", GUILayout.Width(40)))
            {
                SetAllGenerations(true);
            }
            if (GUILayout.Button("None", GUILayout.Width(45)))
            {
                SetAllGenerations(false);
            }
            EditorGUILayout.EndHorizontal();

            for (int g = PKHexDataPaths.MIN_GENERATION; g <= PKHexDataPaths.MAX_GENERATION; g++)
            {
                EditorGUILayout.BeginHorizontal();
                m_generations[g] = EditorGUILayout.Toggle(m_generations[g], GUILayout.Width(16));
                m_generationFoldouts[g] = EditorGUILayout.Foldout(m_generationFoldouts[g], "Generation " + g + "  (" + GetGameSummary(g) + ")");
                EditorGUILayout.EndHorizontal();
                if (!m_generationFoldouts[g])
                {
                    continue;
                }
                EditorGUI.indentLevel += 2;
                List<PKHexGameGroup> groups = PKHexExportUtils.GetGameGroups(g);
                for (int i = 0; i < groups.Count; i++)
                {
                    m_games[groups[i].Key] = EditorGUILayout.ToggleLeft(groups[i].Key + "  (" + GetVersionList(groups[i]) + ")", m_games[groups[i].Key]);
                }
                EditorGUI.indentLevel -= 2;
            }
        }

        protected virtual void DrawLanguages()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Languages", EditorStyles.boldLabel);
            if (GUILayout.Button("All", GUILayout.Width(40)))
            {
                SetAll(m_languages, true);
            }
            if (GUILayout.Button("None", GUILayout.Width(45)))
            {
                SetAll(m_languages, false);
            }
            EditorGUILayout.EndHorizontal();

            const int COLUMNS = 2;
            for (int i = 0; i < m_languages.Length; i += COLUMNS)
            {
                EditorGUILayout.BeginHorizontal();
                for (int c = 0; c < COLUMNS && i + c < m_languages.Length; c++)
                {
                    int index = i + c;
                    m_languages[index] = EditorGUILayout.ToggleLeft(PKHexDataPaths.LANGUAGE_FOLDERS[index] + " (" + PKHexDataPaths.LANGUAGE_CODES[index] + ")", m_languages[index]);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        protected virtual void DrawExportButtons()
        {
            EditorGUILayout.LabelField("Export", EditorStyles.boldLabel);
            GUI.enabled = m_exportText || m_exportAssets || m_exportCsv;

            if (GUILayout.Button("Export selection (checked generations, games and languages)"))
            {
                PKHexExportOptions options = CreateOptions();
                options.Generations = GetSelectedGenerations();
                options.GameKeys = GetSelectedGames();
                options.LanguageCodes = GetSelectedLanguages();
                RunExport(options);
            }

            EditorGUILayout.BeginHorizontal();
            m_singleGeneration = EditorGUILayout.IntSlider("Generation", m_singleGeneration, PKHexDataPaths.MIN_GENERATION, PKHexDataPaths.MAX_GENERATION);
            if (GUILayout.Button("Export all of Gen " + m_singleGeneration, GUILayout.Width(150)))
            {
                PKHexExportOptions options = CreateOptions();
                options.Generations = new int[] { m_singleGeneration };
                options.LanguageCodes = GetSelectedLanguages();
                RunExport(options);
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Export CSV only of Gen " + m_singleGeneration + " (selected languages, English first)"))
            {
                PKHexExportOptions options = CreateOptions();
                options.ExportText = false;
                options.ExportAssets = false;
                options.ExportCsv = true;
                options.Generations = new int[] { m_singleGeneration };
                options.LanguageCodes = GetSelectedLanguages();
                RunExport(options);
            }

            if (GUILayout.Button("Export all generations (selected languages)"))
            {
                PKHexExportOptions options = CreateOptions();
                options.LanguageCodes = GetSelectedLanguages();
                RunExport(options);
            }

            if (GUILayout.Button("Export EVERYTHING (all generations, games and languages)"))
            {
                if (EditorUtility.DisplayDialog("PKHeX Export", "Export every generation, game and language? This takes a while and creates many files.", "Export", "Cancel"))
                {
                    RunExport(CreateOptions());
                }
            }

            GUI.enabled = true;
            if (GUILayout.Button("Rebuild index only"))
            {
                PKHexExportUtils.RebuildIndex(m_outputRoot, null);
                m_lastReport = "Index rebuilt.";
            }
            GUI.enabled = true;
        }

        protected virtual void DrawDataOnlySetup()
        {
            EditorGUILayout.LabelField("Data-only build (no PKHeX.Core.dll in the player)", EditorStyles.boldLabel);
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);
            string dllPath = FindCoreDll();
            bool defined = HasDefine(group, DATA_ONLY_DEFINE);

            EditorGUILayout.HelpBox(
                "Active platform: " + target + "\n" +
                "PKHeX.Core.dll: " + (dllPath ?? "not found") + "\n" +
                DATA_ONLY_DEFINE + " defined for " + group + ": " + (defined ? "yes" : "no") + "\n\n" +
                "Data-only excludes the DLL from this platform's player (the Editor keeps it for exporting) and defines " +
                DATA_ONLY_DEFINE + ", which removes the save loaders/utils that need the DLL from the player build. " +
                "Use PKHexDatabase to read the exported data.", MessageType.None);

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = dllPath != null;
            if (GUILayout.Button("Enable data-only for " + target))
            {
                SetDataOnly(dllPath, target, group, true);
            }
            if (GUILayout.Button("Disable data-only for " + target))
            {
                SetDataOnly(dllPath, target, group, false);
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

        private PKHexExportOptions CreateOptions()
        {
            PKHexExportOptions options = new PKHexExportOptions();
            options.OutputRoot = m_outputRoot;
            options.ExportText = m_exportText;
            options.ExportAssets = m_exportAssets;
            options.ExportCsv = m_exportCsv;
            options.CsvHeader = m_csvHeader;
            options.TextFormat = m_textFormat;
            return options;
        }

        private void RunExport(PKHexExportOptions _options)
        {
            if (_options.LanguageCodes != null && _options.LanguageCodes.Length == 0)
            {
                EditorUtility.DisplayDialog("PKHeX Export", "Select at least one language.", "OK");
                return;
            }
            if (_options.Generations != null && _options.Generations.Length == 0)
            {
                EditorUtility.DisplayDialog("PKHeX Export", "Select at least one generation.", "OK");
                return;
            }
            PKHexExportReport report = PKHexExportUtils.Export(_options);
            m_lastReport = report.ToString();
            if (report.Errors.Count != 0)
            {
                m_lastReport += "\nFirst error: " + report.Errors[0] + "\n(See the Console for all errors.)";
            }
        }

        private int[] GetSelectedGenerations()
        {
            List<int> result = new List<int>();
            for (int g = PKHexDataPaths.MIN_GENERATION; g <= PKHexDataPaths.MAX_GENERATION; g++)
            {
                if (m_generations[g])
                {
                    result.Add(g);
                }
            }
            return result.ToArray();
        }

        private string[] GetSelectedGames()
        {
            List<string> result = new List<string>();
            foreach (KeyValuePair<string, bool> pair in m_games)
            {
                if (pair.Value)
                {
                    result.Add(pair.Key);
                }
            }
            // An empty selection would mean "all games" to the exporter; use a key that matches nothing instead.
            return result.Count != 0 ? result.ToArray() : new string[] { string.Empty };
        }

        private string[] GetSelectedLanguages()
        {
            List<string> result = new List<string>();
            for (int i = 0; i < m_languages.Length; i++)
            {
                if (m_languages[i])
                {
                    result.Add(PKHexDataPaths.LANGUAGE_CODES[i]);
                }
            }
            return result.ToArray();
        }

        private string GetGameSummary(int _generation)
        {
            List<PKHexGameGroup> groups = PKHexExportUtils.GetGameGroups(_generation);
            List<string> keys = new List<string>();
            for (int i = 0; i < groups.Count; i++)
            {
                if (m_games[groups[i].Key])
                {
                    keys.Add(groups[i].Key);
                }
            }
            return keys.Count == 0 ? "no games" : string.Join(", ", keys.ToArray());
        }

        private static string GetVersionList(PKHexGameGroup _group)
        {
            string[] names = new string[_group.Versions.Length];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = _group.Versions[i].ToString();
            }
            return string.Join("/", names);
        }

        private void SetAllGenerations(bool _value)
        {
            SetAll(m_generations, _value);
            List<string> keys = new List<string>(m_games.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                m_games[keys[i]] = _value;
            }
        }

        private static void SetAll(bool[] _values, bool _value)
        {
            for (int i = 0; i < _values.Length; i++)
            {
                _values[i] = _value;
            }
        }

        private static string FindCoreDll()
        {
            string[] guids = AssetDatabase.FindAssets("PKHeX.Core");
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (path.EndsWith("/" + CORE_DLL_NAME))
                {
                    return path;
                }
            }
            return null;
        }

        private static void SetDataOnly(string _dllPath, BuildTarget _target, BuildTargetGroup _group, bool _enabled)
        {
            PluginImporter importer = AssetImporter.GetAtPath(_dllPath) as PluginImporter;
            if (importer == null)
            {
                Debug.LogError("[PKHex] " + _dllPath + " is not a plugin.");
                return;
            }
            if (importer.GetCompatibleWithAnyPlatform())
            {
                importer.SetExcludeFromAnyPlatform(_target, _enabled);
            }
            else
            {
                importer.SetCompatibleWithPlatform(_target, !_enabled);
            }
            importer.SetCompatibleWithEditor(true);
            importer.SaveAndReimport();

            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(_group);
            List<string> list = new List<string>(defines.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));
            list.Remove(DATA_ONLY_DEFINE);
            if (_enabled)
            {
                list.Add(DATA_ONLY_DEFINE);
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(_group, string.Join(";", list.ToArray()));
            Debug.Log("[PKHex] Data-only " + (_enabled ? "enabled" : "disabled") + " for " + _target + " (" + _group + ").");
        }

        private static bool HasDefine(BuildTargetGroup _group, string _define)
        {
            string[] defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(_group).Split(';');
            return System.Array.IndexOf(defines, _define) >= 0;
        }
    }
}
