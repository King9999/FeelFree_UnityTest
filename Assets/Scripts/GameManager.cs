using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Icon Data")]
    public IconObject[] iconObjects;
    bool iconObjectsInstantiated;
    public IconObject iconPrefab;       //empty prefab whose copies will contain icon data at runtime.
    bool foundItem;                     //if true, cursor is resting on an item.
    int foundItemIndex;                 //used with Pulse couroutine

    //screen resolution 
    enum ScreenResolution {SevenTwenty, TenEighty, FourK}
    ScreenResolution currentResolution;
    public TextMeshProUGUI resolutionUI;
    int screenWidth, screenHeight, refreshRate;

    //particle
    public GameObject particle;
    ParticleSystem particlePlayer;     

    public Menu inventory;
    public Cursor cursor;
    public GameObject swapIcon;         //only appears when two icons are overlapping
    Vector3 lastCursorPosition;         //tracks cursor's previous position. Used to check if icon text needs to update.
    public IconManager im = IconManager.instance; 
    public static GameManager instance;

    //temp data
    //public int overlappingObjIndex;     //array location of an icon that is overlapping with another.
    //Vector3 originalPos;                //retains an icon's original position when another icon overlaps with it
    //Vector3 tempPos;                    //temporary position of an inventory icon when it overlaps with another icon
    //bool iconIsOverlapping;             //if true, icon moves to a temporary position.

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

        //screen defaults to 720p. Note that Screen.currenResolution returns the screen data for fullscreen only.
        //to get the resolution in windowed mode, must must Screen.width/Screen.height.
        screenWidth = 1280;
        screenHeight = 720;
        //refreshRate = 60;
        Screen.SetResolution(screenWidth, screenHeight, FullScreenMode.Windowed);
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
         //swap icon is hidden by default
        swapIcon.SetActive(false);

        //particle setup. Hidden by default
        particlePlayer = particle.GetComponent<ParticleSystem>();
        particle.gameObject.SetActive(false);

        StartCoroutine(SetAllIcons());

        /*for (int i = 0; i < iconObjects.Length; i++)
        {
            iconObjects[i] = Instantiate(iconPrefab);

            ResetIconObject(iconObjects[i], im.icons);
        }*/

        //get name of item at current cursor's location
        //GetItemNameOnCursor(cursor.transform.position);

       
        /*Resolution[] resolutions = Screen.resolutions;
        foreach (var res in resolutions)
        {
            Debug.Log(res);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        //update screen resolution text
        resolutionUI.text = Screen.width + "x" + Screen.height;

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

        //play particle
        StartCoroutine(PlayParticle(iconObject.transform.position));
        /*particle.gameObject.SetActive(true);
        particle.transform.position = iconObject.transform.position;
        particlePlayer.Play();
        particle.gameObject.SetActive(false);*/
    }

#region Button Controls
    public void OnLeftShoulderButtonPressed(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //change screen resolution
            if (currentResolution <= ScreenResolution.SevenTwenty)
                currentResolution = ScreenResolution.FourK;
            else
                currentResolution--;
            
            ChangeResolution(currentResolution);
            /*switch(currentResolution)
            {
                case ScreenResolution.SevenTwenty:
                    screenWidth = 1280;
                    screenHeight = 720;
                    //Screen.SetResolution(1280, 720, FullScreenMode.Windowed, 60);
                    break;

                case ScreenResolution.TenEighty:
                    screenWidth = 1920;
                    screenHeight = 1080;
                    //Screen.SetResolution(1920, 1080, FullScreenMode.Windowed, 60);
                    break;

                case ScreenResolution.FourK:
                    screenWidth = 3860;
                    screenHeight = 2160;
                    //Screen.SetResolution(3860, 2160, FullScreenMode.Windowed, 60);
                    break;
                
                default:
                    break;
            }

            //update resolution
            Screen.SetResolution(screenWidth, screenHeight, FullScreenMode.Windowed, refreshRate);
            resolutionUI.text = Screen.currentResolution.ToString();*/
        }
    }

    public void OnRightShoulderButtonPressed(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //change screen resolution
            if (currentResolution >= ScreenResolution.FourK)
                currentResolution = ScreenResolution.SevenTwenty;
            else
                currentResolution++;
            
            ChangeResolution(currentResolution);
           
        }
    }

    void ChangeResolution(ScreenResolution resolution)
    {
        Debug.Log("Current Resolution is " + resolution);

        switch(resolution)
        {
            case ScreenResolution.SevenTwenty:
                screenWidth = 1280;
                screenHeight = 720;
                break;

            case ScreenResolution.TenEighty:
                screenWidth = 1920;
                screenHeight = 1080;
                break;

            case ScreenResolution.FourK:
                screenWidth = 3840;
                screenHeight = 2160;
                break;
            
            default:
                break;
        }

        //update resolution. Warn player if a resolution is not supported.
        Screen.SetResolution(screenWidth, screenHeight, FullScreenMode.Windowed);
        //resolutionUI.text = Screen.width + "x" + Screen.height + " @ " + refreshRate + "Hz";
    }
    
#endregion

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

    IEnumerator PlayParticle(Vector3 location)
    {
        particle.gameObject.SetActive(true);
        particle.transform.position = location;
        particlePlayer.Play();
        
        yield return new WaitForSeconds(0.5f);
        particle.gameObject.SetActive(false);
    }

    IEnumerator SetAllIcons()
    {
        for (int i = 0; i < iconObjects.Length; i++)
        {
            iconObjects[i] = Instantiate(iconPrefab);

            ResetIconObject(iconObjects[i], im.icons);
            yield return new WaitForSeconds(0.1f);
        }

        GetItemNameOnCursor(cursor.transform.position);
    }
#endregion
}
