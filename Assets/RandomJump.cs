using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = Unity.Mathematics.Random;

public class RandomJump : MonoBehaviour
{
    [Serialize]
    public int maxCount = 50;
    private int _counter;
    [Serialize]
    public float velocity = 0.5f;

    private Random _random;
    // Start is called before the first frame update
    void Start()
    {
        _random = new Random();
        _random.InitState();
    }

    // Update is called once per frame
    void Update()
    {
        if (_counter++ < maxCount) return;
        _counter = 0;
        var children = GameObject.FindGameObjectsWithTag("Liquid");
        children[_random.NextInt(children.Length)].GetComponent<Rigidbody>().velocity = Vector3.up * velocity;
    }
}
