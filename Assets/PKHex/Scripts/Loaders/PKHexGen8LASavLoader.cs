using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Legends: Arceus.
    /// </summary>
    public class PKHexGen8LASavLoader : PKHexSavLoader<SAV8LA>
    {
        public static SAV8LA LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV8LA>(_sav);
        }
    }
}
