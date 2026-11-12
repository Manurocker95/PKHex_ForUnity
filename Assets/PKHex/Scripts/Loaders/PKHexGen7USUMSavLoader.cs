using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Ultra Sun and Ultra Moon.
    /// </summary>
    public class PKHexGen7USUMSavLoader : PKHexSavLoader<SAV7USUM>
    {
        public static SAV7USUM LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV7USUM>(_sav);
        }
    }
}
