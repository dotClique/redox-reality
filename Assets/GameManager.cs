using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public enum GameState
    {
        ORDER_LIQUID,
        SUBMIT_ANSWER
    }
    
    public static GameManager Instance { get; private set; }

    public GameState State;

    public LabAI LabAI;
    
    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    public async void OnFullVoiceTranscription(string query)
    {
        Debug.Log($"OnFullVoiceTranscription {State} {query}");

        switch (State)
        {
            case GameState.ORDER_LIQUID:
                LabAI.OrderLiquid(query);
                break;
            case GameState.SUBMIT_ANSWER:
                LabAI.ProcessAnswer(query);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

}
