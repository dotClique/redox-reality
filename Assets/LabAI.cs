using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using UnityEngine;
using UnityEngine.UI;

public class LabAI : MonoBehaviour
{
    private OpenAIService _openAIService;

    [SerializeField] private Text queryText;

    [SerializeField] private Text phText;

    [SerializeField] private Text colorText;

    [SerializeField] private Text viscosityText;

    [SerializeField] private GameObject liquidBeakerToSpawn;

    [SerializeField] private GameObject liquidBeakerSpawnPoint;

    private List<GameObject> spawnedBeakers = new();
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Init OpenAI");
        _openAIService = new OpenAIService(new OpenAiOptions()
        {
            ApiKey = Secrets.OPENAI_API_KEY
        });
    }

    private async Task<string> RequestPH(string liquid)
    {
        Debug.Log($"Requesting pH value for {liquid}");

        var completionResult = await _openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                // TODO: prompt engineer
                ChatMessage.FromSystem("You are a helpful assistant. You only answer with single float numbers," +
                                       " no extra explanations. The number must the parsable, so no characters other" +
                                       " than digits and decimal separator. If you want to answer with a range," +
                                       " give the average value of the range. If you can't find the answer, make" +
                                       " up a number. There is no excuse to not provide a number."),
                ChatMessage.FromUser($"What is the pH value of {liquid}?"),
            },
            Model = Models.Gpt_3_5_Turbo
        });
        
        if (completionResult.Successful)
        {
            return completionResult.Choices.First().Message.Content;
        }
        if (completionResult.Error == null)
        {
            throw new Exception("Unknown Error");
        }
        Debug.Log($"Completion Error - {completionResult.Error.Code}: {completionResult.Error.Message}");
        return null;
    }

    private async Task<string> RequestColor(string liquid)
    {
        var completionResult = await _openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                // TODO: prompt engineer
                ChatMessage.FromSystem("You are a helpful assistant. You only answer with a single hex code," +
                                       " no extra explanations. If there are multiple possible answer, pick the most " +
                                       "correct one. Otherwise pick at random. The string must start with a number " +
                                       "sign '#' and be parsable, so no characters other than hex digits. The format" +
                                       " should be #000000."),
                ChatMessage.FromUser($"What is the color of {liquid}?"),
            },
            Model = Models.Gpt_3_5_Turbo
        });
        if (completionResult.Successful)
        {
            return completionResult.Choices.First().Message.Content;
        }
        if (completionResult.Error == null)
        {
            throw new Exception("Unknown Error");
        }
        Debug.Log($"{completionResult.Error.Code}: {completionResult.Error.Message}");
        return null;
    }
    
    private async Task<string> RequestViscosity(string liquid)
    {
        var completionResult = await _openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                // TODO: prompt engineer
                ChatMessage.FromSystem("You are a helpful assistant. You only answer with single float numbers," +
                                       " no extra explanations. The number must the parsable, so no characters other" +
                                       " than digits and decimal separator. If you want to answer with a range," +
                                       " give the average value of the range. If you can't find the answer, make" +
                                       " up a number. There is no excuse to not provide a number." +
                                       " The unit should be pascal-seconds, but don't include the unit symbol in" +
                                       " the answer."),
                ChatMessage.FromUser($"What is the viscosity of {liquid}?"),
            },
            Model = Models.Gpt_3_5_Turbo
        });
        if (completionResult.Successful)
        {
            return completionResult.Choices.First().Message.Content;
        }
        if (completionResult.Error == null)
        {
            throw new Exception("Unknown Error");
        }
        Debug.Log($"{completionResult.Error.Code}: {completionResult.Error.Message}");
        return null;
    }

    public async void RequestResponse(string query)
    {
        Debug.Log($"Requesting response from LabAI with query: {query}");
        queryText.text = query;
        phText.text = "...";
        colorText.text = "...";
        colorText.color = Color.white;
        viscosityText.text = "...";

        var result = await Task.WhenAll(
            RequestPH(query),
            RequestColor(query)
            // RequestViscosity(query)
        );
        
        phText.text = result[0] ?? "[ERROR]";

        Color color;
        ColorUtility.TryParseHtmlString(result[1], out color);
        // color = color ?? Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        colorText.text = $"#{ColorUtility.ToHtmlStringRGB(color)}";
        colorText.color = color;
        
        // viscosityText.text = result[2] ?? "[ERROR]";

        foreach (var beaker in spawnedBeakers)
        {
            Destroy(beaker);
        }
        spawnedBeakers = new List<GameObject> { Instantiate(liquidBeakerToSpawn, liquidBeakerSpawnPoint.transform) };
    }
    
}
