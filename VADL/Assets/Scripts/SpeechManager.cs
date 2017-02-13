using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechManager : MonoBehaviour
{
    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    // Use this for initialization
    void Start()
    {
        keywords.Add("Reset world", () =>
        {
            // Call the OnReset method on every descendant object.
            this.BroadcastMessage("OnReset");
        });

        keywords.Add("Place Scene", () =>
        {
            this.BroadcastMessage("OnSelect");
        });

        keywords.Add("Rocket Launch", () =>
        {
            this.BroadcastMessage("OnLaunch");
        });

        keywords.Add("Increase size", () =>
        {
            this.BroadcastMessage("OnIncreaseSize");
        });

        keywords.Add("Decrease size", () =>
        {
            this.BroadcastMessage("OnDecreaseSize");
        });

        keywords.Add("Increase thrust", () =>
        {
            this.BroadcastMessage("OnThrustIncrease");
        });

        keywords.Add("Decrease thrust", () =>
        {
            this.BroadcastMessage("OnThrustDecrease");
        });

        keywords.Add("Increase Drag", () =>
        {
            this.BroadcastMessage("OnDragIncrease");
        });

        keywords.Add("Decrease Drag", () =>
        {
            this.BroadcastMessage("OnDragDecrease");
        });

        // Tell the KeywordRecognizer about our keywords.
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }
}