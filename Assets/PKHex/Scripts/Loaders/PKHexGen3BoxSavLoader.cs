using PKHeX.Core;

namespace PKHexForUnity
{
    /// <summary>
    /// Save loader for Pokémon Box Ruby &amp; Sapphire.
    /// </summary>
    public class PKHexGen3BoxSavLoader : PKHexSavLoader<SAV3RSBox>
    {
        public static SAV3RSBox LoadSaveFileFromPath(string _sav)
        {
            return PKHexSavLoaderFactory.LoadSaveFileFromPath<SAV3RSBox>(_sav);
        }
    }
}
