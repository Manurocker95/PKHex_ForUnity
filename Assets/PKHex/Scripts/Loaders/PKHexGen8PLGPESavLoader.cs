using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Let's Go, Pikachu! and Let's Go, Eevee!
    /// </summary>
    public class PKHexGen8PLGPESavLoader : PKHexSavLoader<SAV7b>
    {
        public static SAV7b LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV7b>(_sav);
        }
    }
}
