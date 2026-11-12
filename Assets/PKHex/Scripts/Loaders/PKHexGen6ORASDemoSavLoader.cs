// Needs PKHeX.Core.dll: excluded from data-only player builds (see PKHexExportWindow).
#if !PKHEX_DATA_ONLY || UNITY_EDITOR
using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Omega Ruby and Alpha Sapphire special demo.
    /// </summary>
    public class PKHexGen6ORASDemoSavLoader : PKHexSavLoader<SAV6AODemo>
    {
        public static SAV6AODemo LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV6AODemo>(_sav);
        }
    }
}
#endif
