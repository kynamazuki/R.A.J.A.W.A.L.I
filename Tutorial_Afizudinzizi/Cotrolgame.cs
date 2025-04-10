using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cotrolgame : MonoBehaviour {
    
    public GameObject waktuHabis, ulangi;
    // Use this for initialization
    void Start() {
    
    }

    // Update is called once per frame
    void Update() {
        if (ScriptTimer.waktu <= 0) {
            Time.timeScale = 0;
            waktuHabis.gameObject.SetActive(true);
            ulangi.gameObject.SetActive(true);
        }
    }

    public void ulangiTombol(){
        waktuHabis.gameObject.SetActive(false);
        ulangi.gameObject.SetActive(false);
        Time.timeScale = 1;
        ScriptTimer.waktu = 200f;
        SceneManager.LoadScene("SampleScene");
    }
}