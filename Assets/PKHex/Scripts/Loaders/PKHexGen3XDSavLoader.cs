using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHeX.Core;
using System.IO;
using System.Linq;

namespace PKHexForUnity
{
    public class PKHexGen3XDSavLoader : PKHexSavLoader<SAV3XD>
    {
        public static SAV3XD LoadSaveFileFromPath(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV3XD savefile = new PKHeX.Core.SAV3XD(bytess);

            return savefile;
        }

        public override SAV3XD LoadSaveFile(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV3XD savefile = new PKHeX.Core.SAV3XD(bytess);

            return savefile;
        }        
    }
}
