using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* This script will create 5 icon game objects at runtime using a single prefab. The icons are scriptable objects. */
public class IconManager : MonoBehaviour
{
    public Icon[] icons;     //55 total
    public static IconManager instance;
   
    //Only want one instance of icon manager
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

}
