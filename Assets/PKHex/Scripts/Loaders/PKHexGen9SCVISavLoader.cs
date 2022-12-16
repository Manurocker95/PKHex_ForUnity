using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHeX.Core;
using System.IO;
using System.Linq;


namespace PKHexForUnity
{
    public class PKHexGen9SCVISavLoader : PKHexSavLoader<SAV9SV>
    {
        public override int Badges
        {
            get
            {
                return m_saveFile != null ? 0 : 0; // TODO look for offset here
            }
        }

        public static SAV9SV LoadSaveFileFromPath(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV9SV savefile = new PKHeX.Core.SAV9SV(bytess);

            return savefile;
        }

        public override SAV9SV LoadSaveFile(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV9SV savefile = new PKHeX.Core.SAV9SV(bytess);

            return savefile;
        }
    }
}
