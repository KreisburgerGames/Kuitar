using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaylistManager : MonoBehaviour
{
    public GameObject OSTOne;
    public GameObject Custom;
    public List<GameObject> Playlists = new List<GameObject>();

    void Start()
    {
        Playlists.Add(OSTOne);
        Playlists.Add(Custom);
    }

    public void ShowOSTOne()
    {
        OSTOne.SetActive(true);
        PlayerPrefs.SetString("LastPlaylist", "OST1");
        foreach(GameObject playlist in Playlists)
        {
            if(playlist != OSTOne)
            {
                playlist.SetActive(false);
            }
        }
    }

    public void ShowCustom()
    {
        Custom.SetActive(true);
        PlayerPrefs.SetString("LastPlaylist", "Custom");
        foreach(GameObject playlist in Playlists)
        {
            if(playlist != Custom)
            {
                playlist.SetActive(false);
            }
        }
    }
}
