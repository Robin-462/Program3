// ISTA 425 / INFO 525 Algorithms for Games
//
// Sample code file

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapEntity
{
    public Sprite icon;
    public bool rotateWithObject = true;
    public Vector2 size;
}

public class MiniMapComponent : MonoBehaviour 
{
    public Sprite icon;
    public Vector2 size = new Vector2(20,20);
    public bool rotateWithObject = false;
    public bool isWaypoint = false;
    public int waypointNumber = 1;
    public bool isFinishLine = false;
    public float triggerDistance = 10.0f;
    public AudioClip waypointSound;
    public AudioClip finishSound;

    MiniMapController miniMapController;
    MiniMapEntity mme;
    MapObject mmo;

    private bool hasBeenTriggered = false;
    private GameObject playerCar;
    private AudioSource audioSource;

    void Start()
    {
        miniMapController = GameObject.Find("MiniMap").GetComponent<MiniMapController>();
        mme = new MiniMapEntity();
        mme.icon = icon;
        mme.size = size;
        mme.rotateWithObject = rotateWithObject;

        mmo = miniMapController.RegisterMapObject(this.gameObject, mme);

        if (isWaypoint || isFinishLine)
        {
            playerCar = GameObject.Find("Player Car");
            
            if (playerCar == null)
            {
                playerCar = GameObject.FindGameObjectWithTag("Player");
            }
            
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        if ((!isWaypoint && !isFinishLine) || hasBeenTriggered || playerCar == null)
            return;
        
        float distance = Vector3.Distance(transform.position, playerCar.transform.position);
        
        if (distance <= triggerDistance)
        {
            TriggerWaypoint();
        }
    }

    void TriggerWaypoint()
    {
        hasBeenTriggered = true;
        
        PlaySound();
        
        string message = isFinishLine ? "Race Completed!" : $"Waypoint {waypointNumber} reached!";
        Debug.Log(message);
        
        enabled = false;
    }
    
    void PlaySound()
    {
        if (audioSource == null) return;
        
        AudioClip soundToPlay = isFinishLine ? finishSound : waypointSound;
        
        if (soundToPlay != null)
        {
            audioSource.PlayOneShot(soundToPlay);
        }
        else
        {
            audioSource.Play();
        }
    }
}