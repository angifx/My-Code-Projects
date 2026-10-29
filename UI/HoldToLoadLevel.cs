using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HoldToLoadLevel : MonoBehaviour
{
    public float holdDuration = 1f; 
    public Image fillCircle;

    private float holdTimer = 0;
    private bool isHolding = false;

    public static event Action OnHoldComplete; 
   
    void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime; 
            fillCircle.fillAmount = holdTimer / holdDuration;
            if(holdTimer >= holdDuration)
            {
                OnHoldComplete.Invoke();
                ResetHold();
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
            ResetHold();
        }
    }

    private void ResetHold()
    {
        isHolding = false; 
        holdTimer = 0f; 
        if (fillCircle != null)
            fillCircle.fillAmount = 0f;
    }
}
