using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Cloud.Language.V1;
using Google.Apis.Auth.OAuth2;

public class GoogleNatureLanguageTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LanguageServiceClient client = LanguageServiceClient.Create();
        Document document = Document.FromPlainText(
            "我去大润发买鱼");
        Debug.Log(document.Content);
        AnalyzeSyntaxResponse response = client.AnalyzeSyntax(document);
        Debug.Log($"Detected language: {response.Language}");
        Debug.Log($"Number of sentences: {response.Sentences.Count}");
        Debug.Log($"Number of tokens: {response.Tokens.Count}");
        foreach (Token t in response.Tokens)
        {
            Debug.Log("r part Of Speech: " + t.PartOfSpeech + " dependency edge: " + t.DependencyEdge);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
