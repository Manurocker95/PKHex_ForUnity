using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for My Pokémon Ranch.
    /// </summary>
    public class PKHexGen4RanchSavLoader : PKHexSavLoader<SAV4Ranch>
    {
        public static SAV4Ranch LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV4Ranch>(_sav);
        }
    }
}
