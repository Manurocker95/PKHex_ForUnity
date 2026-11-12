using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Platinum.
    /// </summary>
    public class PKHexGenPTSavLoader : PKHexSavLoader<SAV4Pt>
    {
        public static SAV4Pt LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV4Pt>(_sav);
        }
    }
}
