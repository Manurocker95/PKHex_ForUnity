using System;

namespace PKHexForUnity
{
    /// <summary>
    /// Language-independent data of one species-form in one game, exported from PKHeX.
    /// </summary>
    /// <remarks>
    /// All IDs (types, abilities, moves) index the arrays of <see cref="PKHexStringTable"/>.
    /// Types use the modern type order also in Gen 1-2 (converted on export).
    /// </remarks>
    [Serializable]
    public class PKHexSpeciesEntry
    {
        public int Species;
        public int Form;
        public int FormCount;

        /// <summary> False when the species-form exists in the data tables but cannot be obtained in this game. </summary>
        public bool IsPresentInGame;

        public int Type1;
        public int Type2;

        /// <summary> HP, Attack, Defense, Sp. Attack, Sp. Defense, Speed. </summary>
        public int[] BaseStats = new int[6];
        public int BaseStatTotal;

        /// <summary> Ability IDs in personal-data order (1, 2, hidden when the game has it). Empty in Gen 1-2. </summary>
        public int[] Abilities = new int[0];

        /// <summary> Gender ratio byte (0 = always male, 254 = always female, 255 = genderless). </summary>
        public int GenderRatio;
        public int CatchRate;
        public int BaseExperience;
        public int ExperienceGrowth;
        public int EggGroup1;
        public int EggGroup2;
        public int Height;
        public int Weight;

        /// <summary> Level-up moves; <see cref="LevelUpLevels"/> has the level of each entry. </summary>
        public int[] LevelUpMoves = new int[0];
        public int[] LevelUpLevels = new int[0];

        /// <summary> Gen 1 only: moves already known when obtained (stored outside the learnset in Gen 1). </summary>
        public int[] InitialMoves = new int[0];
        public int[] EggMoves = new int[0];

        /// <summary> Moves learnable through TM/HM/TR in this game. </summary>
        public int[] MachineMoves = new int[0];

        /// <summary> Moves learnable through move tutors in this game. </summary>
        public int[] TutorMoves = new int[0];

        public PKHexEvolutionEntry[] Evolutions = new PKHexEvolutionEntry[0];
    }

    /// <summary> One evolution method of a species-form. </summary>
    [Serializable]
    public class PKHexEvolutionEntry
    {
        public int Species;
        public int Form;

        /// <summary> PKHeX EvolutionType value; <see cref="MethodName"/> is its enum name (e.g. "UseItem"). </summary>
        public int Method;
        public string MethodName;

        /// <summary>
        /// Raw argument from PKHeX's tables (item, move, species, stat... depending on <see cref="Method"/>).
        /// PKHeX does not resolve it, so it is exported unresolved.
        /// </summary>
        public int Argument;
        public int Level;
        public bool RequiresLevelUp;
    }

    /// <summary> Move data available in PKHeX for one game. PKHeX has no power/accuracy/category tables. </summary>
    [Serializable]
    public class PKHexMoveEntry
    {
        public int Move;
        public int Type;
        public int PP;
    }

    /// <summary> Localized name of one form. </summary>
    [Serializable]
    public class PKHexFormName
    {
        public int Species;
        public int Form;
        public string Name;
    }

    /// <summary> Strings that differ between the games of one generation. </summary>
    [Serializable]
    public class PKHexGameStrings
    {
        public string GameKey;
        public string GameName;
        public string[] Items = new string[0];
        public PKHexFormName[] Forms = new PKHexFormName[0];
    }

    /// <summary> What was exported for one generation (see <see cref="PKHexDataIndex"/>). </summary>
    [Serializable]
    public class PKHexExportedGeneration
    {
        public int Generation;
        public string[] GameKeys = new string[0];
        public string[] LanguageFolders = new string[0];
    }
}
