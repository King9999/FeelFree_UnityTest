using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public IconObject[] iconObjects;
    bool iconObjectsInstantiated;
    bool foundItem;                     //if true, cursor is resting on an item.
    int foundItemIndex;                 //used with Pulse couroutine
         
    
    public IconObject iconPrefab;       //empty prefab whose copies will contain icon data at runtime.

    public Menu inventory;
    public Cursor cursor;
    public GameObject swapIcon;         //only appears when two icons are overlapping
    Vector3 lastCursorPosition;         //tracks cursor's previous position. Used to check if icon text needs to update.
    public IconManager im = IconManager.instance; 
    public static GameManager instance;

    //temp data
    public int overlappingObjIndex;     //array location of an icon that is overlapping with another.
    Vector3 originalPos;                //retains an icon's original position when another icon overlaps with it
    Vector3 tempPos;                    //temporary position of an inventory icon when it overlaps with another icon
    bool iconIsOverlapping;             //if true, icon moves to a temporary position.

    //coroutine bools
    bool pulseCoroutineOn;
    bool animateTextCoroutineOn;

    //constants
    public int MaxIconObjects {get;} = 5;
    public int DefaultIconSortingLayer {get;} = 1;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    } 

    // Start is called before the first frame update
    void Start()
    {
        //cursor begins at the first position, which is the top left corner of inventory.
        cursor.transform.position = inventory.inventorySpace[0].transform.position;
        lastCursorPosition = cursor.transform.position;

        //icon object set up. Each one is instantiated and given random scriptable object data
        iconObjects = new IconObject[MaxIconObjects];

        //Random.InitState(1001);

        for (int i = 0; i < iconObjects.Length; i++)
        {
            iconObjects[i] = Instantiate(iconPrefab);

            ResetIconObject(iconObjects[i], im.icons);
        }

        //get name of item at current cursor's location
        GetItemNameOnCursor(cursor.transform.position);

        //swap icon is hidden by default
        swapIcon.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        //update cursor position         
        cursor.transform.position = inventory.inventorySpace[cursor.currentPosition].transform.position;

      

        //whenever cursor moves, need to check if cursor is on an item
        if (lastCursorPosition != cursor.transform.position)
        {
            lastCursorPosition = cursor.transform.position;
            GetItemNameOnCursor(cursor.transform.position);

            //was an item picked up?
            if (cursor.itemPickedUp)
            {
                //update held item's position.
                iconObjects[cursor.heldItemIndex].transform.position = cursor.transform.position;

                //is the held item overlapping with an item in inventory?
                int i = 0;
                bool iconIsOverlapping = false;
                SpriteRenderer iconSr = iconObjects[cursor.heldItemIndex].GetComponent<SpriteRenderer>();

                while(!iconIsOverlapping && i < iconObjects.Length)
                {
                    if (i != cursor.heldItemIndex && iconObjects[i].gameObject.activeSelf && 
                        iconObjects[i].transform.position == iconObjects[cursor.heldItemIndex].transform.position)
                    {
                        //shift item so player can see what they're overlapping with
                        /*overlappingObjIndex = i; 
                        originalPos = iconObjects[i].transform.position;
                        tempPos = new Vector3(originalPos.x, originalPos.y + 1, originalPos.z);
                        iconObjects[i].transform.position = tempPos;*/
                        iconIsOverlapping = true;

                        //display held over the inventory item so player can see what they're swapping 
                        iconSr.color = new Color(iconSr.color.r, iconSr.color.g, iconSr.color.b, 0.4f);                       
                        iconSr.sortingOrder = DefaultIconSortingLayer + 1;   

                        swapIcon.SetActive(true);
                        swapIcon.transform.position = new Vector3(cursor.transform.position.x + 0.3f, cursor.transform.position.y - 0.3f, cursor.transform.position.z);
                    }
                    else
                    {
                        i++;
                        swapIcon.SetActive(false);
                    }
                }

                //if item's not overlapping, place it back to original position.
                if (!iconIsOverlapping)
                {
                    iconSr.color = new Color(iconSr.color.r, iconSr.color.g, iconSr.color.b, 1);
                    iconSr.sortingOrder = DefaultIconSortingLayer;
                    //iconObjects[overlappingObjIndex].transform.position = originalPos;
                    swapIcon.SetActive(false);
                }
            }

        }

        /******Pulse coroutine operation*****/
        if (foundItem)
        {
            if (!pulseCoroutineOn)
            {
                pulseCoroutineOn = true;
                StartCoroutine(Pulse(iconObjects[foundItemIndex]));
            }
        }
        /**********************************/

    }

    public void GetItemNameOnCursor(Vector3 currentCursorPosition)
    {
        foundItem = false;
        int i = 0;
        inventory.itemName.text = "";
        inventory.itemDescription.text = "";
        while (!foundItem && i < iconObjects.Length)
        {
            if (iconObjects[i].gameObject.activeSelf && currentCursorPosition == iconObjects[i].transform.position)
            {
                inventory.itemName.text = iconObjects[i].iconName.text;
                //inventory.itemDescription.text = iconObjects[i].iconDescription.text;
                foundItem = true;
                foundItemIndex = i;

                if (!animateTextCoroutineOn)
                {
                    animateTextCoroutineOn = true;
                    StartCoroutine(AnimateText(0.016f, iconObjects[i].iconDescription.text));
                }

            }
            else
            {
                i++;
                /*if (animateTextCoroutineOn)
                {
                    inventory.itemDescription.text = "";
                    StopCoroutine(AnimateText(0));
                    animateTextCoroutineOn = false;                  
                }*/
            }
        }
    }

    //Replace an icon game object with a new icon and place it at a random location. Note that
    //no existing objects are deleted to prevent potential garbage collection.
    public void ResetIconObject(IconObject iconObject, Icon[] icons)
    {
        //enable object in case it was deactivated.
        iconObject.gameObject.SetActive(true);

        //get random scriptable object data
        int randIcon = Random.Range(0, icons.Length);
        SpriteRenderer sr = iconObject.GetComponent<SpriteRenderer>();
        sr.sprite = icons[randIcon].iconImage;
        sr.sortingOrder = DefaultIconSortingLayer;      //icons should appear above the menu sprite.

        iconObject.iconName.text = icons[randIcon].iconName;
        iconObject.iconDescription.text = icons[randIcon].description;

        //search for an empty space in inventory and place item there.
        int randSpace = Random.Range(0, inventory.isOccupied.Length);

        while(inventory.isOccupied[randSpace] == true)
        {
            randSpace = Random.Range(0, inventory.isOccupied.Length);
        }
        iconObject.transform.position = inventory.inventorySpace[randSpace].transform.position;
        inventory.isOccupied[randSpace] = true;

    }

#region Coroutines
    //This coroutine will increase and decrease an object's scale while the cursor is resting on it.
    IEnumerator Pulse(IconObject icon)
    {
        icon.transform.localScale = new Vector3(1.5f, 1.5f, icon.transform.localScale.z);
        float scaleRate = 1f;

        while(icon.transform.localScale.x > 1f)
        {
            icon.transform.localScale = new Vector3(icon.transform.localScale.x - scaleRate * Time.deltaTime, icon.transform.localScale.y - scaleRate * Time.deltaTime, 
                icon.transform.localScale.z);
            yield return null;
        }
        
        pulseCoroutineOn = false;
        icon.transform.localScale = new Vector3(1, 1, icon.transform.localScale.z);
    }

    //Displays inventory item description text one letter at a time. Should not run again once the text is fully displayed.
    IEnumerator AnimateText(float scrollSpeed, string textToAnimate)
    {
        char[] copy = textToAnimate.ToCharArray();
        inventory.itemDescription.text = "";

        int i = 0;
        while (i < copy.Length)
        {
            if (!foundItem)
            {
                inventory.itemDescription.text = "";
            }
            else
            {
                inventory.itemDescription.text += copy[i];
            }
            i++;
            yield return new WaitForSeconds(scrollSpeed);
        }

        animateTextCoroutineOn = false;
    }

     //moves icon in inventory if it overlaps with held icon
    IEnumerator ShiftIcon(IconObject item)
    {
        Vector3 tempPos = new Vector3(item.transform.position.x, item.transform.position.y + 1, item.transform.position.z);
        Vector3 originalPos = item.transform.position;

        //move item to temporary position
        //while (/*held item is overlapping with inventory item*/)
        //{
            //item.transform.position = tempPos;
            yield return null;
        //}

        //item.transform.position = originalPos;
    }
#endregion
}
