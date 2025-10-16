using UnityEngine;
using UnityEngine.InputSystem;

public class Footsteps : MonoBehaviour
{
    public AudioSource footstepsSound;
    public AudioSource sprintSound;

    void Update()
    {
        var keyboard = Keyboard.current;

        if (keyboard == null)
            return;

        bool isMoving =
            keyboard.wKey.isPressed ||
            keyboard.aKey.isPressed ||
            keyboard.sKey.isPressed ||
            keyboard.dKey.isPressed;

        bool isSprinting = keyboard.leftShiftKey.isPressed;

        if (isMoving)
        {
            if (isSprinting)
            {
                footstepsSound.enabled = false;
                sprintSound.enabled = true;
            }
            else
            {
                footstepsSound.enabled = true;
                sprintSound.enabled = false;
            }
        }
        else
        {
            footstepsSound.enabled = false;
            sprintSound.enabled = false;
        }
    }
}
