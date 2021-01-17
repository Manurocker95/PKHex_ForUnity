using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHeX.Core;
using System.IO;
using System.Linq;

namespace PokemonLetsGoUnity
{
    public class PLGU_PKHexGen6XYSavLoader : PLGU_PKHexSavLoader<SAV6XY>
    {
        public override int Badges
        {
            get
            {
                return m_saveFile != null ? m_saveFile.Misc.Badges : 0;
            }
        }

        public override uint Money
        {
            get
            {
                return m_saveFile != null ? m_saveFile.Misc.Money : 0;
            }
        }

        public static SAV6XY LoadSaveFileFromPath(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV6XY savefile = new SAV6XY(bytess);

            return savefile;
        }

        public override SAV6XY LoadSaveFile(string sav)
        {
            return LoadSaveFileFromPath(sav);
        }        
    }
}
