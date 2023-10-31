using System.Collections;
using System.Collections.Generic;
using Meta.WitAi;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ActivateVoice : MonoBehaviour
{
    [SerializeField] private Wit wit;
    [SerializeField] public XRGrabInteractable microphoneInteractable;
    [SerializeField] public MicrophoneActivator microphoneActivator;

    private bool _listening;

    public bool Listening
    {
        get => _listening;
        set
        {
            if (_listening == value) return;
            microphoneActivator.MicrophoneActivated = value;
            if (value)
            {
                wit.Activate();
            }
            else
            {
                wit.Deactivate();
            }

            _listening = value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (wit == null) wit = GetComponent<Wit>();
    }

    public void OnActivate(InputValue inputValue)
    {
        Debug.Log("TriggerPressed");
        if (inputValue.isPressed && microphoneInteractable.isSelected)
        {
            Listening = true;
        }
        else Listening = false;
    }

    public void OnListenTimeout()
    {
        Listening = false;
    }
}
