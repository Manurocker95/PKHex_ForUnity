using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHeX.Core;
using System.IO;
using System.Linq;

namespace PokemonLetsGoUnity
{
    public class PLGU_PKHexGen3SavLoader : PLGU_PKHexSavLoader<SAV3>
    {
        public override int Badges
        {
            get
            {
                return m_saveFile != null ? m_saveFile.Badges : 0;
            }
        }

        public override uint Coins
        {
            get
            {
                return m_saveFile != null ? m_saveFile.Coin : 0;
            }
        }

        public static SAV3 LoadSaveFileFromPath(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV3 savefile = new PKHeX.Core.SAV3(bytess, GameVersion.RSE);

            return savefile;
        }

        public override SAV3 LoadSaveFile(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV3 savefile = new PKHeX.Core.SAV3(bytess, GameVersion.RSE);

            return savefile;
        }        
    }
}
