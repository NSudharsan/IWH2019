using ClientConsole;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JSONReader : MonoBehaviour
{
    string json;
    //string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);
    public string filePath;
    private object webRequest;
    List<Vector3> listVectors = new List<Vector3>();

    // Start is called before the first frame update
    public List<Vector3> ReturnVectorList()
    {
        //Debug.Log("filepath is: " + filePath);
        //Debug.Log(File.Exists(filePath));
        if (File.Exists(filePath))
        {
            listVectors.Clear();
            json = File.ReadAllText(filePath);
            MyResponse result = JsonConvert.DeserializeObject<MyResponse>(json);
            //Debug.Log("Is result null? " + result);
            //Debug.Log("Is allroutes null? " + result.Allroutes);

            foreach (var posItem in result.Allroutes.Route)
            {
                //Debug.Log("result is " + posItem.x);
                //Debug.Log("result is " + posItem.y);
                //Debug.Log("result is " + posItem.z);
                listVectors.Add(new Vector3(posItem.x, posItem.y, posItem.z));
            }
            Debug.Log("List of vectors: " + listVectors.Count);
            if (listVectors != null) return listVectors;
            else return null;
        }
        else return null;
    }

}
