using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Avatar_Cropper
{

    // get the player's avatar as a cropped circular sprite specified by the avatar number
    public static Sprite get_avatar(int av_num)
    {
        return get_avatar(get_avatar_name(av_num), false);
    }

    // get the player's avatar as a rectangular sprite specified by the avatar number
    public static Sprite get_avatar_no_crop(int av_num)
    {
        return get_avatar(get_avatar_name(av_num), true);
    }

    // gets the player's avatar as a cropped sprite specified by the name of the image (must be located in the pokerpro textures folder)
    // the avatar image must be a square originally
    // param square specifies if the image should be cropped to a circle
    public static Sprite get_avatar(string name, bool square)
    {
        Debug.Log(name);
        Texture2D avatar = Resources.Load<Texture2D>(name);
        // check if the avatar is a square
        if (avatar.height != avatar.width)
        {
            Debug.Log("This avatar image is not a square. Using the height as the width.");
        }
        if (!square)
        {
            // assumption that the texture is a square, if not the height is used as the width too
            // crop the avatar texture to a circle
            avatar = getCircularTexture(avatar.height, avatar.height, avatar.height / 2, avatar.height / 2, avatar.height / 2, avatar);
        }
        // return the avatar as a circular sprite
        return Sprite.Create(avatar, new Rect(0, 0, avatar.height, avatar.width), new Vector2(0, 0));
    }

    // sets all player avatar images in the current game using the info provided by the 'join' call
    public static void set_all_avatars(int[] avatar_nums, bool[] isActive)
    {
        // set the avatar for all active players
        for (int i = 0; i < avatar_nums.Length; ++i)
        {
            // check if the player seat is occupied
            if (isActive[i])
            {
                // facebook profile pick
                if (avatar_nums[i] == 0)
                {
                    //TODO
                }
                // premade avatar index
                else
                {
                    set_player_avatar(i, get_avatar(get_avatar_name(avatar_nums[i]), false));// placeholder until new sprite avatars are done
                }
            }
        }
    }

    // gets the filename of the coresponding avatar
    private static string get_avatar_name(int avatar_num)
    {
        switch (avatar_num)
        {
            // TODO add the new avatars
            case 1:
                return "empty_avatar";
            case 2:
                return "";
            case 3:
                return "";
            case 4:
                return "";
            case 5:
                return "";
            case 6:
                return "";
            case 7:
                return "";
            case 8:
                return "";
        }
        return null;
    }

    // sets the player's (specified by player_num) avatar to the avatar Sprite in game (does not change it on the server)
    public static void set_player_avatar(int player_num, Sprite avatar)
    {
        // find the image GameObject coresponding to the player_num's avatar
        // Note: these GameObjects are named "P[player_num]_avatar"
        GameObject player_panel = GameObject.Find("P" + player_num + "_avatar");
        Image av = player_panel.GetComponent<Image>();
        av.sprite = avatar;
    }

    // returns a new image, cropped to a circle defined by the parameters 
    public static Texture2D getCircularTexture(int height, int width, float radius, float center_x, float center_y, Texture2D source)
    {
        Color[] c = source.GetPixels(0, 0, source.width, source.height);
        Texture2D circle = new Texture2D(height, width);
        // copy the pixels from source to the new texture -- only copy the pixels which lie within the circle
        for (int i = 0; i < (height * width); i++)
        {
            int y = Mathf.FloorToInt(((float)i) / ((float)width));
            int x = Mathf.FloorToInt(((float)i - ((float)(y * width))));
            if (radius * radius >= (x - center_x) * (x - center_x) + (y - center_y) * (y - center_y))
            {
                circle.SetPixel(x, y, c[i]);
            }
            else
            {
                circle.SetPixel(x, y, Color.clear);
            }
        }
        circle.Apply();
        return circle;
    }
}
