namespace PKHexForUnity
{
    /// <summary>
    /// Folder layout of the exported PKHeX data. Shared by the exporter (Editor) and the runtime loader.
    /// </summary>
    /// <remarks>
    /// Layout under a <c>Resources</c> folder:
    /// <code>
    /// Data/PKHexDataIndex.asset                     what was exported
    /// Data/Gen9/Games/SV.asset                      language-independent game data (PKHexGameData)
    /// Data/Gen9/English/PKHexStrings.asset          strings of the generation (PKHexStringTable)
    /// Data/Gen9/English/Species.txt ...             the same data as plain text
    /// </code>
    /// This class has no PKHeX.Core dependency so it can be used by data-only builds (e.g. New Nintendo 3DS).
    /// </remarks>
    public static class PKHexDataPaths
    {
        public const string DEFAULT_EXPORT_ROOT = "Assets/ExportedPKHex/Resources";
        public const string DATA_FOLDER = "Data";
        public const string GAMES_FOLDER = "Games";
        public const string GENERATION_PREFIX = "Gen";
        public const string STRINGS_ASSET_NAME = "PKHexStrings";
        public const string INDEX_ASSET_NAME = "PKHexDataIndex";
        public const string TEXT_EXTENSION = ".txt";
        public const string CSV_FOLDER = "CSV";
        public const string CSV_EXTENSION = ".csv";
        public const string ASSET_EXTENSION = ".asset";

        public const int MIN_GENERATION = 1;
        public const int MAX_GENERATION = 9;

        /// <summary> Language codes used by PKHeX, parallel to <see cref="LANGUAGE_FOLDERS"/>. </summary>
        public static readonly string[] LANGUAGE_CODES = { "ja", "en", "fr", "it", "de", "es", "es-419", "ko", "zh-Hans", "zh-Hant" };

        /// <summary> Folder name of each language, parallel to <see cref="LANGUAGE_CODES"/>. </summary>
        public static readonly string[] LANGUAGE_FOLDERS =
        {
            "Japanese", "English", "French", "Italian", "German", "Spanish", "SpanishLatinAmerica", "Korean",
            "ChineseSimplified", "ChineseTraditional"
        };

        /// <summary> "Data/Gen9" </summary>
        public static string GetGenerationPath(int _generation)
        {
            return DATA_FOLDER + "/" + GENERATION_PREFIX + _generation;
        }

        /// <summary> "Data/Gen9/English" </summary>
        public static string GetLanguagePath(int _generation, string _languageFolder)
        {
            return GetGenerationPath(_generation) + "/" + _languageFolder;
        }

        /// <summary> Resources path (no extension) of a game data asset: "Data/Gen9/Games/SV". </summary>
        public static string GetGameResourcePath(int _generation, string _gameKey)
        {
            return GetGenerationPath(_generation) + "/" + GAMES_FOLDER + "/" + _gameKey;
        }

        /// <summary> Resources path (no extension) of a string table: "Data/Gen9/English/PKHexStrings". </summary>
        public static string GetStringsResourcePath(int _generation, string _languageFolder)
        {
            return GetLanguagePath(_generation, _languageFolder) + "/" + STRINGS_ASSET_NAME;
        }

        /// <summary> Resources path (no extension) of an exported text file: "Data/Gen9/English/Species". </summary>
        public static string GetTextResourcePath(int _generation, string _languageFolder, string _fileName)
        {
            return GetLanguagePath(_generation, _languageFolder) + "/" + _fileName;
        }

        /// <summary> "Data/Gen9/CSV" (all languages of the generation as columns, English first). </summary>
        public static string GetCsvPath(int _generation)
        {
            return GetGenerationPath(_generation) + "/" + CSV_FOLDER;
        }

        /// <summary> Resources path (no extension) of an exported CSV file: "Data/Gen9/CSV/Types". </summary>
        public static string GetCsvResourcePath(int _generation, string _fileName)
        {
            return GetCsvPath(_generation) + "/" + _fileName;
        }

        /// <summary> Resources path (no extension) of the export index. </summary>
        public static string GetIndexResourcePath()
        {
            return DATA_FOLDER + "/" + INDEX_ASSET_NAME;
        }

        /// <summary> "en" → "English"; null when the code is unknown. </summary>
        public static string GetLanguageFolder(string _languageCode)
        {
            int index = System.Array.IndexOf(LANGUAGE_CODES, _languageCode);
            return index >= 0 ? LANGUAGE_FOLDERS[index] : null;
        }

        /// <summary> "English" → "en"; null when the folder is unknown. </summary>
        public static string GetLanguageCode(string _languageFolder)
        {
            int index = System.Array.IndexOf(LANGUAGE_FOLDERS, _languageFolder);
            return index >= 0 ? LANGUAGE_CODES[index] : null;
        }
    }
}
