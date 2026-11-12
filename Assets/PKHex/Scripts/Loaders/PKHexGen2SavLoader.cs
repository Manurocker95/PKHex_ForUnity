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
