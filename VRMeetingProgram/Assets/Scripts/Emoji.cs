using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Reflection;

public class Emoji : MonoBehaviour
{
    public GameObject[] Emojis;

    public int index;

    private float fDestroyTime = 2f;
    private float fTickTime = 2f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        fTickTime += Time.deltaTime;
        if (fTickTime >= fDestroyTime)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                CreateEmoji("crying");
                fTickTime = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                CreateEmoji("Smiley01");
                fTickTime = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                CreateEmoji("Smiley04");
                fTickTime = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                CreateEmoji("Smiley16");
                fTickTime = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                CreateEmoji("Smiley40");
                fTickTime = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                CreateEmoji("Smiley50");
                fTickTime = 0;
            }
        }
        
    }

    void CreateEmoji(string emoji)
    {

        Debug.Log("CreateEmoji");
        Transform spawnPoints = GameObject.Find("EmojiSpawnPoint").GetComponentInChildren<Transform>();
        //Transform[] spawnPoints = GameObject.Find("SpqwnPointGroup").GetComponentsInChildren<Transform>();
        Vector3 pos = spawnPoints.position;
        Quaternion rot = spawnPoints.rotation;

        var parent = gameObject;

        PhotonNetwork.Instantiate(emoji, pos, rot).transform.parent = parent.transform;
    }

    void merong() { }

    

    
}


