using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Scarlet and Violet.
    /// </summary>
    public class PKHexGen9SCVISavLoader : PKHexSavLoader<SAV9SV>
    {
        public static SAV9SV LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV9SV>(_sav);
        }
    }
}
