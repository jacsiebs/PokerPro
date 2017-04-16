using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Avatar_Selector : MonoBehaviour {

    // Use this for initialization
    void Awake () {
        // get the pre-made sprites and load them into the images

        load_avatars();
        update_avatar_pic();
    }

    // loads in the sprites for the avatar selector display 
    private void load_avatars()
    {
        // TODO update for more sprties
        // add each avatar to a button on the selector display
        for (int i = 1; i < 2; i++)
        {
            GameObject button = GameObject.Find("avatar_" + i);
            button.GetComponent<Image>().sprite = Avatar_Cropper.get_avatar_no_crop(i);
        }
    }

    // use this for on-click
    public void change_avatar(int avatar_num)
    {
        StartCoroutine(change_avatar_helper(avatar_num));
    }

    // updates the server with this player's new avatar selection 
    // also update the avatar on screen
    private static IEnumerator change_avatar_helper(int avatar_num)
    {
        // send the update request
        string url = "http://104.131.99.193/changeStat/avatarId/" + GlobalVars.player_id + "/" + avatar_num;// placeholder url
        WWW www = new WWW(url);
        yield return www;
        GlobalVars.avatar_num = avatar_num;

        GlobalVars.avatar = Avatar_Cropper.get_avatar(avatar_num);
        // change the avatar on screen
        update_avatar_pic();
    }

    // updates the avatar picture in the settings view
    private static void update_avatar_pic()
    {
        GameObject avatar_display = GameObject.Find("Avatar");
        Image av_image = avatar_display.GetComponent<Image>();
        av_image.sprite = GlobalVars.avatar;
    }
}
