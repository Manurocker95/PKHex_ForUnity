// Needs PKHeX.Core.dll: excluded from data-only player builds (see PKHexExportWindow).
#if !PKHEX_DATA_ONLY || UNITY_EDITOR
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
#endif
