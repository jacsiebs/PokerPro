using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;


public class fb_script : MonoBehaviour {


    private void Awake()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(InitCallback, OnHideUnity);
        } else
        {
            FB.ActivateApp();
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
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
    
    // logs the current user out of facebook and returns to the login screen
    public void FBlogout()
    {
        Debug.Log("Signing out of facebook");
        // TODO clear all player_id and other player data (new funciton)
        FB.LogOut();
        UnityEngine.SceneManagement.SceneManager.LoadScene("login_scene");
    }

    void AuthCallBack(IResult result)
    {
        if (result.Error != null)
        {
            Debug.Log("Facebook login error: " + result.Error);
        } else
        {
            if(FB.IsLoggedIn)
            {
                Debug.Log("FB is logged in.");
                // record the user id token to use as a unique player id
                GlobalVars.fb_id = AccessToken.CurrentAccessToken.UserId;
                // used to get the player_id from the fb_id once the main menu is loaded

                // switch to the main menu
                UnityEngine.SceneManagement.SceneManager.LoadScene("Main_Menu_Scene");
            } else
            {
                Debug.Log("FB is not logged in");
            }
        }
    }
}

