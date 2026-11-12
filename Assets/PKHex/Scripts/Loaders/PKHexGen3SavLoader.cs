using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Ruby, Sapphire, Emerald, FireRed and LeafGreen.
    /// </summary>
    public class PKHexGen3SavLoader : PKHexSavLoader<SAV3>
    {
        public static SAV3 LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV3>(_sav);
        }
    }
}
