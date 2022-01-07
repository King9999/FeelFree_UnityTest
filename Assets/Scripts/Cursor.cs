using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

//the cursor is controllable, and should have input to handle its movement and cursor selection.
public class Cursor : MonoBehaviour
{
    public Menu menu;                                //this is used to access Menu variables for moving the cursor.
    public int currentPosition;                     //menu array index of where the cursor is located.
    public bool itemPickedUp;                       //changes the behaviour of the ResetInventory method if true.
    [HideInInspector]public int heldItemIndex;       //location of a held item in iconObjects array.
    bool cursorBlinking;                            //if true, will run the Blink coroutine
    bool coroutineRunning;                          //ensures the Blink coroutine runs once per frame

    SpriteRenderer cursorSr;                        //used with Blink coroutine.

    public GameManager gm = GameManager.instance;


    void Start()
    {
        cursorSr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        /******Blink coroutine operation*****/
        if (cursorBlinking)
        {
            if (!coroutineRunning)
            {
                coroutineRunning = true;
                StartCoroutine(Blink());
            }
        }
        else
        {
            cursorSr.enabled = true;    //ensures that cursor sprite is enabled when coroutine stops
        }
        /**********************************/


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
            
            gm.soundSource.PlayOneShot(gm.cursorSound);
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
            gm.soundSource.PlayOneShot(gm.cursorSound);
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
            
            gm.soundSource.PlayOneShot(gm.cursorSound);
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
                
            gm.soundSource.PlayOneShot(gm.cursorSound);
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
                gm.iconObjects[heldItemIndex].gameObject.SetActive(false);
                itemPickedUp = false;
                cursorBlinking = false;
                //gm.inventory.itemName.text = "";
                Debug.Log("Item Destroyed");
            }
            else
            {
                //reset inventory space
                gm.inventory.isOccupied = new bool[gm.inventory.inventorySpace.Length];

                //replace the current 5 items in inventory with new ones
                /*foreach (IconObject icon in gm.iconObjects)
                {
                    gm.ResetIconObject(icon, IconManager.instance.icons);
                }*/
                StartCoroutine(gm.SetAllIcons());

                //Update item name on cursor since items have changed.
                /*gm.inventory.itemName.text =*/ gm.GetItemNameOnCursor(transform.position);
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
            if (!itemPickedUp)
            {
                if (gm.inventory.isOccupied[currentPosition])
                {
                    itemPickedUp = true;
                    gm.inventory.isOccupied[currentPosition] = false;
                    gm.soundSource.PlayOneShot(gm.pickupItem, 0.5f);

                    Debug.Log("Item Picked Up");

                    //cursor will blink when item is selected
                    cursorBlinking = true;

                    //find the item to be picked up
                    int i = 0;
                    bool itemFound = false;

                    while(!itemFound && i < gm.iconObjects.Length)
                    {
                        if (gm.iconObjects[i].gameObject.activeSelf && gm.iconObjects[i].transform.position == gm.inventory.inventorySpace[currentPosition].transform.position)
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
            }
            else
            {
                //drop item in new space
                //check if space already occupied. If so, pick up the new item, and old item is placed in inventory.
                if (gm.inventory.isOccupied[currentPosition])
                {
                    int i = 0;
                    bool itemFound = false;
                    SpriteRenderer iconSr = gm.iconObjects[heldItemIndex].GetComponent<SpriteRenderer>();

                    while(!itemFound && i < gm.iconObjects.Length)
                    {
                        if (i != heldItemIndex && gm.iconObjects[i].gameObject.activeSelf && 
                            gm.iconObjects[i].transform.position == gm.inventory.inventorySpace[currentPosition].transform.position)
                        {
                            //change held item's alpha back to 1 in case it was changed in game manager
                            iconSr.color = new Color(iconSr.color.r, iconSr.color.g, iconSr.color.b, 1);
                            iconSr.sortingOrder = gm.DefaultIconSortingLayer;

                            //swap inventory item with held item
                            IconObject tempObj = gm.iconObjects[i];
                            gm.iconObjects[i] = gm.iconObjects[heldItemIndex];
                            gm.iconObjects[heldItemIndex] = tempObj;

                            //reset data for replaced item since its position won't have changed
                            iconSr = gm.iconObjects[heldItemIndex].GetComponent<SpriteRenderer>();
                            iconSr.sortingOrder = gm.DefaultIconSortingLayer + 1;
                            iconSr.color = new Color(iconSr.color.r, iconSr.color.g, iconSr.color.b, 0.4f);
                            gm.GetItemNameOnCursor(transform.position);

                            //play particle effect
                            StartCoroutine(gm.PlayParticle(transform.position));

                            itemFound = true;
                            Debug.Log("Items Swapped");
                        }
                        else
                        {
                            i++;
                        }
                    }
                    
                }
                else    //drop item in empty space
                {
                    itemPickedUp = false;
                    gm.inventory.isOccupied[currentPosition] = true;
                    cursorBlinking = false;

                    //play particle effect
                    StartCoroutine(gm.PlayParticle(transform.position));

                    gm.GetItemNameOnCursor(transform.position);
                    Debug.Log("Item Dropped");
                }
            } 
        }
    }

    //cursor will rapidly turn on and off when item is selected.
    IEnumerator Blink()
    {
        cursorSr.enabled = !cursorSr.enabled;
        yield return new WaitForSeconds(0.08f);
        coroutineRunning = false;
    }

   
}
