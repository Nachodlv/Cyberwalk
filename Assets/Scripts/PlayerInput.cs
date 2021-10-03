using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace DefaultNamespace
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Shooter playerShooter;

        private Camera _cachedCamera;
        private void Start()
        {
            _cachedCamera = Camera.main;
        }

        private void Update()
        {
            if (characterController)
            {
                float rotation = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ? 1 : 0;
                rotation += Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ? -1 : 0;

                bool jump = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

                float speed = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? -1 : 0;
                speed += Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1 : 0;

                characterController.Move(speed, speed, false, jump);
            }

            UpdateShooter();
        }

        private void UpdateShooter()
        {
            Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 worldPosition = _cachedCamera.ScreenToWorldPoint(screenPosition);
            playerShooter.LookAt(worldPosition);

            if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
            {
                playerShooter.Shoot();
            }
        }
    }
}
