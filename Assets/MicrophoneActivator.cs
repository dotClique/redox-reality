using UnityEngine;

public class MicrophoneActivator : MonoBehaviour
{
    private const float Offset = 0.006f;

    [SerializeField] private Transform onOffSwitch;
    [SerializeField] private GameObject lightup;

    private bool _microphoneActivated;

    public bool MicrophoneActivated
    {
        get => _microphoneActivated;
        set
        {
            if (_microphoneActivated == value) return;
            onOffSwitch.localPosition += Vector3.up * (value ? Offset : -Offset);
            lightup.SetActive(value);
            _microphoneActivated = value;
        }
    }
}
