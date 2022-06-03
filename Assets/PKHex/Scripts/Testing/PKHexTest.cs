using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using PKHeX.Core;
using PokemonLetsGoUnity;

namespace PKHexForUnity
{
    public class PKHexTest : MonoBehaviour
    {
        public string sav = "D:\\Pokémon\\PKHex_ForUnity\\Assets\\PKHex\\Resources\\Save Data Test\\main";
        public string PK = "D:\\Pokémon\\PKHex_ForUnity\\Assets\\PKHex\\Resources\\Save Data Test\\TEST.pk8";
        public int slotToCheckMoves = 0;

        // Start is called before the first frame update
        void Start()
        {
            LoadSaveFile();
        }

        public void LoadSaveFile()
        {
            var savLoader = PKHexUtils.InitializeGenericSavLoader(sav);

            if (savLoader != null)
            {
  
                Debug.Log("Player Badges: " + savLoader.Badges);
                Debug.Log("Gender: " + savLoader.Gender);
                Debug.Log($"Played Time {savLoader.PlayedHours}:{savLoader.PlayedMinutes}:{savLoader.PlayedSeconds}");
                Debug.Log($"TID {savLoader.TID} and displayed TID {savLoader.DisplayTID}");

                Debug.Log($"Max Coins {savLoader.MaxCoins}");

                PKM pokemon = savLoader.GetPokemonInPartyInCurrentFile(slotToCheckMoves);
	            Debug.Log($"Pokemon in slot 0:{pokemon.Species}: {pokemon.Nickname} with level {pokemon.CurrentLevel}");

                var kvp = PKHexUtils.LoadPKMFromPath(PK);//savLoader.LoadPKMFromPath(PK);
                if (kvp.Key)
                {
                    Debug.Log("Pokemon loaded from PK: " + PKHexUtils.GetPokemonSpeciesNameInLanguage(kvp.Value, SystemLanguage.English));
                }
            }
        }

    }

}