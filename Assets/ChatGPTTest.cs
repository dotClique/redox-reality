using System;
using System.Collections.Generic;
using System.Linq;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using UnityEngine;
using UnityEngine.UI;

public class ChatGPTTest : MonoBehaviour
{
    private OpenAIService _openAIService;

    [SerializeField] private Text _debugText;
    
    // Start is called before the first frame update
    async void Start()
    {
        Debug.Log("Init OpenAI");
        _openAIService = new OpenAIService(new OpenAiOptions()
        {
            ApiKey = Secrets.OPENAI_API_KEY
        });

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void RequestResponse(string query)
    {
        Debug.Log($"Requesting response with query: {query}");
        // // TODO: get liquid request from Voice SDK speech-to-text
        // var completionResult = await _openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        // {
        //     Messages = new List<ChatMessage>
        //     {
        //         ChatMessage.FromSystem("You are a helpful assistant. You only answer with single float numbers," +
        //                                " no extra explanations. The number must the parsable, so no characters other" +
        //                                " than digits and decimal separator. If you want to answer with a range," +
        //                                " give the average value of the range"),
        //         ChatMessage.FromUser($"What is the pH value of {query}?"),
        //     },
        //     Model = Models.Gpt_3_5_Turbo
        // });
        // if (completionResult.Successful)
        // {
        //     _debugText.text = completionResult.Choices.First().Message.Content;
        // }
        // else
        // {
        //     if (completionResult.Error == null)
        //     {
        //         throw new Exception("Unknown Error");
        //     }
        //
        //     Debug.Log($"{completionResult.Error.Code}: {completionResult.Error.Message}");
        // }
    }
    
    
    
}