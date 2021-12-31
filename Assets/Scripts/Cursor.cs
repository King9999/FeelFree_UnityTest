using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//the cursor is controllable, and should have input to handle its movement and cursor selection.
public class Cursor : MonoBehaviour
{
    public Menu menu;               //need this to get reference to menu space and any items in that space.
    public int currentPosition;    //menu array index of where the cursor is located.

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //update cursor's position in menu
    }

    public void MoveLeft(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //check if cursor is at the edge of the menu and send it to the other side if true
            if (currentPosition <= 0 || currentPosition == menu.MaxCols || currentPosition == menu.MaxCols * 2)
                currentPosition += menu.MaxCols - 1;
            else
                currentPosition -= 1;
            //currentPosition = currentPosition - 1 < 0 ? currentPosition + menu.MaxCols - 1 : currentPosition - 1;
            Debug.Log("Moving Left");
        }


    }

    public void MoveRight(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (currentPosition == menu.MaxCols - 1 || currentPosition == menu.MaxCols * 2 - 1 
                || currentPosition == menu.MaxCols * 3 - 1)
            {
                currentPosition -= menu.MaxCols - 1;
            }
            else
                currentPosition += 1;
            Debug.Log("Moving Right");
        }

    }

    public void MoveUp(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //jump ahead 6 spaces in the array to get to the intended space
            Debug.Log("Moving Up");
        }

    }

    public void MoveDown(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //move back 6 spaces in the array to get to the intended space
            Debug.Log("Moving Down");
        }
    }
}
