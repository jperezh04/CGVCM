using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace MegaMan25D
{
    public static class GameInput
    {
        public static float Horizontal
        {
            get
            {
                float value = 0f;

#if ENABLE_INPUT_SYSTEM
                Keyboard keyboard = Keyboard.current;
                if (keyboard != null)
                {
                    if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) value -= 1f;
                    if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) value += 1f;
                }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) value -= 1f;
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) value += 1f;
#endif
                return Mathf.Clamp(value, -1f, 1f);
            }
        }

        public static float Vertical
        {
            get
            {
                float value = 0f;

#if ENABLE_INPUT_SYSTEM
                Keyboard keyboard = Keyboard.current;
                if (keyboard != null)
                {
                    if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) value -= 1f;
                    if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) value += 1f;
                }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) value -= 1f;
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) value += 1f;
#endif
                return Mathf.Clamp(value, -1f, 1f);
            }
        }

        public static bool JumpPressedThisFrame
        {
            get
            {
                bool pressed = false;

#if ENABLE_INPUT_SYSTEM
                Keyboard keyboard = Keyboard.current;
                if (keyboard != null)
                {
                    pressed |= keyboard.spaceKey.wasPressedThisFrame;
                    pressed |= keyboard.wKey.wasPressedThisFrame;
                    pressed |= keyboard.upArrowKey.wasPressedThisFrame;
                }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
                pressed |= Input.GetKeyDown(KeyCode.Space);
                pressed |= Input.GetKeyDown(KeyCode.W);
                pressed |= Input.GetKeyDown(KeyCode.UpArrow);
#endif
                return pressed;
            }
        }

        public static bool FireHeld
        {
            get
            {
                bool held = false;

#if ENABLE_INPUT_SYSTEM
                Keyboard keyboard = Keyboard.current;
                Mouse mouse = Mouse.current;

                if (keyboard != null)
                {
                    held |= keyboard.jKey.isPressed;
                    held |= keyboard.xKey.isPressed;
                    held |= keyboard.leftCtrlKey.isPressed;
                }

                if (mouse != null)
                {
                    held |= mouse.leftButton.isPressed;
                }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
                held |= Input.GetKey(KeyCode.J);
                held |= Input.GetKey(KeyCode.X);
                held |= Input.GetKey(KeyCode.LeftControl);
                held |= Input.GetMouseButton(0);
#endif
                return held;
            }
        }
    }
}
