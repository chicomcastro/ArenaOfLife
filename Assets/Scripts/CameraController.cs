using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerCameraSystem
{
    public class CameraController : MonoBehaviour
    {
        private GameObject[] players;
        private Camera mainCamera;
        private Camera[] playerCameras;

        public float refDistance;
        private float initialSize;
        public float maxAllowedDistance;

        void Awake()
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

            players = GameObject.FindGameObjectsWithTag("Player");
            playerCameras = new Camera[players.Length];
            for (int i = 0; i < players.Length; i++)
            {
                playerCameras[i] = players[i].transform.parent.gameObject.GetComponentInChildren<Camera>();
            }
        }

        void Start()
        {
            // Validate references
            if (mainCamera == null)
            {
                Debug.LogWarning("There's no main camera attached to CameraController script");
                this.enabled = false;
                return;
            }

            if (players.Length <= 1)
            {
                if (players.Length == 1)
                    mainCamera.gameObject.GetComponent<Camera2DFollower>().target = players[0].transform;

                this.enabled = false;
                return;
            }

            if (players.Length != playerCameras.Length)  // We should create a class for players and their cameras
            {
                Debug.LogWarning("There's not yet players or cameras attached to CameraController script");
                this.enabled = false;
                return;
            }

            // Set initial parameters
            //refDistance = ActualDistance();
            initialSize = mainCamera.orthographicSize;

            // Validate cameras components
            if (mainCamera.GetComponent<Camera2DFollower>().target == null)
            {
                mainCamera.gameObject.AddComponent<Camera2DFollower>().target = this.transform;
            }
            for (int i = 0; i < playerCameras.Length; i++)
            {
                Camera camera = playerCameras[i];
                if (camera.GetComponent<Camera2DFollower>().target == null)
                {
                    camera.gameObject.AddComponent<Camera2DFollower>().target = players[i].transform;
                }
            }

            SetSingleCameraModeActive(false);
            this.enabled = false;
        }

        void Update()
        {
            if (ActualDistance() < maxAllowedDistance)
            {
                // Active main camera
                HandleSingleCamera(mainCamera);
                SetSingleCameraModeActive(true);
            }
            else
            {
                // Active players cameras
                SetSingleCameraModeActive(false);

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

            ValidateTransform();

            // If we have multiple players, try to fit them all when using a single camera
            float actualDistance = ActualDistance();
            if (actualDistance > refDistance)
                _cam.orthographicSize = initialSize / refDistance * actualDistance;
        }

        private void ValidateTransform()
        {
            // Get collider from walls
            // See if camera target is too near from walls
            // Set it to far enough
            transform.position /= players.Length;  // Mean position
        }

        public void SetSingleCameraModeActive(bool _status)
        {

            mainCamera.enabled = _status;

            foreach (Camera camera in playerCameras)
            {
                camera.enabled = !_status;
            }
        }
    }
}