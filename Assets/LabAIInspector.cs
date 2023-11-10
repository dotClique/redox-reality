#if (UNITY_EDITOR) 

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LabAI))]
public class LabAIInspector : Editor
{

    private string _requestMessage;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        // Ignore if not playing
        if (!Application.isPlaying)
        {
            return;
        }
        
        // Request via text
        EditorGUILayout.Space();
        GUILayout.Label("Editor Request", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        _requestMessage = GUILayout.TextField(_requestMessage);
        if (GUILayout.Button("Request", GUILayout.Width(80)))
        {
            ((LabAI)target).OrderLiquid(_requestMessage);
        }
        GUILayout.EndHorizontal();
        
    }
}

#endif
