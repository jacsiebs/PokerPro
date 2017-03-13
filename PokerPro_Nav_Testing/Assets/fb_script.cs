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
                UnityEngine.SceneManagement.SceneManager.LoadScene("Main_Menu_Scene");
            } else
            {
                Debug.Log("FB is not logged in");
            }
        }
    }
}

