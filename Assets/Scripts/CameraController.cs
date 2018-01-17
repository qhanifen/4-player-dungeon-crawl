using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    #region Camera Variables
    public int xCameraBuff = 10;
    public int yCameraBuff = 7;
    public float minZDistance = 25f;
    public float maxZDistance = 50f;
    public float cameraPanSpeed = 7.0f;
    public float cameraZoomSpeed = 5.0f;
    public float cameraDampTime = .2f;
    public float cameraAngle = 55f;
    #endregion

    public List<Transform> roomFocalPoints;
    public Transform focusedRoom;
    public List<Transform> trackedObjects;
    public bool trackedObjectsInView;
    public Rect cameraRect;
    public Rect objectBounds;

    Camera cam;
    public Vector3 focalVector;
    public Vector3 calculatedCenter;
    public Vector3 cameraAngleVector;
    public float cameraZoom;
    float cameraDistance = 25f;
    float minBoundsDelta = 25f;
    float maxBoundsDelta = 150f;

    public void Start()
    {
        cam = GetComponent<Camera>();
        /*foreach(Hero hero in GameManager.instance.heroes)
        {
            trackedObjects.Add(hero.gameObject);
        }*/
        cameraRect = new Rect(xCameraBuff, yCameraBuff, Screen.width - (xCameraBuff * 2), Screen.height - (yCameraBuff * 2));

        //Initialize camera position
        SetCameraPosition();        
    }

    [ContextMenu("Reset Camera Position")]
    void SetCameraPosition()
    {
        focalVector = Vector3.zero;
        cameraAngleVector = Quaternion.Euler(90f - cameraAngle, 0, 0) * Vector3.back;
        cameraDistance = minZDistance;
        transform.position = Vector3.zero + -transform.forward * minZDistance;
        cameraAngleVector = Quaternion.Euler(90f - cameraAngle, 0, 0) * Vector3.back;
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        cameraRect = new Rect(xCameraBuff, yCameraBuff, Screen.width - (xCameraBuff * 2), Screen.height - (yCameraBuff * 2));
        Texture2D tex = new Texture2D(1, 1);
        Texture2D objectTex = new Texture2D(1, 1);
        Color color = trackedObjectsInView ? Color.green : Color.red;
        Color objectBoundsColor = Color.blue;
        tex.SetPixel(0, 0, color);
        tex.Apply();
        objectTex.SetPixel(0, 0, objectBoundsColor);
        objectTex.Apply();
        float thickness = 2;
        //Top line
        GUI.DrawTexture(new Rect(xCameraBuff, yCameraBuff, Screen.width - (2 * xCameraBuff), thickness), tex);
        GUI.DrawTexture(new Rect(objectBounds.xMin, objectBounds.yMin, objectBounds.xMax - objectBounds.xMin, thickness), objectTex);
        //Bottom line
        GUI.DrawTexture(new Rect(xCameraBuff, Screen.height - yCameraBuff, Screen.width - (2 * xCameraBuff), thickness), tex);
        GUI.DrawTexture(new Rect(objectBounds.xMin, objectBounds.yMax, objectBounds.xMax - objectBounds.xMin, thickness), objectTex);
        //Left line
        GUI.DrawTexture(new Rect(xCameraBuff, yCameraBuff, thickness, Screen.height - (2 * yCameraBuff)), tex);
        GUI.DrawTexture(new Rect(objectBounds.xMin, objectBounds.yMin, thickness, objectBounds.yMax - objectBounds.yMin), objectTex);
        //Right line
        GUI.DrawTexture(new Rect(Screen.width - xCameraBuff, yCameraBuff, thickness, Screen.height - (2 * yCameraBuff)), tex);
        GUI.DrawTexture(new Rect(objectBounds.xMax, objectBounds.yMin, thickness, objectBounds.yMax - objectBounds.yMin), objectTex);

        //Drawn center
        Vector3 screenPos = cam.WorldToScreenPoint(calculatedCenter);
        GUI.DrawTexture(new Rect(new Vector2(screenPos.x, Screen.height - screenPos.y), new Vector2(5,5)), tex);

        Debug.DrawLine(transform.position, focalVector, color);
        
        //Draw game objects
        color = Color.blue;
        tex.SetPixel(0, 0, color);
        tex.Apply();
        for (int i = 0; i < trackedObjects.Count; i++)
        {
            screenPos = cam.WorldToScreenPoint(trackedObjects[i].position);
            GUI.DrawTexture(new Rect(new Vector2(screenPos.x, Screen.height - screenPos.y), new Vector2(5, 5)), tex);
        }
    }
#endif

    void FixedUpdate()
    {
        CheckClosestRoom();
        AdjustCameraView();
    }

    void CheckClosestRoom()
    {

    }

    void TrackNewObjects()
    {
        
    }

    void AdjustCameraView()
    {
        //Find min/max points on screen
        calculatedCenter = new Vector3();
        objectBounds = new Rect(cam.pixelRect.center, Vector2.zero);
        bool objectsOnScreen = true;
        for(int i=0; i < trackedObjects.Count; i++)
        {
            Vector3 pos = trackedObjects[i].transform.position;          
            Vector2 screenPoint = cam.WorldToScreenPoint(pos);
            calculatedCenter += pos;

            if(screenPoint.x < objectBounds.xMin)
            {
                objectBounds.xMin = screenPoint.x;
            }
            if (screenPoint.x > objectBounds.xMax)
            {
                objectBounds.xMax = screenPoint.x;
            }
            if (Screen.height - screenPoint.y < objectBounds.yMin)
            {
                objectBounds.yMin = Screen.height - screenPoint.y;
            }
            if (Screen.height - screenPoint.y > objectBounds.yMax)
            {
                objectBounds.yMax = Screen.height - screenPoint.y;
            }

            if(objectsOnScreen)
            {
                if(!cameraRect.Contains(screenPoint))
                {
                    objectsOnScreen = false;
                }
            }
        }
        trackedObjectsInView = objectsOnScreen;
        calculatedCenter /= trackedObjects.Count;
        calculatedCenter.y = 0;
        Vector3 deltaVector = (calculatedCenter - focalVector);

        //Vector3 cameraCenter = Physics.Raycast(cam.ScreenPointToRay(screenCenter);

        MoveCamera(deltaVector);        
    }
        
    void MoveCamera(Vector3 deltaVector)
    {
        //To do: Add Lerped Movement Speed
        //To do: Add camera Zoom In/Out
        //To do: fix zooming error
        focalVector += deltaVector * cameraPanSpeed * Time.deltaTime;

        //cameraZoom = trackedObjectsInView ? -1 : 1;

        float deltaX = Mathf.Abs(objectBounds.x - cameraRect.x);
        float deltaY = Mathf.Abs(objectBounds.y - cameraRect.y);

        if (deltaX > maxBoundsDelta || deltaY > maxBoundsDelta)
        {
            cameraZoom = 1;
        }
        else if(deltaX < minBoundsDelta || deltaY < minBoundsDelta)
        {
            cameraZoom = 0;
        }
        else
        {
            cameraZoom = Mathf.InverseLerp(0, maxBoundsDelta, Mathf.Min(deltaX, deltaY));
        }
        float cameraDesired = Mathf.Lerp(minZDistance, maxZDistance, cameraZoom);
        cameraDistance = Mathf.SmoothDamp(cameraDistance, cameraDesired, ref cameraZoomSpeed, cameraDampTime);
        transform.position = focalVector + (cameraAngleVector * cameraDistance);

        //Testing purposes, remove if necessary
        transform.LookAt(calculatedCenter);
    }
}
