using UnityEngine;

//Takes 2 background images and scrolls them horizontally. When one of the images goes offscreen, it's moved to the other side of the trailing image.
public class BackgroundScroller : MonoBehaviour
{
    public GameObject[] backgrounds;
    float scrollSpeed;
    public GameManager gm = GameManager.instance;
    //Vector3 screenPos;          //used to get screen coordinates to check boundaries based on position of game manager.
    SpriteRenderer bgSr;

    float screenBoundary;       //number of units to reach one edge of the screen from the origin.
    float xOffset;              //used to cover any small gaps between backgrounds.

    // Start is called before the first frame update
    void Start()
    {
        scrollSpeed = 1;
        //screenPos = Camera.main.WorldToViewportPoint(gm.transform.position);
        bgSr = backgrounds[0].GetComponent<SpriteRenderer>();   //both backgrounds are same, doesn't matter which one is referenced.
        screenBoundary = bgSr.GetComponent<SpriteRenderer>().bounds.extents.x;
        //Debug.Log("Screen Pos " + screenPos.x);
        Debug.Log("screenBoundary " + screenBoundary);
        xOffset = 0.08f;
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject background in backgrounds)
        {
            background.transform.position = new Vector3(background.transform.position.x - scrollSpeed * Time.deltaTime, background.transform.position.y,
                background.transform.position.z);
            
            //if a background is out of bounds, move it to other side of screen
            if (background.transform.position.x + bgSr.GetComponent<SpriteRenderer>().bounds.extents.x < -screenBoundary)   //checking left screen boundary
            {
                background.transform.position = new Vector3(screenBoundary * 2 - xOffset, background.transform.position.y,
                    background.transform.position.z);
            }
        }
    }
}
