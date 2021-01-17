using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHeX.Core;
using System.IO;
using System.Linq;

namespace PokemonLetsGoUnity
{
    public class PLGU_PKHexGen5BWSavLoader : PLGU_PKHexSavLoader<SAV5BW>
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

        public static SAV5BW LoadSaveFileFromPath(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV5BW savefile = new SAV5BW(bytess);

            return savefile;
        }

        public override SAV5BW LoadSaveFile(string sav)
        {
            return LoadSaveFileFromPath(sav);
        }        
    }
}
