using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public GameObject Owner { get; set; }
    public GameObject SourcePrefab { get; set; }
    public bool IsItemActive;
    public string WeaponName;
    public Sprite WeaponIcon;

    public virtual void Equip(bool equip)
    {

    }
}
