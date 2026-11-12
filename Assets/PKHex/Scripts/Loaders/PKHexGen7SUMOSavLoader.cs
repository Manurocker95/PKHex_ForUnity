using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Sun and Moon.
    /// </summary>
    public class PKHexGen7SUMOSavLoader : PKHexSavLoader<SAV7SM>
    {
        public static SAV7SM LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV7SM>(_sav);
        }
    }
}
