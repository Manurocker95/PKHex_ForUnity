using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHeX.Core;
using System.IO;
using System.Linq;

namespace PKHexForUnity
{
    public class PKHexGen3ColosseumSavLoader : PKHexSavLoader<SAV3Colosseum>
    {
        public static SAV3Colosseum LoadSaveFileFromPath(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV3Colosseum savefile = new PKHeX.Core.SAV3Colosseum(bytess);

            return savefile;
        }

        public override SAV3Colosseum LoadSaveFile(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV3Colosseum savefile = new PKHeX.Core.SAV3Colosseum(bytess);

            return savefile;
        }        
    }
}
