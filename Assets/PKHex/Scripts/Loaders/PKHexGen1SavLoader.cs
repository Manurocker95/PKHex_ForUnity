using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Red, Green, Blue and Yellow.
    /// </summary>
    public class PKHexGen1SavLoader : PKHexSavLoader<SAV1>
    {
        public static SAV1 LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV1>(_sav);
        }

        /// <summary> True for Pokémon Yellow saves. </summary>
        public bool IsYellow
        {
            get
            {
                return m_saveFile != null && IsYellowSave(m_saveFile);
            }
        }

        /// <summary>
        /// Gen 1 requires Pokémon Yellow.
        /// </summary>
        public override bool IsSurfingPikachuInPartySaveData(string _sav)
        {
            SAV1 savefile = LoadSaveFile(_sav);
            return savefile != null && IsSurfingPikachuInPartySaveData(savefile);
        }

        public override bool IsSurfingPikachuInPartySaveData(SAV1 _savefile)
        {
            return _savefile != null && IsYellowSave(_savefile) && IsThereAPokemonInPartyWithMove(_savefile, "Surf", "Pikachu");
        }

        private static bool IsYellowSave(SAV1 _savefile)
        {
            return _savefile.Version == GameVersion.YW;
        }
    }
}
