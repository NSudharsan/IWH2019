using ClientConsole;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class APICaller : MonoBehaviour
{
    public string url;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetText());
    }
    IEnumerator GetText()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();


        if (webRequest.isNetworkError)
        {
            Debug.Log("Error in accessing the service");
        }
        else
        {
            //MyResponse result = JsonUtility.FromJson<MyResponse>(webRequest.downloadHandler.text);
            MyResponse result = JsonConvert.DeserializeObject<MyResponse>(webRequest.downloadHandler.text);
            Debug.Log("Is result null? " + result);
            Debug.Log("Is allroutes null? " + result.Allroutes);
            foreach (var posItem in result.Allroutes.Route)
            {
                Debug.Log("result is " + posItem.x);
            }
        }
    }
     
}
