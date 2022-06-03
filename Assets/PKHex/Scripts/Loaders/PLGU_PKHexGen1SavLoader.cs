using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHeX.Core;
using System.IO;
using System.Linq;

namespace PokemonLetsGoUnity
{
    public class PLGU_PKHexGen1SavLoader : PLGU_PKHexSavLoader<SAV1>
    {
        public override int Badges
        {
            get
            {
                return m_saveFile != null ? m_saveFile.Badges : 0;
            }
        }

        public override string PlayerName
        {
            get
            {
                return m_saveFile != null ? m_saveFile.OT : base.PlayerName;
            }
        }


        public override uint Coins
        {
            get
            {
                return m_saveFile != null ? m_saveFile.Coin : 0;
            }
        }

        public static SAV1 LoadSaveFileFromPath(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV1 savefile = new PKHeX.Core.SAV1(bytess, GameVersion.RBY);

            return savefile;
        }

        public override SAV1 LoadSaveFile(string sav)
        {
            byte[] bytess = File.ReadAllBytes(sav);
            SAV1 savefile = new PKHeX.Core.SAV1(bytess, GameVersion.RBY);

            return savefile;
        }

        /// <summary>
        /// Gen 1 requires pokémon Yellow
        /// </summary>
        /// <param name="sav"></param>
        /// <returns></returns>
        public override bool IsSurfingPikachuInPartySaveData(string sav)
        {
            SAV1 savefile = LoadSaveFile(sav);
            return (savefile.Yellow && IsThereAPokemonInPartyWithMove(savefile, "Surf"));
        }

        public override bool IsSurfingPikachuInPartySaveData(SAV1 savefile)
        {
            return (savefile.Yellow && IsThereAPokemonInPartyWithMove(savefile, "Surf"));
        }
    }
}
