using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for HeartGold and SoulSilver.
    /// </summary>
    public class PKHexGenHGSSSavLoader : PKHexSavLoader<SAV4HGSS>
    {
        public static SAV4HGSS LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV4HGSS>(_sav);
        }
    }
}
