using UnityEngine;
using TMPro;

//The only thing this script does is change the text for the Y and A button's functionality depending on whether 
//an item is selected or not.
public class ButtonControlText : MonoBehaviour
{
    public TextMeshProUGUI yButtonUI;
    public TextMeshProUGUI aButtonUI;
    public Cursor cursorState;

    // Start is called before the first frame update
    void Start()
    {
        yButtonUI.text = "Reset Inventory";
        aButtonUI.text = "Select Item";
    }

    // Update is called once per frame
    void Update()
    {
        if (cursorState.itemPickedUp)
        {
            yButtonUI.text = "Destroy Item";
            aButtonUI.text = "Place Item";
        }
        else
        {
            yButtonUI.text = "Reset Inventory";
            aButtonUI.text = "Select Item";
        }
    }
}
