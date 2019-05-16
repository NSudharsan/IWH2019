using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionManager : MonoBehaviour
{
    private TextToSpeech textToSpeech;
    private void Awake()
    {
        textToSpeech = GetComponent<TextToSpeech>();
    }
    public void GiveInstructions(string speakText)
    {
        // create message
        var msg = string.Format(speakText, textToSpeech.Voice.ToString());
        // speak msg
        textToSpeech.StartSpeaking(msg);
        Debug.Log("finished speaking text: "+msg);
    }
}
