using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using UnityEngine;

public class LabAI : MonoBehaviour
{
    private OpenAIService _openAIService;

    [SerializeField] private GameObject liquidBeakerToSpawn;

    [SerializeField] private GameObject liquidBeakerSpawnPoint;

    private GameObject spawnedBeaker;
    
    // Start is called before the first frame update
    async void Start()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

        Debug.Log("Init OpenAI");
        _openAIService = new OpenAIService(new OpenAiOptions()
        {
            ApiKey = Secrets.OPENAI_API_KEY
        });
        Debug.Log($"OpenAIService ListModel {await _openAIService.Models.ListModel()}");
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
        Debug.Log($"Requesting color value for {liquid}");
        var completionResult = await _openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                // TODO: prompt engineer
                ChatMessage.FromSystem("You are a helpful assistant. You only answer with a single hex with alpha," +
                                       " no extra explanations. If there are multiple possible answer, pick the most " +
                                       "correct one. Otherwise pick at random. The string must start with a number " +
                                       "sign '#' and be parsable, so no characters other than hex digits. The format" +
                                       " should be #RRGGBBAA, e.g. #FF4DB847. Remember that the alpha channel, the last" +
                                       " two digits, should match how transparent the liquid is"),
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
        Debug.LogError($"{completionResult.Error.Code}: {completionResult.Error.Message}");
        return null;
    }

    private async Task<float?> ParseAnswerToFloat(string answer)
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
                                       " The number is dimensionless with no units. Extract the float number from the speech transcript. Provide only the numeric value without any additional text. If the transcript includes words like 'Five. Three,' I want the response to be '5.3.'"),
                ChatMessage.FromUser($"Give me the intended float number from the following transcript '{answer}'"),
            },
            Model = Models.Gpt_3_5_Turbo
        });
        if (completionResult.Successful)
        {
            return float.TryParse(completionResult.Choices.First().Message.Content, out var res) ? res : null;
        }
        if (completionResult.Error == null)
        {
            throw new Exception("Unknown Error");
        }
        Debug.LogError($"{completionResult.Error.Code}: {completionResult.Error.Message}");
        return null;
    }

    public async Task OrderLiquid(string query)
    {
        Debug.Log($"Requesting liquid response from LabAI with query: {query}");

        GameManager.Instance.SetPrimaryPanelText(query);
        GameManager.Instance.SetSecondaryPanelText("analyzing...");

        var result = await Task.WhenAll(
            RequestPH(query),
            RequestColor(query)
            // RequestViscosity(query)
        );
        

        GameManager.Instance.SetSecondaryPanelText("use the test paper and cheat sheet to find pH value");
        
        Debug.Log($"Result: {result[0]},{result[1]}");
        
        Color color;
        ColorUtility.TryParseHtmlString(result[1], out color);
        
        // Make sure it's not too transparent (even if ChatGPT says so...)
        color.a = Math.Min(color.a, 0.5f);

        // viscosityText.text = result[2] ?? "[ERROR]";
        
        if (!float.TryParse(result[0], out var pH))
        {
            Debug.LogError($"Failed to parse liquid pH, string was '{result[0]}'");
            return;
        }
        Debug.Log($"Parsed spawned liquid ph as {pH}");
        
        Destroy(spawnedBeaker);
        var newSpawnedBeaker = Instantiate(liquidBeakerToSpawn, liquidBeakerSpawnPoint.transform);
        var spawnedLiquids = newSpawnedBeaker.GetComponentsInChildren<Liquid>();
        foreach (var spawnedLiquid in spawnedLiquids) {
            spawnedLiquid.pH = pH;
            spawnedLiquid.SetColor(color);
        }
        spawnedBeaker = newSpawnedBeaker;

        GameManager.Instance.State = GameManager.GameState.SUBMIT_ANSWER;
    }

    public async Task ProcessAnswer(string answer)
    {
        Debug.Log($"Processing answer {answer}");
        var floatAnswer = await ParseAnswerToFloat(answer);

        if (floatAnswer == null)
        {
            GameManager.Instance.SetPrimaryPanelText("[ERROR]");
            return;
        }
        
        Debug.Log($"Parsed answer as float {floatAnswer}");

        var spawnedLiquid = spawnedBeaker.GetComponentInChildren<Liquid>();
        GameManager.Instance.SetPrimaryPanelText($"You: {floatAnswer}\nAnswer: {spawnedLiquid.pH}");
        GameManager.Instance.SetSecondaryPanelText("Order a new liquid to try again");
        
        GameManager.Instance.State = GameManager.GameState.ORDER_LIQUID;
    }
    
}
