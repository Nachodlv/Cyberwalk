using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;

        private void Update()
        {
            float rotation = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ? 1 : 0;
            rotation += Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ? -1 : 0;

            bool jump = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

            float speed = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? -1 : 0;
            speed += Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1 : 0;

            characterController.Move(speed, speed, false, jump);
        }
    }
}
