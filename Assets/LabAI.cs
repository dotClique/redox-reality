using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using OpenAI;
using UnityEngine;

public class LabAI : MonoBehaviour
{
    private OpenAIApi _openAI;

    [SerializeField] private GameObject liquidBeakerToSpawn;

    [SerializeField] private GameObject liquidBeakerSpawnPoint;

    private GameObject spawnedBeaker;

    // Start is called before the first frame update
    void Start()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

        Debug.Log("Init OpenAI");
        _openAI = new OpenAIApi(Secrets.OPENAI_API_KEY);
    }

    private async Task<string> RequestPH(string liquid)
    {
        Debug.Log($"Requesting pH value for '{liquid}'");
        var response = await _openAI.CreateChatCompletion(new CreateChatCompletionRequest
        {
            Model = "gpt-3.5-turbo-0613",
            Messages = new List<ChatMessage>
            {
                new()
                {
                    Role = "system",
                    Content = "You are a helpful assistant. You only answer with single float numbers," +
                              " no extra explanations. The number must the parsable, so no characters other" +
                              " than digits and decimal separator. If you want to answer with a range," +
                              " give the average value of the range. If you can't find the answer, make" +
                              " up a number. There is no excuse to not provide a number."
                },
                new()
                {
                    Role = "user",
                    Content = $"What is the pH value of {liquid}?"
                }
            }
        });
        if (response.Error != null)
        {
            throw new Exception(
                $"Completion Error: {response.Error.Code} {response.Error.Type} {response.Error.Message}");
        }

        var responseValue = response.Choices.First().Message.Content;
        Debug.Log($"GPT liquid pH response: {responseValue}");
        return responseValue;
    }

    private async Task<string> RequestColor(string liquid)
    {
        Debug.Log($"Requesting color value for '{liquid}'");
        var response = await _openAI.CreateChatCompletion(new CreateChatCompletionRequest
        {
            Model = "gpt-3.5-turbo-0613",
            Messages = new List<ChatMessage>
            {
                new()
                {
                    Role = "system",
                    Content = "You are a helpful assistant. You only answer with a single hex with alpha," +
                              " no extra explanations. If there are multiple possible answer, pick the most " +
                              "correct one. Otherwise pick at random. The string must start with a number " +
                              "sign '#' and be parsable, so no characters other than hex digits. The format" +
                              " should be #RRGGBBAA, e.g. #FF4DB847. Remember that the alpha channel, the last" +
                              " two digits, should match how opaque the liquid is. For example, FF is completely" +
                              "opaque, and 00 is completely transparent."
                },
                new()
                {
                    Role = "user",
                    Content = $"What is the color of {liquid}?"
                }
            }
        });
        if (response.Error != null)
        {
            throw new Exception(
                $"Completion Error: {response.Error.Code} {response.Error.Type} {response.Error.Message}");
        }

        var responseValue = response.Choices.First().Message.Content;
        Debug.Log($"GPT liquid color response: {responseValue}");
        return responseValue;
    }

    private async Task<float?> ParseAnswerToFloat(string answer)
    {
        Debug.Log($"Requesting float value from user answer '{answer}'");
        var response = await _openAI.CreateChatCompletion(new CreateChatCompletionRequest
        {
            Model = "gpt-3.5-turbo-0613",
            Messages = new List<ChatMessage>
            {
                new()
                {
                    Role = "system",
                    Content = "You are a helpful assistant. You only answer with single float numbers," +
                              " no extra explanations. The number must the parsable, so no characters other" +
                              " than digits and decimal separator. If you want to answer with a range," +
                              " give the average value of the range. If you can't find the answer, make" +
                              " up a number. There is no excuse to not provide a number." +
                              " The number is dimensionless with no units. Extract the float number from the" +
                              " speech transcript. Provide only the numeric value without any additional text. " +
                              "If the transcript includes words like 'Five. Three,' I want the response to be '5.3.'"
                },
                new()
                {
                    Role = "user",
                    Content = $"Give me the intended float number from the following transcript '{answer}'"
                }
            }
        });
        if (response.Error != null)
        {
            throw new Exception(
                $"Completion Error: {response.Error.Code} {response.Error.Type} {response.Error.Message}");
        }

        var responseValue = response.Choices.First().Message.Content;
        Debug.Log($"GPT user answer float response: {responseValue}");
        return float.TryParse(responseValue, out var res) ? res : null;
    }

    public async Task OrderLiquid(string query)
    {
        Debug.Log($"Requesting liquid response from LabAI with query: {query}");

        GameManager.Instance.SetPrimaryPanelText(query);
        GameManager.Instance.SetSecondaryPanelText("analyzing...");

        var result = await Task.WhenAll(
            RequestPH(query),
            RequestColor(query)
        );

        GameManager.Instance.SetSecondaryPanelText("use the test paper and cheat sheet to find pH value");

        Color color;
        ColorUtility.TryParseHtmlString(result[1], out color);

        // Make sure it's not too transparent (even if ChatGPT says so...)
        color.a = Math.Max(color.a, 0.15f);

        if (!float.TryParse(result[0], out var pH))
        {
            Debug.LogError($"Failed to parse liquid pH, string was '{result[0]}'");
            return;
        }

        Debug.Log($"Parsed spawned liquid ph as {pH}");

        Destroy(spawnedBeaker);
        var newSpawnedBeaker = Instantiate(liquidBeakerToSpawn, liquidBeakerSpawnPoint.transform);
        var spawnedLiquids = newSpawnedBeaker.GetComponentsInChildren<Liquid>();
        foreach (var spawnedLiquid in spawnedLiquids)
        {
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