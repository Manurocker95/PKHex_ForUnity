// Needs PKHeX.Core.dll: excluded from data-only player builds (see PKHexExportWindow).
#if !PKHEX_DATA_ONLY || UNITY_EDITOR
using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Brilliant Diamond and Shining Pearl.
    /// </summary>
    public class PKHexGen8BSSavLoader : PKHexSavLoader<SAV8BS>
    {
        public static SAV8BS LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV8BS>(_sav);
        }
    }
}
#endif
