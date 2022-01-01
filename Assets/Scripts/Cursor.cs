using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//the cursor is controllable, and should have input to handle its movement and cursor selection.
public class Cursor : MonoBehaviour
{
    public Menu menu;               //need this to get reference to menu space and any items in that space.
    public int currentPosition;    //menu array index of where the cursor is located.

    public GameManager gm = GameManager.instance;


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
            //If cursor is in top row (the first 6 positions in array), add number of columns * 2 to position.
            //otherwise, subtract number of columns from position.
            if (currentPosition < menu.MaxCols)
                currentPosition += menu.MaxCols * 2;
            else
                currentPosition -= menu.MaxCols;
            Debug.Log("Moving Up");
        }

    }

    public void MoveDown(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //If cursor is in bottom row (the last 6 positions in array), subtract number of columns * 2 from position.
            //otherwise, add number of columns to position.
            if (currentPosition >= menu.MaxCols * 2)
                currentPosition -= menu.MaxCols * 2;
            else
                currentPosition += menu.MaxCols;
            Debug.Log("Moving Down");
        }
    }

    //NOTE: This method has a different function if an item is currently being held with the A button.
    public void ResetInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //replace the current 5 items in inventory with new ones
            int i = 0;
            if (gm == null)
            {
                Debug.Log("GM instance is null");
                return;
            }
            while (i < gm.MaxIconObjects)
            {
                Destroy(gm.iconObjects[gm.iconObjects.Length - 1]);
                i++;
            }
            Debug.Log("Inventory Reset");
        }
    }
}
