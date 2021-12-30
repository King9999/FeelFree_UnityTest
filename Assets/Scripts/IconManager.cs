using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* This script will create 5 icon game objects at runtime using a single prefab. The icons are scriptable objects. */
public class IconManager : MonoBehaviour
{
    //int MaxIconObjects {get;} = 5;
    public Icon[] icons;     //55 total
    //public GameObject[] iconObjects;
    //public GameObject iconPrefab;       //empty prefab whose copies will contain icon data at runtime.
    public static IconManager instance;

    // Start is called before the first frame update
    void Start()
    {
        //icon object set up. Each one is instantiated and given random scriptable object data
        /*iconObjects = new GameObject[MaxIconObjects];
        Vector3 iconPos = Vector3.zero;

        for (int i = 0; i < iconObjects.Length; i++)
        {
            iconObjects[i] = Instantiate(iconPrefab, iconPos, Quaternion.identity);

            //get random scriptable object data
            int randIcon = Random.Range(0, icons.Length);
            SpriteRenderer sr = iconObjects[i].GetComponent<SpriteRenderer>();
            sr.sprite = icons[randIcon].iconImage;

            TextMeshProUGUI tm = iconObjects[i].GetComponentInChildren<TextMeshProUGUI>();
            tm.text = icons[randIcon].iconName;

            iconPos = new Vector3(iconPos.x + 1, iconPos.y, iconPos.z);
        }*/
        //SpriteRenderer sr = GetComponent<SpriteRenderer>();
        //sr.sprite = icon[0].iconImage;
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);    //Only want one instance of icon manager
            return;
        }

        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
