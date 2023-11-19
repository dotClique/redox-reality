using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Primitives;
using TMPro;
using UnityEngine;

public class DebugLogCanvas : MonoBehaviour
{
    [SerializeField] int maxLines = 50;
    [SerializeField] TextMeshProUGUI debugLogText;

    Queue<string> queue = new Queue<string>();

    private Canvas _canvas;

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
        if (_canvas is not null)
        {
            _canvas.enabled = false;
        }
    }

    void OnEnable()
    {
        Application.logMessageReceivedThreaded += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceivedThreaded -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Delete oldest message
        if (queue.Count >= maxLines) queue.Dequeue();

        queue.Enqueue(logString);

        var builder = new StringBuilder();
        foreach (string st in queue)
        {
            builder.Append("> ").Append(st).Append("\n");
        }

        debugLogText.text = builder.ToString();
    }

    public void ToggleCanvas()
    {
        if (_canvas is not null)
        {
            _canvas.enabled = !_canvas.enabled;
        }
    }
    
}