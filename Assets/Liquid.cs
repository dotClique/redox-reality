using System;
using UnityEngine;

public class Liquid : MonoBehaviour
{

    public float pH;

    private Renderer _renderer;

    private void Awake()
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
        _renderer.material.SetColor("_Color", color);
    }
    
}
