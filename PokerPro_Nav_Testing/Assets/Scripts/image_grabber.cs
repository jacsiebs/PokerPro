﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class image_grabber : MonoBehaviour {

    private static Image avatar = null;
    private static Image rank = null;
    private static WWW www = null;

	// Use this for initialization
	void Awake () {
//        get_player_id(GlobalVars.fb_id);
        // wait until the server responds to continue (player_id must be set)
//        WaitForSeconds w;
//        while (!www.isDone)
//            w = new WaitForSeconds(0.1f);

        // load in the player avatar and rank images
        set_player_info();
        load_avatar();
        load_rank();
        update_images();
	}

    // set GlobalVars.avatar_num, GlobalVars.sprite, GlobalVars.rank, and GlobalVars.rank_sprite
    private void set_player_info()
    {
        // get the avatar_num from the server
//        get_avatar_num();
        // wait until the server response with the avatar_num
//        WaitForSeconds w;
//        while (!www.isDone)
//            w = new WaitForSeconds(0.1f);

        GlobalVars.square_avatar = Avatar_Cropper.get_avatar_no_crop(GlobalVars.avatar_num);
        GlobalVars.avatar = Avatar_Cropper.get_avatar(GlobalVars.avatar_num);
    }

    // call to the server to get the player's current avatar number
    private IEnumerator get_avatar_num()
    {
        
        string url = "";// TODO
        www = new WWW(url);
        yield return www;
        GlobalVars.avatar_num = int.Parse(www.text);
    }

    // call to the server to get the player_id from fb_id
    private IEnumerator get_player_id(string fb_id)
    {
        string url = "" + fb_id;//TODO
        WWW www = new WWW(url);
        yield return www;
        GlobalVars.player_id = www.text;
    } 

    // moves the setting gear for when it is pressed, use for on-click
    public void settings_clicked()
    {
        // rotate the sprite 
        GameObject settings_button = GameObject.Find("Settings");

        // TODO animate rotation of gear
        //        RectTransform rectTransform = settings_button.GetComponent<RectTransform>();
        //        rectTransform.Rotate(new Vector3(0, 0, 45));

        // load into the settings view
        UnityEngine.SceneManagement.SceneManager.LoadScene("Settings");
    }

    // updates the rank and image sprites on screen
    public static void update_images()
    {
        if (avatar == null || rank == null)
            Debug.Log("You wouldn't dare call this before awake and loads.");
        avatar.sprite = GlobalVars.square_avatar;
        rank.sprite = GlobalVars.rank_sprite;
    }

    // load the avatar panel into this script so that the sprite can be updated
    private void load_avatar()
    {
        GameObject user_panel = GameObject.Find("user_avatar");
        avatar = user_panel.GetComponent<Image>();
    }

    // load rank panel inot this script so that the sprite can be updated
    private void load_rank()
    {
        GameObject rank_panel = GameObject.Find("Rank");
        rank = rank_panel.GetComponent<Image>();
    }

}
