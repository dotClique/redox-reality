using System.Collections;
using System.Collections.Generic;
using Meta.WitAi;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActivateVoice : MonoBehaviour
{

    [SerializeField] private Wit wit;
    [SerializeField] public MicrophoneActivator microphoneActivator;

    // Update is called once per frame
    void Update()
    {
        if (wit == null) wit = GetComponent<Wit>();
    }

    public void TriggerPressed(InputAction.CallbackContext context)
    {
        Debug.Log("TriggerPressed");
        if (context.performed)
        {
            WitActivate();
        }
    }

    public void WitActivate()
    {
        wit.Activate();
    }
    
}
