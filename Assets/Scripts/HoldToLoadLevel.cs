using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HoldToLoadLevel : MonoBehaviour
{
    public float holdDuration = 1.0f; //how long player needs to hold to load the next level
    public Image fillCircle;

    private float holdTimer = 0.0f;
    private bool isHolding = false;

    public static event Action OnHoldComplete;

    // Update is called once per frame
    void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime;
            fillCircle.fillAmount = holdTimer / holdDuration;
            if (holdTimer >= holdDuration)
            {
                // load next level code
                OnHoldComplete.Invoke(); // "GameController" handles the rest
                ResetHolding();
            }
        }
    }

    public void OnHold(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isHolding = true;
        }
        else if (context.canceled)
        {
            ResetHolding();
        }
    }

    private void ResetHolding()
    {
        isHolding = false;
        holdTimer = 0.0f;
        fillCircle.fillAmount = 0.0f;
    }
}
