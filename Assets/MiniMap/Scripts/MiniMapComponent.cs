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
            
            if (GameObject.Find("MessageDisplay") == null)
            {
                GameObject displayObj = new GameObject("MessageDisplay");
                displayObj.AddComponent<MessageDisplay>();
            }
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
        
        string message = isFinishLine ? "Race Completed!" : $"Arrive {gameObject.name}";
        
        MessageDisplay.SetMessage(message, 2.0f);
        
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

public class MessageDisplay : MonoBehaviour
{
    private static string message = "";
    private static float timer = 0f;
    
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                message = "";
            }
        }
    }
    
    void OnGUI()
    {
        if (!string.IsNullOrEmpty(message))
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 36;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.yellow;
            style.alignment = TextAnchor.MiddleCenter;
            
            GUI.Label(new Rect(0, Screen.height/2 - 50, Screen.width, 100), message, style);
            
            GUI.color = Color.black;
            GUI.Label(new Rect(2, Screen.height/2 - 48, Screen.width, 100), message, style);
            GUI.Label(new Rect(-2, Screen.height/2 - 48, Screen.width, 100), message, style);
            GUI.Label(new Rect(0, Screen.height/2 - 52, Screen.width, 100), message, style);
            GUI.Label(new Rect(0, Screen.height/2 - 48, Screen.width, 100), message, style);
            
            GUI.color = Color.yellow;
            GUI.Label(new Rect(0, Screen.height/2 - 50, Screen.width, 100), message, style);
        }
    }
    
    public static void SetMessage(string newMessage, float displayTime)
    {
        message = newMessage;
        timer = displayTime;
    }
}