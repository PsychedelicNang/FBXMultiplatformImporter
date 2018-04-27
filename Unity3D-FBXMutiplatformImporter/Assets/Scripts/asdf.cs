using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using System;

public class asdf : MonoBehaviour {

	[DllImport("Test")]
	static public extern int GetNumber(int number);

	// Use this for initialization
	void Start () {
		print (GetNumber (10));
	}
	
	// Update is called once per frame
	void Update () {
	}
}
