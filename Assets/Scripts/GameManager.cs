using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public IconObject[] iconObjects;
    bool iconObjectsInstantiated;
    bool foundItem;                     //if true, cursor is resting on an item.
    int foundItemIndex;                 //used with Pulse couroutine
    bool pulseCoroutineOn;       
    public int MaxIconObjects {get;} = 5;
    public IconObject iconPrefab;       //empty prefab whose copies will contain icon data at runtime.

    public Menu inventory;
    public Cursor cursor;
    Vector3 lastCursorPosition;         //tracks cursor's previous position. Used to check if icon text needs to update.
    public IconManager im = IconManager.instance; 
    public static GameManager instance;

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

            //get random scriptable object data
            /*int randIcon = Random.Range(0, im.icons.Length);
            SpriteRenderer sr = iconObjects[i].GetComponent<SpriteRenderer>();
            sr.sprite = im.icons[randIcon].iconImage;
            sr.sortingOrder = 1;      //icons should appear above the menu sprite.

            TextMeshProUGUI tm = iconObjects[i].GetComponentInChildren<TextMeshProUGUI>();
            tm.text = im.icons[randIcon].iconName;

            //search for an empty space in inventory and place item there.
            int randSpace = Random.Range(0, inventory.isOccupied.Length);

            while(inventory.isOccupied[randSpace] == true)
            {
                randSpace = Random.Range(0, inventory.isOccupied.Length);
            }
            iconObjects[i].transform.position = inventory.inventorySpace[randSpace].transform.position;
            inventory.isOccupied[randSpace] = true;*/

        }

        //get name of item at current cursor's location
        /*inventory.itemName.text =*/ GetItemNameOnCursor(cursor.transform.position);

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
            /*inventory.itemName.text =*/ GetItemNameOnCursor(cursor.transform.position);

            //was an item picked up?
            if (cursor.itemPickedUp)
            {
                //update held item's position.
                iconObjects[cursor.heldItemIndex].transform.position = cursor.transform.position;
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
                //TextMeshProUGUI tm = iconObjects[i].GetComponentInChildren<TextMeshProUGUI>();
                inventory.itemName.text = iconObjects[i].iconName.text;
                inventory.itemDescription.text = iconObjects[i].iconDescription.text;
                foundItem = true;
                foundItemIndex = i;
            }
            else
            {
                i++;
                foundItem = false;
            }
        }

        //return itemName;
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
        sr.sortingOrder = 1;      //icons should appear above the menu sprite.

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

        //get name of item at current cursor's location in case it changed
        //inventory.itemName.text = GetItemNameOnCursor(cursor.transform.position);
    }

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
}
