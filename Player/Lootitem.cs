using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class Lootitem
{
    public GameObject itemPrefab;
    [Range(0, 100)] public float dropChance; 
}
