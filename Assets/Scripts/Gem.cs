using System;
using UnityEngine;

public class Gem : MonoBehaviour, IItem
{
    public static event Action<int> OnGemCollect;
    public int gemWorth = 5;

    public void Collect()
    {
        OnGemCollect.Invoke(gemWorth);
        Destroy(gameObject);
    }
}
