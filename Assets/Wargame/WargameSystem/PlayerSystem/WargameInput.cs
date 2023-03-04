using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wargame.PlayerSystem
{
    [RequireComponent(typeof(PlayerInput))]
    public class WargameInput : MonoBehaviour
    {

        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool fire;
        public bool reload;
        public bool aim;

        public int select;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;

        void OnMove(InputValue value)
        {
            move = value.Get<Vector2>();
        }

        void OnLook(InputValue value)
        {
            look = value.Get<Vector2>();
        }

        void OnJump(InputValue value)
        {
            jump = value.isPressed;
        }

        void OnSprint(InputValue value)
        {
            sprint = value.isPressed;
        }

        void OnFire(InputValue value)
        {
            fire = value.isPressed;
        }

        void OnReload(InputValue value)
        {
            reload = value.isPressed;
        }
        
        void OnAim(InputValue value)
        {
            aim = value.isPressed;
        }

        void OnSelectPrimary(InputValue value)
        {
            select = 0;
        }
        void OnSelectSecondary(InputValue value)
        {
            select = 1;
        }
        void OnSelectMelee(InputValue value)
        {
            select = 2;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}