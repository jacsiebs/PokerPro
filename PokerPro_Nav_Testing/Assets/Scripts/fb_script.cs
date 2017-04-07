using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;


public class fb_script : MonoBehaviour {


    private void Awake()
    {
        FB.Init(SetInit, OnHideUnity);
    }

    void SetInit()
    {

    }

    void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void FBlogin()
    {
        // only neeed basic public permissions
        List<string> permissions = new List<string>();
        permissions.Add("public_profile");
        FB.LogInWithReadPermissions(permissions, AuthCallBack);
    }

    void AuthCallBack(IResult result)
    {
        if (result.Error != null)
        {
            Debug.Log(result.Error);
        } else
        {
            if(FB.IsLoggedIn)
            {
                Debug.Log("FB is logged in");
                // record the user id token to use as a unique player id
                // Note: The full access token is too long to use with this mechanism
                //       A better system will be implemented in iteration 2
                //       For now the token is simply cut to a proper size
                GlobalVars.player_id = AccessToken.CurrentAccessToken.TokenString.Substring(0,2);
                if (GlobalVars.player_id == null)
                    Debug.Log("User access token is null");
                
                // switch to the main menu
                UnityEngine.SceneManagement.SceneManager.LoadScene("Main_Menu_Scene");
            } else
            {
                Debug.Log("FB is not logged in");
            }
        }
    }
}

