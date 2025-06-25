using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunItem : MonoBehaviour
{
    public Sprite sprite;
    public string gunName;

    public GunItem(string name, Sprite sprite) {
        this.sprite = sprite;
        this.gunName = name;
    }
}
