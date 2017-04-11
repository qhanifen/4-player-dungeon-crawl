using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour {

    #region Camera Variables
    public int xCameraBuff = 10;
    public int yCameraBuff = 7;
    public float minZDistance = 25f;
    public float maxZDistance = 50f;
    public float cameraPanSpeed = 7.0f;
    public float cameraZoomSpeed = 5.0f;
    #endregion

    public List<Transform> roomFocalPoints;
    public Transform focusedRoom;
    public List<GameObject> trackedObjects;
    public bool trackedObjectsInView;
    public Rect cameraRect;

    Camera cam;
    public Vector3 focalVector;
    public Vector3 calculatedCenter;

    public void Start()
    {
        cam = GetComponent<Camera>();
        /*foreach(Hero hero in GameManager.instance.heroes)
        {
            trackedObjects.Add(hero.gameObject);
        }*/

        cameraRect = new Rect(xCameraBuff, yCameraBuff, Screen.width - (xCameraBuff * 2), Screen.height - (yCameraBuff * 2));
        focalVector = Vector3.zero;
        transform.position = Vector3.zero + -transform.forward * minZDistance;
    }

    [ContextMenu("Reset Camera Position")]
    void ResetCameraPosition()
    {
        focalVector = Vector3.zero;
        transform.position = Vector3.zero + -transform.forward * minZDistance;
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        cameraRect = new Rect(xCameraBuff, yCameraBuff, Screen.width - (xCameraBuff * 2), Screen.height - (yCameraBuff * 2));
        Texture2D tex = new Texture2D(1, 1);
        Color color = trackedObjectsInView ? Color.green : Color.red;
        tex.SetPixel(0, 0, color);
        tex.Apply();        
        float thickness = 2;
        //Top line
        GUI.DrawTexture(new Rect(xCameraBuff, yCameraBuff, Screen.width - (2 * xCameraBuff), thickness), tex);
        //Bottom line
        GUI.DrawTexture(new Rect(xCameraBuff, Screen.height - yCameraBuff, Screen.width - (2 * xCameraBuff), thickness), tex);
        //Left line
        GUI.DrawTexture(new Rect(xCameraBuff, yCameraBuff, thickness, Screen.height - (2 * xCameraBuff)), tex);
        //Right line
        GUI.DrawTexture(new Rect(Screen.width - xCameraBuff, yCameraBuff, thickness, Screen.height - (2 * xCameraBuff)), tex);

        //Drawn center
        Vector3 screenPos = cam.WorldToScreenPoint(calculatedCenter);
        GUI.DrawTexture(new Rect(new Vector2(screenPos.x, Screen.height - screenPos.y), new Vector2(5,5)), tex);

        //Draw game objects
        color = Color.blue;
        tex.SetPixel(0, 0, color);
        tex.Apply();
        foreach (GameObject obj in trackedObjects)
        {
            screenPos = cam.WorldToScreenPoint(obj.transform.position);
            GUI.DrawTexture(new Rect(new Vector2(screenPos.x, Screen.height - screenPos.y), new Vector2(5, 5)), tex);
        }
    }
#endif

    void Update()
    {
        CheckClosestRoom();
        TrackNewObjects();
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
        float minX = 0, minZ = 0, maxX = 0, maxZ = 0;
        bool objectsOnScreen = true;
        for(int i=0; i < trackedObjects.Count; i++)
        {   
            Vector3 pos = trackedObjects[i].transform.position;
            if(i == 0)
            {
                minX = pos.x;
                maxX = pos.x;
                minZ = pos.z;
                maxZ = pos.z;
            }
            if(pos.x < minX)
            {
                minX = pos.x;
            }
            if (pos.x > maxX)
            {
                maxX = pos.x;
            }
            if (pos.z < minZ)
            {
                minZ = pos.z;
            }
            if (pos.z > maxZ)
            {
                maxZ = pos.z;
            }

            Vector3 screenPoint = cam.WorldToScreenPoint(pos);
            if(objectsOnScreen)
            {
                if(!cameraRect.Contains(screenPoint))
                {
                    objectsOnScreen = false;
                }
            }
        }
        trackedObjectsInView = objectsOnScreen;
        Vector3 lastPosition = calculatedCenter;
        calculatedCenter = new Vector3((minX + (maxX - minX)/2), 0, (minZ + (maxZ - minZ)/2));

        //Vector3 cameraCenter = Physics.Raycast(cam.ScreenPointToRay(screenCenter);

        MoveCamera();        
    }

    void MoveCamera()
    {
        Vector3 deltaVector = calculatedCenter - focalVector;

        /*if ()
        {

        }
        */
        //transform.Translate(deltaVector * cameraPanSpeed * Time.deltaTime);
        if (!trackedObjectsInView)
        {
            transform.Translate(transform.forward.normalized * -cameraZoomSpeed * Time.deltaTime, Space.Self);
        }
        else
        {
            transform.Translate(transform.forward * cameraZoomSpeed * Time.deltaTime, Space.Self);
        }
        //Vector3.Lerp(cam.transform.position, dir, )
    }
}
