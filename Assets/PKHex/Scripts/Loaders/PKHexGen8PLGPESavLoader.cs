// Needs PKHeX.Core.dll: excluded from data-only player builds (see PKHexExportWindow).
#if !PKHEX_DATA_ONLY || UNITY_EDITOR
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
#endif
