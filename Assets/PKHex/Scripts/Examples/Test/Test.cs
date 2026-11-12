using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		var list = PKHexForUnity.PKHexUtils.GetItemNames(SystemLanguage.Spanish);
		foreach (var l in list)
			Debug.Log(l);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
