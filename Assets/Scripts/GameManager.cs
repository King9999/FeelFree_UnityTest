using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject[] iconObjects;
    int MaxIconObjects {get;} = 5;
    public GameObject iconPrefab;       //empty prefab whose copies will contain icon data at runtime.

    public Menu inventory;
    public IconManager im = IconManager.instance;       //this must be public 

    // Start is called before the first frame update
    void Start()
    {
        //icon object set up. Each one is instantiated and given random scriptable object data
        iconObjects = new GameObject[MaxIconObjects];
        Vector3 iconPos = Vector3.zero;

        for (int i = 0; i < iconObjects.Length; i++)
        {
            iconObjects[i] = Instantiate(iconPrefab, iconPos, Quaternion.identity);

            //get random scriptable object data
            int randIcon = Random.Range(0, im.icons.Length);
            SpriteRenderer sr = iconObjects[i].GetComponent<SpriteRenderer>();
            sr.sprite = im.icons[randIcon].iconImage;
            sr.sortingOrder = 1;      //icons should appear above the menu sprite.

            TextMeshProUGUI tm = iconObjects[i].GetComponentInChildren<TextMeshProUGUI>();
            tm.text = im.icons[randIcon].iconName;

            //TODO: place the icon objects into a random position in the inventory
            iconPos = new Vector3(iconPos.x + 1, iconPos.y, iconPos.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
