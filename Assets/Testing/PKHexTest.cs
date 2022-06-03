using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using PKHeX.Core;
using PokemonLetsGoUnity;
using SFB;

namespace PKHexForUnity
{
    public class PKHexTest : MonoBehaviour
    {
        [SerializeField] public int m_slotToCheckMoves = 0;

        // Start is called before the first frame update
        void Start()
        {
            LoadSaveFile();
        }

        public void LoadSaveFile()
        {
            var extensions = new[]
{
                new ExtensionFilter("SAV", "sav" ),
                new ExtensionFilter("All Files", "*" ),
            };

            var paths = StandaloneFileBrowser.OpenFilePanel("Select the SAV file", "", extensions, false);

            if (paths.Count > 0)
            {
                var sav = paths[0].FullName;

                if (!string.IsNullOrEmpty(sav))
                {
                    var savLoader = PKHexUtils.InitializeGenericSavLoader(sav);

                    if (savLoader != null)
                    {

                        Debug.Log("Player Badges: " + savLoader.Badges);
                        Debug.Log("Gender: " + savLoader.Gender);
                        Debug.Log($"Played Time {savLoader.PlayedHours}:{savLoader.PlayedMinutes}:{savLoader.PlayedSeconds}");
                        Debug.Log($"TID {savLoader.TID} and displayed TID {savLoader.DisplayTID}");

                        Debug.Log($"Max Coins {savLoader.MaxCoins}");

                        PKM pokemon = savLoader.GetPokemonInPartyInCurrentFile(m_slotToCheckMoves);
                        Debug.Log($"Pokemon in slot 0:{pokemon.Species}: {pokemon.Nickname} with level {pokemon.CurrentLevel}");

                        var types = PKHexUtils.GetPokemonTypes(pokemon);

                        Debug.Log($"Pokemon Type 1:{types.ElementAt(0)}");

                        if (types.Count() > 1 && types.ElementAt(0) != types.ElementAt(1))
                            Debug.Log($"Pokemon Type 2:{types.ElementAt(1)}");
                    }
                }
            }


            var pikachu = PKHexUtils.GetPokemonFromSpecies("Pikachu", PokemonGeneration.Galar);
            Debug.Log(pikachu.Species);
        }
    }
}