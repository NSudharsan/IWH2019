using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * This code has been adapted from: Mixed Reality Examples and 
 * https://medium.com/southworks/how-to-use-spatial-understanding-to-query-your-room-with-hololens-4a6192831a6f
 * It has been modified to fit this project by Nischita Sudharsan (https://codeholo.com)
 */

public class RoomScanner : MonoBehaviour, IInputClickHandler
{
    public TextMesh visualInfo;
    private SpatialUnderstandingDll.Imports.RaycastResult rayCastResult;

    // Start is called before the first frame update
    void Start()
    {
        // start the scanning from spatial understanding
        InputManager.Instance.PushFallbackInputHandler(this.gameObject);
        SpatialUnderstanding.Instance.RequestBeginScanning();
        SpatialUnderstanding.Instance.ScanStateChanged += ScanStateChanged;

    }
    public void OnInputClicked(InputClickedEventData eventData)
    {
        this.visualInfo.text = "Requested Finish Scan...";
        SpatialUnderstanding.Instance.RequestFinishScan();
    }
    private void ScanStateChanged()
    {
        if (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Scanning)
        {
            LogSurfaceState();
        }
        // is scanning done? then start categorizing the surfaces
        else if (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Done)
        {
           IdentifyFloor();
        }
    }
    private void Update()
    {
        //Inthe Update method, check the Scan State.If the user hasn’t 
        //  decided to finish the scanning, you can display some metrics about the scanned
        //surfaces to show some progress to the user like shown in the following code:

        switch (SpatialUnderstanding.Instance.ScanState)
        {
            case SpatialUnderstanding.ScanStates.None:
            case SpatialUnderstanding.ScanStates.ReadyToScan:
                break;
            case SpatialUnderstanding.ScanStates.Scanning:
                this.LogSurfaceState();
                break;
            case SpatialUnderstanding.ScanStates.Finishing:
                this.visualInfo.text = "Finishing Scan...";
                break;
            case SpatialUnderstanding.ScanStates.Done:
                this.visualInfo.text = "Scan Finished";
                break;
            default:
                break;
        }
    }

    private void LogSurfaceState()
    {
        IntPtr statsPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStatsPtr();
        if (SpatialUnderstandingDll.Imports.QueryPlayspaceStats(statsPtr) != 0)
        {
            var stats = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStats();
            this.visualInfo.text = string.Format("TotalSurfaceArea: {0:0.##}\nWallSurfaceArea: {1:0.##}\nHorizSurfaceArea: {2:0.##}\nTap to initiate finish scan", 
                stats.TotalSurfaceArea, stats.WallSurfaceArea, stats.HorizSurfaceArea);
            // TODO: add visual command: tap to finish scanning
            // Assumption: someone else who is not visually impaired does the pre-scanning
        }
    }
    private void OnDestroy()
    {
        SpatialUnderstanding.Instance.ScanStateChanged -= ScanStateChanged;
    }
    //Show some visual signs of the floor
    private void IdentifyFloor()
    {
        this.visualInfo.text = "Identifying floor...";
       


    }
}
