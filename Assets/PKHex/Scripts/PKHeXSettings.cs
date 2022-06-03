using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using PKHeX.Core;

namespace PKHeX.UnityVersion
{
    [Serializable]
    public sealed class PKHeXSettings
    {
        public StartupSettings Startup { get; set; } = new();
        public BackupSettings Backup { get; set; } = new();

        // General
        public LegalitySettings Legality { get; set; } = new();
        public SetImportSettings Import { get; set; } = new();
        public SlotWriteSettings SlotWrite { get; set; } = new();
        public PrivacySettings Privacy { get; set; } = new();

        // UI Tweaks
        public DisplaySettings Display { get; set; } = new();
        public SpriteSettings Sprite { get; set; } = new();
        public SoundSettings Sounds { get; set; } = new();
        public HoverSettings Hover { get; set; } = new();

        public AdvancedSettings Advanced { get; set; } = new();
        public EntityEditorSettings EntityEditor { get; set; } = new();
        public EntityDatabaseSettings EntityDb { get; set; } = new();
        public EncounterDatabaseSettings EncounterDb { get; set; } = new();
        public MysteryGiftDatabaseSettings MysteryDb { get; set; } = new();

        public static PKHeXSettings GetSettings(string configPath)
        {
            if (!File.Exists(configPath))
                return new PKHeXSettings();

            try
            {
                var lines = File.ReadAllText(configPath);
                return JsonConvert.DeserializeObject<PKHeXSettings>(lines) ?? new PKHeXSettings();
            }
            catch (Exception x)
            {
                DumpConfigError(x);
                return new PKHeXSettings();
            }
        }

        public static void SaveSettings(string configPath, PKHeXSettings cfg)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    DefaultValueHandling = DefaultValueHandling.Populate,
                    NullValueHandling = NullValueHandling.Ignore,
                };
                var text = JsonConvert.SerializeObject(cfg, settings);
                File.WriteAllText(configPath, text);
            }
            catch (Exception x)
            {
                DumpConfigError(x);
            }
        }

        private static void DumpConfigError(Exception x)
        {
            try
            {
                File.WriteAllLines("config error.txt", new[] { x.ToString() });
            }
            catch (Exception)
            {
                Debug.WriteLine(x); // ???
            }
        }
    }

    [Serializable]
    public sealed class BackupSettings
    {
        [LocalizedDescription("Automatic Backups of Save Files are copied to the backup folder when true.")]
        public bool BAKEnabled { get; set; } = true;

        [LocalizedDescription("Tracks if the \"Create Backup\" prompt has been issued to the user.")]
        public bool BAKPrompt { get; set; }

        [LocalizedDescription("List of extra locations to look for Save Files.")]
        public string[] OtherBackupPaths { get; set; } = Array.Empty<string>();

        [LocalizedDescription("Save File file-extensions (no period) that the program should also recognize.")]
        public string[] OtherSaveFileExtensions { get; set; } = Array.Empty<string>();
    }

    [Serializable]
    public sealed class StartupSettings : IStartupSettings
    {
        [Browsable(false)]
        [LocalizedDescription("Last version that the program was run with.")]
        public string Version { get; set; } = string.Empty;

        [LocalizedDescription("Force HaX mode on Program Launch")]
        public bool ForceHaXOnLaunch { get; set; }

        [LocalizedDescription("Automatically locates the most recently saved Save File when opening a new file.")]
        public bool TryDetectRecentSave { get; set; } = true;

        [LocalizedDescription("Automatically Detect Save File on Program Startup")]
        public AutoLoadSetting AutoLoadSaveOnStartup { get; set; } = AutoLoadSetting.RecentBackup;

        [LocalizedDescription("Show the changelog when a new version of the program is run for the first time.")]
        public bool ShowChangelogOnUpdate { get; set; } = true;

        [LocalizedDescription("Loads plugins from the plugins folder, assuming the folder exists. Try LoadFile to mitigate intermittent load failures.")]
        public PluginLoadSetting PluginLoadMethod { get; set; } = PluginLoadSetting.LoadFrom;

        [Browsable(false)]
        public List<string> RecentlyLoaded { get; set; } = new(MaxRecentCount);

        // Don't let invalid values slip into the startup version.
        private GameVersion _defaultSaveVersion = GameVersion.PLA;
        private string _language = GameLanguage.DefaultLanguage;

        [Browsable(false)]
        public string Language
        {
            get => _language;
            set
            {
                if (GameLanguage.GetLanguageIndex(value) == -1)
                    return;
                _language = value;
            }
        }

        [Browsable(false)]
        public GameVersion DefaultSaveVersion
        {
            get => _defaultSaveVersion;
            set
            {
                if (!value.IsValidSavedVersion())
                    return;
                _defaultSaveVersion = value;
            }
        }

        private const int MaxRecentCount = 10;

        public void LoadSaveFile(string path)
        {
            var recent = RecentlyLoaded;
            // Remove from list if already present.
            if (!recent.Remove(path) && recent.Count >= MaxRecentCount)
                recent.RemoveAt(recent.Count - 1);
            recent.Insert(0, path);
        }
    }

    public enum PluginLoadSetting
    {
        DontLoad,
        LoadFrom,
        LoadFile,
        UnsafeLoadFrom,
        LoadFromMerged,
        LoadFileMerged,
        UnsafeMerged,
    }

    [Serializable]
    public sealed class LegalitySettings : IParseSettings
    {
        [LocalizedDescription("Checks player given Nicknames and Trainer Names for profanity. Bad words will be flagged using the 3DS console's regex lists.")]
        public bool CheckWordFilter { get; set; } = true;

        [LocalizedDescription("GB: Allow Generation 2 tradeback learnsets for PK1 formats. Disable when checking RBY Metagame rules.")]
        public bool AllowGen1Tradeback { get; set; } = true;

        [LocalizedDescription("Severity to flag a Legality Check if it is a nicknamed In-Game Trade the player cannot normally nickname.")]
        public Severity NicknamedTrade { get; set; } = Severity.Invalid;

        [LocalizedDescription("Severity to flag a Legality Check if it is a nicknamed Mystery Gift the player cannot normally nickname.")]
        public Severity NicknamedMysteryGift { get; set; } = Severity.Fishy;

        [LocalizedDescription("Severity to flag a Legality Check if the RNG Frame Checking logic does not find a match.")]
        public Severity RNGFrameNotFound { get; set; } = Severity.Fishy;

        [LocalizedDescription("Severity to flag a Legality Check if Pok�mon from Gen1/2 has a Star Shiny PID.")]
        public Severity Gen7TransferStarPID { get; set; } = Severity.Fishy;

        [LocalizedDescription("Severity to flag a Legality Check if a Gen8 Memory is missing for the Handling Trainer.")]
        public Severity Gen8MemoryMissingHT { get; set; } = Severity.Fishy;

        [LocalizedDescription("Severity to flag a Legality Check if the HOME Tracker is Missing")]
        public Severity Gen8TransferTrackerNotPresent { get; set; } = Severity.Fishy;

        [LocalizedDescription("Severity to flag a Legality Check if Pok�mon has a Nickname matching another Species.")]
        public Severity NicknamedAnotherSpecies { get; set; } = Severity.Fishy;

        [LocalizedDescription("Severity to flag a Legality Check if Pok�mon has a zero value for both Height and Weight.")]
        public Severity ZeroHeightWeight { get; set; } = Severity.Fishy;
    }

    [Serializable]
    public sealed class AdvancedSettings
    {
        [LocalizedDescription("Allow PKM file conversion paths that are not possible via official methods. Individual properties will be copied sequentially.")]
        public bool AllowIncompatibleConversion { get; set; }

        [LocalizedDescription("Folder path that contains dump(s) of block hash-names. If a specific dump file does not exist, only names defined within the program's code will be loaded.")]
        public string PathBlockKeyList { get; set; } = string.Empty;

        [LocalizedDescription("Hide event variables below this event type value. Removes event values from the GUI that the user doesn't care to view.")]
        public NamedEventType HideEventTypeBelow { get; set; }

        [LocalizedDescription("Hide event variable names for that contain any of the comma-separated substrings below. Removes event values from the GUI that the user doesn't care to view.")]
        public string HideEvent8Contains { get; set; } = string.Empty;

        [Browsable(false)]
        public string[] GetExclusionList8() => Array.ConvertAll(HideEvent8Contains.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries), z => z.Trim());
    }

    [Serializable]
    public class EntityDatabaseSettings
    {
        [LocalizedDescription("When loading content for the PKM Database, search within backup save files.")]
        public bool SearchBackups { get; set; } = true;

        [LocalizedDescription("When loading content for the PKM Database, search within OtherBackupPaths.")]
        public bool SearchExtraSaves { get; set; } = true;

        [LocalizedDescription("When loading content for the PKM Database, search subfolders within OtherBackupPaths.")]
        public bool SearchExtraSavesDeep { get; set; } = true;

        [LocalizedDescription("When loading content for the PKM database, the list will be ordered by this option.")]
        public DatabaseSortMode InitialSortMode { get; set; }

        [LocalizedDescription("Hides unavailable Species if the currently loaded save file cannot import them.")]
        public bool FilterUnavailableSpecies { get; set; } = true;
    }

    public enum DatabaseSortMode
    {
        None,
        SpeciesForm,
        SlotIdentity,
    }

    [Serializable]
    public sealed class EntityEditorSettings
    {
        [LocalizedDescription("When changing the Hidden Power type, automatically maximize the IVs to ensure the highest Base Power result. Otherwise, keep the IVs as close as possible to the original.")]
        public bool HiddenPowerOnChangeMaxPower { get; set; } = true;
    }

    [Serializable]
    public sealed class EncounterDatabaseSettings
    {
        [LocalizedDescription("Skips searching if the user forgot to enter Species / Move(s) into the search criteria.")]
        public bool ReturnNoneIfEmptySearch { get; set; } = true;

        [LocalizedDescription("Hides unavailable Species if the currently loaded save file cannot import them.")]
        public bool FilterUnavailableSpecies { get; set; } = true;

        [LocalizedDescription("Use properties from the PKM Editor tabs to specify criteria like Gender and Nature when generating an encounter.")]
        public bool UseTabsAsCriteria { get; set; } = true;

        [LocalizedDescription("Use properties from the PKM Editor tabs even if the new encounter isn't the same evolution chain.")]
        public bool UseTabsAsCriteriaAnySpecies { get; set; } = true;
    }

    [Serializable]
    public sealed class MysteryGiftDatabaseSettings
    {
        [LocalizedDescription("Hides gifts if the currently loaded save file cannot (indirectly) receive them.")]
        public bool FilterUnavailableSpecies { get; set; } = true;
    }

    [Serializable]
    public sealed class HoverSettings
    {
        [LocalizedDescription("Show PKM Slot ToolTip on Hover")]
        public bool HoverSlotShowText { get; set; } = true;

        [LocalizedDescription("Play PKM Slot Cry on Hover")]
        public bool HoverSlotPlayCry { get; set; } = true;

        [LocalizedDescription("Show a Glow effect around the PKM on Hover")]
        public bool HoverSlotGlowEdges { get; set; } = true;
    }

    [Serializable]
    public sealed class SoundSettings
    {
        [LocalizedDescription("Play Sound when loading a new Save File")]
        public bool PlaySoundSAVLoad { get; set; } = true;
        [LocalizedDescription("Play Sound when popping up Legality Report")]
        public bool PlaySoundLegalityCheck { get; set; } = true;
    }

    [Serializable]
    public sealed class SetImportSettings
    {
        [LocalizedDescription("Apply StatNature to Nature on Import")]
        public bool ApplyNature { get; set; } = true;
        [LocalizedDescription("Apply Markings on Import")]
        public bool ApplyMarkings { get; set; } = true;
    }

    [Serializable]
    public sealed class SlotWriteSettings
    {
        [LocalizedDescription("Automatically modify the Save File's Pok�dex when injecting a PKM.")]
        public bool SetUpdateDex { get; set; } = true;

        [LocalizedDescription("Automatically adapt the PKM Info to the Save File (Handler, Format)")]
        public bool SetUpdatePKM { get; set; } = true;

        [LocalizedDescription("When enabled and closing/loading a save file, the program will alert if the current save file has been modified without saving.")]
        public bool ModifyUnset { get; set; } = true;
    }

    [Serializable]
    public sealed class DisplaySettings
    {
        [LocalizedDescription("Show Unicode gender symbol characters, or ASCII when disabled.")]
        public bool Unicode { get; set; } = true;

        [LocalizedDescription("Don't show the Legality popup if Legal!")]
        public bool IgnoreLegalPopup { get; set; }

        [LocalizedDescription("Flag Illegal Slots in Save File")]
        public bool FlagIllegal { get; set; } = true;
    }

    [Serializable]
    public sealed class SpriteSettings
    {
        [LocalizedDescription("Choice for which sprite building mode to use.")]
        public int SpritePreference { get; set; } = 0;

        [LocalizedDescription("Show fan-made shiny sprites when the PKM is shiny.")]
        public bool ShinySprites { get; set; } = true;

        [LocalizedDescription("Show an Egg Sprite As Held Item rather than hiding the PKM")]
        public bool ShowEggSpriteAsHeldItem { get; set; } = true;

        [LocalizedDescription("Show the required ball for an Encounter Template")]
        public bool ShowEncounterBall { get; set; } = true;

        [LocalizedDescription("Opacity for the Encounter Type background layer.")]
        public byte ShowEncounterOpacityBackground { get; set; } = 0x3F; // kinda low

        [LocalizedDescription("Opacity for the Encounter Type stripe layer.")]
        public byte ShowEncounterOpacityStripe { get; set; } = 0x5F; // 0xFF opaque

        [LocalizedDescription("Show a thin stripe to indicate the percent of level-up progress")]
        public bool ShowExperiencePercent { get; set; }

        [LocalizedDescription("Amount of pixels thick to show when displaying the encounter type color stripe.")]
        public int ShowEncounterThicknessStripe { get; set; } = 4; // pixels
    }

    [Serializable]
    public sealed class PrivacySettings
    {
        [LocalizedDescription("Hide Save File Details in Program Title")]
        public bool HideSAVDetails { get; set; }

        [LocalizedDescription("Hide Secret Details in Editors")]
        public bool HideSecretDetails { get; set; }
    }
}
