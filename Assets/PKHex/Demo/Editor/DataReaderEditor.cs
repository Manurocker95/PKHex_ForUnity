#if PKHEX_FOR_UNITY
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PKHeX.Core;

namespace PKHexForUnity
{
    [CustomEditor(typeof(DataReader))]
    public class DataReaderEditor : Editor
    {
        protected int m_maxPokemon = 905;
        protected PokemonGeneration m_lastGeneration = PokemonGeneration.None;

        protected DataReader myTarget;

        protected virtual void OnEnable()
        {
            myTarget = (DataReader)target;
            SetMaxPokemon(m_lastGeneration);
        }

        protected virtual void SetMaxPokemon(PokemonGeneration _newGen)
        {
            m_lastGeneration = _newGen;
            m_maxPokemon = PKHexUtils.GetDexCount(m_lastGeneration);
            if (myTarget != null)
            {
                if (myTarget.PokemonIndex > m_maxPokemon)
                {
                    myTarget.PokemonIndex = m_maxPokemon;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            if (myTarget == null)
                myTarget = (DataReader)target;

            if (myTarget == null)
                return;

            EditorGUILayout.LabelField("PKHex For Unity ");
            myTarget.PokemonIndex = EditorGUILayout.IntSlider("Pokémon Index", myTarget.PokemonIndex, 1, m_maxPokemon);
            myTarget.Language = (LanguageID)EditorGUILayout.EnumPopup("Language", myTarget.Language);
            myTarget.Generation = (PokemonGeneration)EditorGUILayout.EnumPopup("Generation", myTarget.Generation);

            if (m_lastGeneration != myTarget.Generation)
            {
                SetMaxPokemon(myTarget.Generation);
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Generate Pokémon Data"))
            {
                myTarget.GenerateMonster();
            }


            if (GUILayout.Button("Show Full Dex List"))
            {
                myTarget.ShowFullDexList();
            }

            GUILayout.Space(20);

            EditorGUILayout.LabelField("PokeAPI");
            myTarget.PokeAPILoader = (PokeAPI.PokeAPILoader)EditorGUILayout.ObjectField(myTarget.PokeAPILoader, typeof(PokeAPI.PokeAPILoader), true);

            GUILayout.Space(20);
            
            if (myTarget.PokeAPILoader)
            {
                if (!myTarget.PokeAPILoader.IsParsingDexFromAPI)
                {
                    if (GUILayout.Button("Show Pokemon From API"))
                        myTarget.PokeAPILoader.GetPokemonFromAPI(myTarget.PokemonIndex, false);

                    if (GUILayout.Button("Show Pokemon Dex Entry From API"))
                        myTarget.PokeAPILoader.GetDexDescription(myTarget.PokemonIndex, myTarget.PokeAPILoader.GetGameID(myTarget.PokeAPILoader.Version), myTarget.Language);

                    if (GUILayout.Button("Save Pokemon From API"))
                        myTarget.PokeAPILoader.GetPokemonFromAPI(myTarget.PokemonIndex, true);

                    if (GUILayout.Button("Save Full Dex List From API"))
                        myTarget.PokeAPILoader.FillDexFromAPI();

                    if (GUILayout.Button("Load Full Dex List From API JSON"))
                        myTarget.PokeAPILoader.LoadDexFromAPIJSON();

                    if (GUILayout.Button("Load Pokemon From API JSON"))
                        myTarget.PokeAPILoader.LoadPokemonFromAPIJSON(myTarget.PokemonIndex-1);
                }
                else
                {
                    if (GUILayout.Button("Cancel parse Dex List From API"))
                        myTarget.PokeAPILoader.CancelFillDexFromAPI();
                }

            }
            else
            {
                EditorGUILayout.LabelField("Add reference for PokeAPI Loader");
            }
        }
    }
}
#endif