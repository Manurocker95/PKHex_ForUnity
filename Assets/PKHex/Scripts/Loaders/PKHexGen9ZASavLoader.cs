using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Legends: Z-A.
    /// </summary>
    public class PKHexGen9ZASavLoader : PKHexSavLoader<SAV9ZA>
    {
        public static SAV9ZA LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV9ZA>(_sav);
        }
    }
}
