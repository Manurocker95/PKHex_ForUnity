using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHeX.Core;
using System.IO;
using System.Linq;

namespace PokemonLetsGoUnity
{
    public class PLGU_PKHexGen7SUMOSavLoader : PLGU_PKHexSavLoader<SAV7SM>
    {
        public override int Badges
        {
            get
            {
                return m_saveFile != null ? (int)m_saveFile.Misc.Stamps : 0;
            }
        }

        public override uint Money
        {
            get
            {
                return m_saveFile != null ? m_saveFile.Misc.Money : 0;
            }
        }

        public static SAV7SM LoadSaveFileFromPath(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV7SM savefile = new SAV7SM(bytess);

            return savefile;
        }

        public override SAV7SM LoadSaveFile(string sav)
        {
            return LoadSaveFileFromPath(sav);
        }        
    }
}
