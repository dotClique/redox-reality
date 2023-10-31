using System.Collections;
using System.Collections.Generic;
using Meta.WitAi.Requests;
using UnityEngine;

public class VoiceSimulator : MonoBehaviour
{
    // public LabAI _labAI;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ProcessRequest(VoiceServiceRequest request)
    {
        Debug.Log(request.ToString());
    }
    
}
