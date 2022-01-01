using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject[] iconObjects;
    public int MaxIconObjects {get;} = 5;
    public GameObject iconPrefab;       //empty prefab whose copies will contain icon data at runtime.

    public Menu inventory;
    public Cursor cursor;
    Vector3 lastCursorPosition;         //tracks cursor's previous position. Used to check if icon text needs to update.
    public IconManager im = IconManager.instance;       //this must be public

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
        //cursor begins at the first position.
        cursor.transform.position = inventory.inventorySpace[0].transform.position;
        lastCursorPosition = cursor.transform.position;

        //icon object set up. Each one is instantiated and given random scriptable object data
        iconObjects = new GameObject[MaxIconObjects];

        //Random.InitState(1001);

        for (int i = 0; i < iconObjects.Length; i++)
        {
            iconObjects[i] = Instantiate(iconPrefab);

            //get random scriptable object data
            int randIcon = Random.Range(0, im.icons.Length);
            SpriteRenderer sr = iconObjects[i].GetComponent<SpriteRenderer>();
            sr.sprite = im.icons[randIcon].iconImage;
            sr.sortingOrder = 1;      //icons should appear above the menu sprite.

            TextMeshProUGUI tm = iconObjects[i].GetComponentInChildren<TextMeshProUGUI>();
            tm.text = im.icons[randIcon].iconName;

            //search for an empty space in inventory and place item there.
            int randSpace = Random.Range(0, inventory.isOccupied.Length);

            while(inventory.isOccupied[randSpace] == true)
            {
                randSpace = Random.Range(0, inventory.inventorySpace.Length);
            }
            iconObjects[i].transform.position = inventory.inventorySpace[randSpace].transform.position;
            inventory.isOccupied[randSpace] = true;

        }

        //get name of item at current cursor's location
        inventory.itemName.text = GetItemNameOnCursor(cursor.transform.position);

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
            inventory.itemName.text = GetItemNameOnCursor(cursor.transform.position);
        }
    }

    string GetItemNameOnCursor(Vector3 currentCursorPosition)
    {
        bool foundItem = false;
        int i = 0;
        string itemName = "";
        while (!foundItem && i < iconObjects.Length)
        {
            if (currentCursorPosition == iconObjects[i].transform.position)
            {
                TextMeshProUGUI tm = iconObjects[i].GetComponentInChildren<TextMeshProUGUI>();
                itemName = tm.text;
                foundItem = true;
            }
            else
            {
                i++;
            }
        }

        return itemName;
    }
}
