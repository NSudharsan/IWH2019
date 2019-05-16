using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateVectors : MonoBehaviour
{
    public Vector3 destinationVector;
    private Vector3 currentPosition;
    private Vector3 previousPosition;
    public GameObject destinationPrefab;
    public GameObject linePrefab;
    Vector3 centerPos;

    float distance;
    // save all lines in scene
    private Stack<Line> Lines = new Stack<Line>();

    public SpatialMappingObserver spatialMappingObserverObject;
    private void Start()
    {
        currentPosition = Camera.main.transform.position;
        previousPosition = currentPosition;
        // Instantiate a capsule at the destination vector and create a point of collision
        Instantiate(destinationPrefab, destinationVector, Camera.main.transform.rotation);
        //point = (GameObject)Instantiate(pointPrefab, destPoint, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {

        if (spatialMappingObserverObject != null)
        {
            currentPosition = Camera.main.transform.position;
            //Debug.Log("Current position is: " + currentPosition);
        }
        // only draw line if userposition has changed
        if (currentPosition != previousPosition)
        {
            previousPosition = currentPosition;
            DrawLine();
        }

        else //Debug.Log("User position has not changed");
            Debug.Log("");
    }
    private void CalculateDistance()
    {
        //centerPos = destinationVector; // this works with point in the midpoint of the line
        centerPos = destinationVector;
        distance = Vector3.Distance(currentPosition, centerPos);
        Debug.Log("Centerpos is: " + centerPos);
        Debug.Log("Current distance to destination is: " + distance);
    }
    private void DrawLine()
    {
        CalculateDistance();
      
        //get direction between source and destination vectors
        Vector3 direction = centerPos - currentPosition;
        Debug.Log("Current direction to destination is: " + direction);
        // first clear all lines
        ClearLines();
        GameObject line = (GameObject)Instantiate(linePrefab, centerPos, Quaternion.LookRotation(direction));
        Debug.Log("Current distance to destination is: " + distance);
        
        line.transform.localScale = new Vector3(distance, 0.005f, 0.005f);
        line.transform.Rotate(Vector3.down, 90f);

        GameObject root = new GameObject();
        line.transform.parent = root.transform;

        Lines.Push(new Line
        {
            Start = currentPosition,
            End = destinationVector,
            Root = root,
            Distance = distance
        });
    }
    private void ClearLines()
    {
        if (Lines != null && Lines.Count > 0)
        {
            while (Lines.Count > 0)
            {
                Line lastLine = Lines.Pop();
                Destroy(lastLine.Root);
            }
        }
    }

    public struct Line
    {
        public Vector3 Start { get; set; }

        public Vector3 End { get; set; }

        public GameObject Root { get; set; }

        public float Distance { get; set; }
    }
   
}
