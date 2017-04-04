using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Change_to_main : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // switch view to main menu
    public void transitionToMain()
    {
        Debug.Log("Returning to Main Menu...");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main_Menu_Scene");
    }
}
