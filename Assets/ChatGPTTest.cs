using System;
using System.Linq;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using UnityEngine;

public class ChatGPTTest : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        Debug.Log("Init OpenAI");
        var openAiService = new OpenAIService(new OpenAiOptions()
        {
            // ApiKey =  Environment.GetEnvironmentVariable("MY_OPEN_AI_API_KEY")!,
            // Organization = Environment.GetEnvironmentVariable("MY_OPEN_ORGANIZATION_ID") //optional
        });
        Debug.Log("Request completion");
        var completionResult = await openAiService.Completions.CreateCompletion(new CompletionCreateRequest()
        {
            Prompt = "What is the pH value of vinegar?",
            Model = Models.TextDavinciV3
        });
        if (completionResult.Successful)
        {
            Debug.Log(completionResult.Choices.First().Text);
        }
        else
        {
            if (completionResult.Error == null)
            {
                throw new Exception("Unknown Error");
            }

            Debug.Log($"{completionResult.Error.Code}: {completionResult.Error.Message}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
