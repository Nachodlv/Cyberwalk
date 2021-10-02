using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;

        private void Update()
        {
            bool jump = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
            bool crouch = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
            float speed = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? -1 : 0;
            speed += Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
            Debug.Log($"Jump: {jump}, crouch: {crouch}, speeed: {speed}");
            characterController.Move(speed, crouch, jump);
        }
    }
}
