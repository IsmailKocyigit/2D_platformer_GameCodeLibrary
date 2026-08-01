using System;
using UnityEngine;

public class HealthItem : MonoBehaviour, IItem
{
    public static event Action<int> OnHealthCollect;

    public int healAmount = 1;

    public void Collect()
    {
        OnHealthCollect.Invoke(healAmount);
        Destroy(gameObject);
    }
}
