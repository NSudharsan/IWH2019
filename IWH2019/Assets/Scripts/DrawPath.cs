using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPath : MonoBehaviour
{
    public List<Vector3> PointVectors = new List<Vector3>();
    public JSONReader jsonReader;
    int numberofPointsInPath = 0;
    public GameObject pointPrefab;
    public GameObject linePrefab;

    public GameObject movableObject;
    
    private Vector3 currentPoint;
    private Vector3 nextPoint;
    bool isAlternatePath;

    // save all lines in scene
    private Stack<Line> Path = new Stack<Line>();
    void Start()
    {
        PointVectors = JSONData();
        CreatePointsAndLines(false, PointVectors[0]);
       
    }


    public void CreatePointsAndLines(bool isAlternatePath, Vector3 startPoint)
    {

        if (isAlternatePath)
        {
            //destroy any previous paths
            GameObject[] points = GameObject.FindGameObjectsWithTag("Point");
            GameObject[] lines = GameObject.FindGameObjectsWithTag("Line");
            Debug.Log("points size is:" + points.Length+ "Lines size is: "+lines.Length);
            foreach (GameObject p in points)
            {
                Destroy(p);
            }
            foreach (GameObject l in lines)
            {
                Destroy(l);
            }

            PointVectors = AlternateJSONData(startPoint);

        }
        // TODO future: JSON reading
        //PointVectors = jsonReader.ReturnVectorList();
      
        if (PointVectors != null)
        {
            // get List count
            numberofPointsInPath = PointVectors.Count;
            Debug.Log("numberofPointsInPath: "+numberofPointsInPath);
            int i = 0;
            int j = 0;
            foreach (Vector3 pv in PointVectors)
            {
                //instantiate points at each of the vectors returned
                GameObject point = Instantiate(pointPrefab, pv, Camera.main.transform.rotation);
                point.gameObject.tag = "Point";

                while (i < numberofPointsInPath-1)
                {
                    //Debug.Log("i is: " + i);
                    //Debug.Log("pv is: " + pv);
                    // Original Path visualization: draw lines between each point in sequential order. This will represent the path
                    currentPoint = PointVectors[i];
                    nextPoint = PointVectors[i+1];
                    //Debug.Log("currentPoint: " + currentPoint);
                    //Debug.Log("nextPoint: " + nextPoint);
                    //instantiate wall at each current point
                    Vector3 temp = transform.rotation.eulerAngles;
                    temp.y = 90.0f;
                    //wallObject = Instantiate(WallPrefab, currentPoint, Quaternion.Euler(temp));
                    //walls.Add(wallObject);
                    //walls[i].name = "Wall " + i;
                    float distance = Vector3.Distance(currentPoint, nextPoint);
                    //Debug.Log("Distance is: " + distance);
                    Vector3 direction = nextPoint - currentPoint;
                    //Debug.Log("Direction is: " + direction);
                    Vector3 centerPos = (currentPoint + nextPoint) * 0.5f;
                    GameObject line = (GameObject)Instantiate(linePrefab, centerPos, Quaternion.LookRotation(direction));
                    line.gameObject.tag = "Line";
                    line.transform.localScale = new Vector3(distance, 0.005f, 0.005f);
                    line.transform.Rotate(Vector3.down, 90f);

                    i++;
                }
               
            }
            Debug.Log("Done creating path");
            
        }
        else
        {

            Debug.Log(" Path is null");
        }

        // position the movable object at the first point
        movableObject.transform.position = PointVectors[0];
        //Debug.Log("Movable Object's position is now at the point: " + movableObject.transform.position);
    }
   
    public List<Vector3> JSONData()
    {
        // TODO: future, can be read from a backend service providing the path
        List<Vector3> returnList = new List<Vector3>();
        returnList.Add(new Vector3(0.7f, -0.7f, -7.3f));
        //returnList.Add(new Vector3(0.5f, -0.6f, -5.6f));
        returnList.Add(new Vector3(0.6f, -0.7f, -3.9f));
        returnList.Add(new Vector3(2.1f, -0.7f, -4.2f));
        
        return returnList;
    }
    public List<Vector3> AlternateJSONData(Vector3 fromPoint)
    {
        // TODO: future, can be read from a backend service providing the alternate path for a point
        List<Vector3> returnListAlternate = new List<Vector3>();
        returnListAlternate.Add(new Vector3(fromPoint.x, fromPoint.y, fromPoint.z));
        returnListAlternate.Add(new Vector3(2.1f, -0.7f, -5.7f));
        returnListAlternate.Add(new Vector3(2.1f, -0.7f, -4.2f));
        return returnListAlternate;
    }
    public struct Line
    {
        public Vector3 Start { get; set; }

        public Vector3 End { get; set; }

        public GameObject Root { get; set; }

        public float Distance { get; set; }
    }
}
