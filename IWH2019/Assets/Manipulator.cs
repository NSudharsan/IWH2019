using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manipulator : MonoBehaviour
{
    public GameObject model;
    public GameObject demoHolograms;

    private void Start()
    {
        HideAll();
    }

    public void HideAll()
    {

        Debug.Log("Hide All)");
        model.SetActive(false);

    }
    public void HideModelOnly()
    {
        Debug.Log("Hide model only)");
        model.SetActive(false);
        Debug.Log("Demoholograms is: " + demoHolograms.name);
        demoHolograms.SetActive(true);
    }
    public void Show()
    {
        Debug.Log("Showing");
        model.SetActive(true);
    }
}
