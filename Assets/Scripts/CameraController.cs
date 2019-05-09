using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerCameraSystem
{
    public class CameraController : MonoBehaviour
    {
        public GameObject[] players;
        public Camera mainCamera;
        public Camera[] playerCameras;

        public float refDistance;
        private float initialSize;
        public float maxAllowedDistance;

        void Start()
        {
            // Validate references
            if (players.Length <= 1)
            {
                Debug.LogWarning("There's not yet players attached to CameraController script");
                this.enabled = false;
                return;
            }

            if (players.Length != playerCameras.Length)  // We should create a class for players and their cameras
            {
                Debug.LogWarning("There's not yet players or cameras attached to CameraController script");
                this.enabled = false;
                return;
            }

            if (mainCamera == null)
            {
                Debug.LogWarning("There's no main camera attached to CameraController script");
                this.enabled = false;
                return;
            }

            // Set initial parameters
            //refDistance = ActualDistance();
            initialSize = mainCamera.orthographicSize;

            // Validate cameras components
            if (mainCamera.GetComponent<Camera2DFollower>() == null)
            {
                mainCamera.gameObject.AddComponent<Camera2DFollower>().target = this.transform;
            }
            for (int i = 0; i < playerCameras.Length; i++)
            {
                Camera camera = playerCameras[i];
                if (camera.GetComponent<Camera2DFollower>() == null)
                {
                    camera.gameObject.AddComponent<Camera2DFollower>().target = players[i].transform;
                }
            }
        }

        void Update()
        {
            if (ActualDistance() < maxAllowedDistance)
            {
                // Active main camera
                mainCamera.gameObject.SetActive(true);
                HandleSingleCamera(mainCamera);

                foreach (Camera camera in playerCameras)
                {
                    camera.gameObject.SetActive(false);
                }
            }
            else
            {
                // Active players cameras
                mainCamera.gameObject.SetActive(false);

                foreach (Camera camera in playerCameras)
                {
                    camera.gameObject.SetActive(true);
                }

                // We should improve this to move cameras to players position before activating them and to select the best scroll side for each one
            }
        }

        private float ActualDistance()
        {
            // If you have more than 2 players, you should return the max distance between them instead of this above line
            return (players[0].transform.position - players[1].transform.position).magnitude;
        }

        private void HandleSingleCamera(Camera _cam)
        {
            // Main camera should follow this gameObject, then set its transform to be the baricenter of players
            transform.position = Vector3.zero;

            foreach (GameObject player in players)
            {
                transform.position += player.transform.position;
            }

            transform.position /= players.Length;  // Mean position

            // If we have multiple players, try to fit them all when using a single camera
            float actualDistance = ActualDistance();
            if (actualDistance > refDistance)
                _cam.orthographicSize = initialSize / refDistance * actualDistance;
        }
    }
}