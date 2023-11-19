using System;
using Unity.Tutorials.Core.Editor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

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

    public GameObject primaryPanel;
    public GameObject secondaryPanel;
    
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

        primaryPanel.GetComponentInChildren<Text>().text = "Order liquid using the microphone";
        secondaryPanel.GetComponentInChildren<Text>().text = "use 'Hand Trigger' to grab and 'Index Trigger' to speak";
        secondaryPanel.SetActive(true);

    }

    public async void OnFullVoiceTranscription(string query)
    {
        Debug.Log($"OnFullVoiceTranscription {State} {query}");

        switch (State)
        {
            case GameState.ORDER_LIQUID:
                primaryPanel.GetComponentInChildren<Text>().text = "Processing ...";
                secondaryPanel.SetActive(false);
                secondaryPanel.GetComponentInChildren<Text>().text = "";
                await LabAI.OrderLiquid(query);
                break;
            case GameState.SUBMIT_ANSWER:
                await LabAI.ProcessAnswer(query);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

    public void SetPrimaryPanelText(string text)
    {
        primaryPanel.GetComponentInChildren<Text>().text = text;
    }

    public void SetSecondaryPanelText(string text)
    {
        secondaryPanel.SetActive(!text.IsNullOrEmpty());
        secondaryPanel.GetComponentInChildren<Text>().text = text;
    }

}
