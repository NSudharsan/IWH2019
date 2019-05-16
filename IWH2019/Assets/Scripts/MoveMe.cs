using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMe : MonoBehaviour
{

    private List<Vector3> targetObjects;

    private Vector3 endPosition;
    public float speed = 1.0f;
    private float startTime;
    private float journeyLength;
    int arraySize;
    private int currentPoint;

    public DrawPath drawPathObject;
    public JSONReader jsonReader;

    public const float RayCastLength = 10.0f;
    private SpatialUnderstandingDll.Imports.RaycastResult rayCastResult;

    public TextMesh visualInfo;
    private bool lerpChecker;
    public InstructionManager instructionManager;
    int counter = 1;
    public Camera userCamera;
    bool spoken;

    private void Awake()
    {

        startTime = Time.time;
        // modified for HoloLens
        targetObjects = drawPathObject.JSONData();
        //targetObjects=jsonReader.ReturnVectorList();
        arraySize = targetObjects.Count;
        //Debug.Log("transform.position " + transform.position);
        //Debug.Log("arraysize is: " + arraySize);
        foreach (Vector3 to in targetObjects)
        {
            Debug.Log("Targetobject is: " + to);
        }

    }
    private void Update()
    {
        // start checking only if spatial scanning is done
        if (SpatialUnderstanding.Instance.AllowSpatialUnderstanding &&
              SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Done)
        {
            RaycastHit hitCamera;
            Physics.Raycast(userCamera.transform.position, Vector3.down, out hitCamera, 10);
            if (counter < arraySize)
            {
                // is user facing next point in path?
                Vector3 targetDir = targetObjects[counter] - userCamera.transform.position;
                Vector3 camforward = userCamera.transform.forward;
                float angle = Vector3.Angle(targetDir, camforward);

                if (hitCamera.collider != null)
                {
                    Debug.Log("hit object is: " + hitCamera.collider.gameObject.name);
                    if (hitCamera.collider.gameObject.name == "MovableObject" && spoken == false)
                    {
                        instructionManager.GiveInstructions("Stop, Turn around slowly");
                        spoken = true;
                    }
                    if (hitCamera.collider.gameObject.name == "MovableObject" && angle <= 20)
                    {
                        enabled = false;
                        Debug.Log("counter in update if check is: " + counter);
                        CheckRayCast(targetObjects[counter]);
                    }
                }
            }
            else
            {

                if (hitCamera.collider.gameObject.name == "MovableObject")
                {
                    instructionManager.GiveInstructions("You have reached your destination");
                    enabled = false;
                }

            }
        }
    }

public void RayCastCheckCaller()
    {
        CheckRayCast(targetObjects[1]);

    }
    public void CheckRayCast(Vector3 targetObjectIndex)
    {
            Debug.Log("Checking raycast for targetObjectIndex..." + targetObjectIndex);
            // Uncomment for HoloLens
       
             //use for object raycasts
            Vector3 rayPos = transform.position;
            //Vector3 hitdirection = this.transform.TransformDirection(Vector3.forward);
            Vector3 hitdirection = targetObjectIndex - rayPos;

            //Debug.Log("cube position: " + transform.position);
            //Debug.Log("path direction: " + hitdirection);
            // if three is a raycast hit, then do the checks
            RaycastHit hit;
            if (Physics.Raycast(rayPos, hitdirection, out hit, 10))
            {
               // instantiate object where raycast hit an obstacle
                GameObject capsule = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Capsule), hit.point, transform.rotation);
                capsule.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                
                IntPtr raycastResultPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticRaycastResultPtr();
                Debug.Log("raycastResultPtr is: " + raycastResultPtr);

                // Uncomment for HoloLens
                SpatialUnderstandingDll.Imports.PlayspaceRaycast(
                    rayPos.x, rayPos.y, rayPos.z,
                    hitdirection.x, hitdirection.y, hitdirection.z,
                    raycastResultPtr);
                rayCastResult = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticRaycastResult();
                //Debug.Log("Raycast result is: " + rayCastResult.SurfaceType);

                // calculate distance between movable object and point where raycast hit
                float distance = Vector3.Distance(transform.position, capsule.transform.position);
                Debug.Log("distance is: " + distance);

                visualInfo.text = "Next obstacle at: " + distance + " metres";

                // call move logic whether to move the movable object or not
                MoveLogic(distance, targetObjectIndex);

            }
            // if there are no raycasts hit, then  no obstacles, move the object
            else
            {
                MoveLogic(0, targetObjectIndex);
            }
        
        Debug.Log("Finished checking raycast");
    }

    private void MoveLogic(float obstacleDistance, Vector3 temporaryTarget)
    {
        
        // checking if and how much movable object can move
        // assumption: always move forward - TODO add logic for user to stay on path
        // if distance between raycast point and movable object >= distance between movable object and next point in path
        // then move movable object to next point
        //Debug.Log("obstacle distance is: " + obstacleDistance);
        Debug.Log("temporary target is: " + temporaryTarget);
        //Debug.Log("this transform pos is null? " + transform.position);
        //Debug.Log("point distance is: " + Vector3.Distance(transform.position, targetObjects[1]));

        if (obstacleDistance == 0 || obstacleDistance >= Vector3.Distance(this.transform.position, temporaryTarget))
        {
            Debug.Log("distance is more than the next point, can travel");
            // move the movable object
            // Distance moved = time * speed.
            float distCovered = (Time.time - startTime) * speed;

            // Fraction of journey completed = current distance divided by total distance.
            float fracJourney = distCovered / journeyLength;

            //Debug.Log("distcovered and fracjourney are: " + distCovered + " " + fracJourney);
            // Set our position as a fraction of the distance between the markers.
            //transform.position = Vector3.Lerp(startPosition, endPosition, fracJourney);
            transform.position = Vector3.Lerp(transform.position, temporaryTarget, fracJourney);
            //give audio instructions to the user
            instructionManager.GiveInstructions("Ok, Walk forward");
            counter++;
            Debug.Log("counter in raycast is: " + counter);
            spoken = false;
            enabled = true;

        }
        // else if distance between raycast point and movable object < distance between movable object and next point in path
        // then ask for alternate path at this point
        else
        {
            Debug.Log("distance is less than the next point, cannot travel, ask for alternate path");
            instructionManager.GiveInstructions("Obstacle in the path. Retrieved alternate path");
            // ask for alternate path
            Vector3 startAlternatePathPoint = targetObjects[counter-1];
            Debug.Log("StartalternatePath from is: " + startAlternatePathPoint);
            // destroy previous path and draw new path
            drawPathObject.CreatePointsAndLines(true, startAlternatePathPoint);
            //spoken = false;
            //enabled = true;
        }

       
    }
  

}

