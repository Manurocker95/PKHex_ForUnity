// Needs PKHeX.Core.dll: excluded from data-only player builds (see PKHexExportWindow).
#if !PKHEX_DATA_ONLY || UNITY_EDITOR
using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Gold, Silver and Crystal.
    /// </summary>
    public class PKHexGen2SavLoader : PKHexSavLoader<SAV2>
    {
        public static SAV2 LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV2>(_sav);
        }
    }
}
#endif
