using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Pokémon Battle Revolution.
    /// </summary>
    public class PKHexGen4BRSavLoader : PKHexSavLoader<SAV4BR>
    {
        public static SAV4BR LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV4BR>(_sav);
        }
    }
}
