using System;
using Unity.VisualScripting;
using UnityEngine;

public class TestPaper : MonoBehaviour
{
    private Renderer _renderer;
    private int _lerpCount;
    private Color _destinationColor = Color.white, _startColor = Color.white;
    [Serialize] public int lerpTime = 1000;

    private static readonly Color32[] PhBaseColors =
    {
        new(238, 55, 34, 255), // 1
        new(238, 52, 121, 255), // 2
        new(245, 126, 38, 255), // 3
        new(251, 169, 35, 255), // 4
        new(244, 236, 8, 255), // 5
        new(163, 205, 57, 255), // 6
        new(77, 184, 71, 255), // 7
        new(1, 146, 71, 255), // 8
        new(5, 148, 149, 255), // 9
        new(81, 117, 186, 255), // 10
        new(69, 74, 159, 255), // 11
        new(42, 47, 132, 255), // 12
        new(148, 36, 140, 255), // 13
        new(123, 39, 121, 255), // 14
    };

    private static readonly int BaseColor = Shader.PropertyToID("_Color");

    private static Color pHToColor(float pH)
    {
        var lowerIndex = (int)Math.Floor(pH);
        var upperIndex = (int)Math.Ceiling(pH);

        // Make sure the indices are within bounds
        lowerIndex = Math.Max(1, Math.Min(14, lowerIndex));
        upperIndex = Math.Max(1, Math.Min(14, upperIndex));

        // Interpolate between the two nearest base colors
        var t = pH - lowerIndex;
        var lowerColor = PhBaseColors[lowerIndex - 1];
        var upperColor = PhBaseColors[upperIndex - 1];

        var r = (byte)(lowerColor.r + t * (upperColor.r - lowerColor.r));
        var g = (byte)(lowerColor.g + t * (upperColor.g - lowerColor.g));
        var b = (byte)(lowerColor.b + t * (upperColor.b - lowerColor.b));

        return new Color32(r, g, b, 255);
    }


    // Start is called before the first frame update
    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _lerpCount = lerpTime + 1;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_lerpCount++ <= lerpTime)
        {
            _renderer.material.SetColor(BaseColor, GetColor());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"OnTriggerEnter: {collision.gameObject.gameObject.tag}");
        if (collision.gameObject.CompareTag("Liquid"))
        {
            var liquid = collision.gameObject.GetComponent<Liquid>();
            Debug.Log($"Liquid has ph {liquid.pH}");
            Debug.Log($"That would be the color {pHToColor(liquid.pH)}");
            _startColor = GetColor();
            _destinationColor = pHToColor(liquid.pH);
            _lerpCount = 0;

            GameManager.Instance.SetSecondaryPanelText("use the microphone to submit your pH guess");
        }
    }

    private Color GetColor()
    {
        return Color.Lerp(_startColor, _destinationColor, 1 - Mathf.Pow(1 - (float)_lerpCount / lerpTime, 3));
    }

}
