using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class image_grabber : MonoBehaviour {

    private static Image avatar = null;
    private static Image rank = null;
    private static Text username = null;
    private static WWW www = null;
    private static bool isDone = false;

	// Use this for initialization
	void Awake () {
//        get_player_id(GlobalVars.fb_id);
        // wait until the server responds to continue (player_id must be set)
//        WaitForSeconds w;
//        while (!www.isDone)
//            w = new WaitForSeconds(0.1f);

        // load in the player avatar and rank images
        StartCoroutine(set_player_info());
        load_avatar();
        load_rank();
	}

    private void Update()
    {
        if(isDone)
        {
            Debug.Log("Updating the main menu with player data.");
            update_images();
            isDone = false;
        }
        
    }

    // set GlobalVars.avatar_num, GlobalVars.sprite, GlobalVars.rank, and GlobalVars.rank_sprite
    private IEnumerator set_player_info()
    {
        Debug.Log("Asking server for player info in main menu...");
        string url = "http://104.131.99.193/playerStats/" + GlobalVars.player_id;
        WWW www = new WWW(url);
        // wait for a response
        yield return www;
 
        Debug.Log("Got a player object.");
        string jsonString = www.text;
        var playerJson = JsonMapper.ToObject(jsonString);// parse
        GlobalVars.ELO = (int) playerJson["elo"];
        GlobalVars.avatar_num = (int) playerJson["avatarId"];
        GlobalVars.username = playerJson["name"].ToString();
        Debug.Log(GlobalVars.username + " has been logged in.");
        //GlobalVars.rank = Ranker.getRank(GlobalVars.ELO);// Not completed with elo calculations yet
        //GlobalVars.rank_sprite = Ranker.getSprite(GlobalVars.rank);// incomplete

        GlobalVars.square_avatar = Avatar_Cropper.get_avatar_no_crop(GlobalVars.avatar_num);
        GlobalVars.avatar = Avatar_Cropper.get_avatar(GlobalVars.avatar_num);
        isDone = true;      
    }

    // call to the server to get the player_id from fb_id
    private IEnumerator get_player_id(string fb_id)
    {
        string url = "http://104.131.99.193/register" + fb_id;//TODO
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

    // updates the rank and image sprites on screen as well as the player's username
    public static void update_images()
    {
        if (avatar == null || rank == null || username == null)
            Debug.Log("You wouldn't dare call this before awake and loads.");
        avatar.sprite = GlobalVars.square_avatar;
        rank.sprite = GlobalVars.rank_sprite;
        Debug.Log("username: " + GlobalVars.username);
        username.text = GlobalVars.username;
    }

    // load the avatar panel into this script so that the sprite can be updated
    private void load_avatar()
    {
        GameObject user_panel = GameObject.Find("user_avatar");
        avatar = user_panel.GetComponent<Image>();
        // also load the username panel
        username = GameObject.Find("username").GetComponent<Text>();
    }

    // load rank panel inot this script so that the sprite can be updated
    private void load_rank()
    {
        GameObject rank_panel = GameObject.Find("Rank");
        rank = rank_panel.GetComponent<Image>();
    }

}
