using System;
using UnityEngine;

public class Liquid : MonoBehaviour
{

    public float pH;

    private Renderer _renderer;
    private static readonly int BaseColor = Shader.PropertyToID("_Color");
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    private void OnEnable()
    {
        _renderer = GetComponent<Renderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColor(Color color)
    {
        _renderer.material.SetColor(BaseColor, color);
        _renderer.material.SetColor(EmissionColor, color);
    }
    
}
