using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gem : MonoBehaviour, IItem
{
    public static event Action<int> OnGemCollect;
    public int worth = 1;
    private bool collected = false;
    public void Collect()
    {
        if (collected) return;
        collected = true;

        OnGemCollect.Invoke(worth);
        SoundEffectMangager.Play("gem"); 
        Destroy(gameObject);
    }
}