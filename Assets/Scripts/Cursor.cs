using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//the cursor is controllable, and should have input to handle its movement and cursor selection.
public class Cursor : MonoBehaviour
{
    public Menu menu;               //need this to get reference to menu space and any items in that space.
    public int currentPosition;    //menu array index of where the cursor is located.
    public bool itemPickedUp;              //changes the behaviour of the ResetInventory method if true.
    public Vector3 heldItemPosition;       //location of item to be moved.
    public GameObject heldItem;
    public int heldItemIndex;       //location of a held item in iconObjects array.

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
            if (itemPickedUp)
            {
                //remove icon 
                itemPickedUp = false;
                Debug.Log("Item Destroyed");
            }
            else
            {
                //reset inventory space
                gm.inventory.isOccupied = new bool[gm.inventory.inventorySpace.Length];

                //replace the current 5 items in inventory with new ones
                foreach (GameObject icon in gm.iconObjects)
                {
                    gm.ResetIconObject(icon, IconManager.instance.icons);
                }

                //Update item name on cursor since items have changed.
                gm.inventory.itemName.text = gm.GetItemNameOnCursor(transform.position);
                Debug.Log("Inventory Reset");
            }
        }
    }

    //Pick up an item. Pressing the same button puts the item down. Pressing the Y button while
    //an item is picked up destroys the item.
    public void SelectIcon(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (!itemPickedUp && gm.inventory.isOccupied[currentPosition])
            {
                itemPickedUp = true;
                gm.inventory.isOccupied[currentPosition] = false;
                Debug.Log("Item Picked Up");

                //remove the item
                int i = 0;
                bool itemFound = false;
                while(!itemFound && i < gm.iconObjects.Length)
                {
                    if (gm.iconObjects[i].transform.position == gm.inventory.inventorySpace[currentPosition].transform.position)
                    {
                        //found item. Record its array index.
                        heldItemIndex = i;
                        itemFound = true;
                        Debug.Log("Picked Up " + gm.inventory.itemName.text);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            else
            {
                //drop item in new space
                //check if space already occupied. If so, pick up the new item, and old item is placed in space.
                if (gm.inventory.isOccupied[currentPosition])
                {
                    //pick up item and replace with previously held item
                }
                else
                {
                    itemPickedUp = false;
                    gm.inventory.isOccupied[currentPosition] = true;
                }
            } 
        }
    }
}
