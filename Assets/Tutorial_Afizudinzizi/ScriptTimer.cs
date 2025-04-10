using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptTimer : MonoBehaviour {
    
    Text text;
    public static float waktu = 200f;
    // Use this for initialization 
    void Start() {
        text = GetComponent<Text> ();
    }

    // Update is called once per frame
    void Update() {
        waktu -= Time.deltaTime;
        if (waktu < 0)
            waktu = 0;
        text.text = " 0 " + Mathf.Round(waktu);
    }
}
