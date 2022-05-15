using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handle all the items in the form of Scriptable Objects.
public class InventoryAssets : MonoBehaviour
{
public static InventoryAssets Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    public ItemSO[] itemSOArray;

    public ItemSO chips;
    public ItemSO soda1;
    public ItemSO soda2;
    public ItemSO soda3;
    public ItemSO soda4;
    public ItemSO cookie;
    public ItemSO eggs;

    public ItemSO GetItemSOFromName(string itemSOName) {
        foreach (ItemSO itemSO in itemSOArray) {
            if (itemSO.name == itemSOName) {
                return itemSO;
            }
        }
        return null;
    }


    public Sprite gridBackground;
    public Sprite gridBackground_2;
    public Sprite gridBackground_3;

    public Transform gridVisual;
}