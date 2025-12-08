// ISTA 425 / INFO 525 Algorithms for Games
//
// Sample code file

using UnityEngine;

namespace MiniMap.Scripts
{
    public class MiniMapEntity
    {
        public Sprite Icon;
        public bool rotateWithObject = true;
        public Vector2 Size;
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
        private float playerCarRadius = 2.0f;

        void Start()
        {
            miniMapController = GameObject.Find("MiniMap").GetComponent<MiniMapController>();
            mme = new MiniMapEntity();
            mme.Icon = icon;
            mme.Size = size;
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
        
            if (CheckSphereCollision())
            {
                TriggerWaypoint();
            }
        }
    
        bool CheckSphereCollision()
        {
            Vector3 playerPos = playerCar.transform.position;
            Vector3 waypointPos = transform.position;
        
            float dx = playerPos.x - waypointPos.x;
            float dy = playerPos.y - waypointPos.y;
            float dz = playerPos.z - waypointPos.z;
        
            float distanceSquared = dx*dx + dy*dy + dz*dz;
            float radiusSum = triggerDistance + playerCarRadius;
            float radiusSumSquared = radiusSum * radiusSum;
        
            return distanceSquared <= radiusSumSquared;
        }

        void TriggerWaypoint()
        {
            hasBeenTriggered = true;
        
            PlaySound();
        
            string message = isFinishLine ? "Race Completed!" : $"Waypoint {waypointNumber} Reached!";
        
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
            }
        }
    
        public static void SetMessage(string newMessage, float displayTime)
        {
            message = newMessage;
            timer = displayTime;
        }
    }
}