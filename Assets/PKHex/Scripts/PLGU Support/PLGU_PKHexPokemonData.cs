using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PokemonLetsGoUnity
{
    [System.Serializable]
    public class PLGU_PKHexPokemonData
    {
        public string m_species;
        public List<string> m_moves = new List<string>();
        public List<string> m_relearnMoves = new List<string>();
        public int PID = -1;
        public int m_gender = -1;
        public int m_form = -1;
        public int m_obtainMode = 0;
        public bool m_hasPokerus = false;
        public int m_stagePokerus = 0;
        public int m_strainPokerus = 0;
        public int m_daysPokerus = 0;
        public int m_happiness = 0;
        public string m_nature;
        public string m_item;
        public string m_ability;
        public int m_currentLevel;
        public int m_currentExperience;
        public bool m_isShiny;
        public List<int> m_ivs = new List<int>();
        public List<int> m_evs = new List<int>();
        public string m_nickname;
        public string m_trainerName;
        public int TID;
        public int STID;
        public int m_trainerGender = -1;
        public string m_ball;
        public string m_msg;
        public string m_location;
        public string m_status;
        public List<bool> m_marks = new List<bool>();

        public PLGU_PKHexPokemonData()
        {

        }
    }
}
