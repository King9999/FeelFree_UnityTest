using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject[,] inventorySpace;
    int MaxRows {get;} = 3;
    int MaxCols {get;} = 6;
    // Start is called before the first frame update
    void Start()
    {
        //menu starts empty. If I can't get around the errors that will result from empty indexes, I can try inserting an empty gameobject.
        inventorySpace = new GameObject[MaxRows, MaxCols];

        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
