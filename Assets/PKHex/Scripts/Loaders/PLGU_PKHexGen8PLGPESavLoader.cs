using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHeX.Core;
using System.IO;
using System.Linq;

namespace PokemonLetsGoUnity
{
    public class PLGU_PKHexGen8PLGPESavLoader : PLGU_PKHexSavLoader<SAV7b>
    {
        public override int Badges
        {
            get
            {
                return m_saveFile != null ? 0 : 0; // TODO look for offset here
            }
        }

        public static SAV7b LoadSaveFileFromPath(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV7b savefile = new PKHeX.Core.SAV7b(bytess);

            return savefile;
        }

        public override SAV7b LoadSaveFile(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV7b savefile = new PKHeX.Core.SAV7b(bytess);

            return savefile;
        }
    }
}
