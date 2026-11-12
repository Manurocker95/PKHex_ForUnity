using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Sword and Shield.
    /// </summary>
    public class PKHexGen8ShSwSavLoader : PKHexSavLoader<SAV8SWSH>
    {
        public static SAV8SWSH LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV8SWSH>(_sav);
        }
    }
}
