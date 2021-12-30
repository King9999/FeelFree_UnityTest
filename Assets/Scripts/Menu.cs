using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject[] inventorySpace;
    public bool[] isOccupied;               //if true, an item is in this space.
    public int MaxRows {get;} = 3;
    public int MaxCols {get;} = 6;

    void Awake()
    {
        //menu starts empty. If I can't get around the errors that will result from empty indexes, I can try inserting an empty gameobject.
        //inventorySpace = new GameObject[MaxRows, MaxCols];
        isOccupied = new bool[inventorySpace.Length];
    }
    // Start is called before the first frame update
    void Start()
    {
        //menu starts empty. If I can't get around the errors that will result from empty indexes, I can try inserting an empty gameobject.
        //inventorySpace = new GameObject[MaxRows, MaxCols];
        //isOccupied = new bool[inventorySpace.Length];
    

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
