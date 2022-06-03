using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHeX.Core;
using System.IO;
using System.Linq;

namespace PKHexForUnity
{
    public class PKHexGen7USUMSavLoader : PKHexSavLoader<SAV7USUM>
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

        public static SAV7USUM LoadSaveFileFromPath(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV7USUM savefile = new SAV7USUM(bytess);

            return savefile;
        }

        public override SAV7USUM LoadSaveFile(string sav)
        {
            return LoadSaveFileFromPath(sav);
        }        
    }
}
