using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHeX.Core;
using System.IO;
using System.Linq;

namespace PKHexForUnity
{
    public class PKHexGen3BoxSavLoader : PKHexSavLoader<SAV3RSBox>
    {
        public static SAV3RSBox LoadSaveFileFromPath(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV3RSBox savefile = new PKHeX.Core.SAV3RSBox(bytess);

            return savefile;
        }

        public override SAV3RSBox LoadSaveFile(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV3RSBox savefile = new PKHeX.Core.SAV3RSBox(bytess);

            return savefile;
        }        
    }
}
