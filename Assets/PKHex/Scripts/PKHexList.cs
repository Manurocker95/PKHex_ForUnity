using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PKHeX;
using PKHeX.Core;
using PokemonLetsGoUnity;

namespace Pokedex
{
    public class PKHexList : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var names = PLGU_PKHexUtils.GetFullDexNames();
            foreach (string nam in names)
            { Debug.Log(nam); }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}