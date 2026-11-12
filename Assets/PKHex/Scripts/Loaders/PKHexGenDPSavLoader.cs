using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Diamond and Pearl.
    /// </summary>
    public class PKHexGenDPSavLoader : PKHexSavLoader<SAV4DP>
    {
        public static SAV4DP LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV4DP>(_sav);
        }
    }
}
