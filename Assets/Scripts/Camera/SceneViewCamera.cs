using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]

public class SceneViewCamera : MonoBehaviour
{
    public float targetOrtho;
    // public float targetOrtho;
    public float smoothSpeed = 2.0f;
    public float minOrtho = 1.0f;
    public float maxOrtho = 20.0f;
    public Camera camera0;
    public Camera camera1;
    public Camera camera2;

    [SerializeField, Range(0.1f, 10f)]
    private float zoomSpeed = 1f;
    // private float wheelSpeed = 1f;

    [SerializeField, Range(0.1f, 10f)]
    private float moveSpeed = 0.3f;

    [SerializeField, Range(0.1f, 10f)]
    // private float rotateSpeed = 0.3f;

    private Vector3 preMousePos;
    private float scrollWheel = 0;
    private float cameraX = 0;
    private float cameraY = 0;
    private GameObject AlienPinkObject;
    private PinkCharacter AlienPinkScript;
    private bool isMovingCenter = false;
    private float moveCenterSpeed = 10;


    private void Start(){
        // targetOrtho1 = camera1.orthographicSize;
        // targetOrtho2 = camera2.orthographicSize;
        AlienPinkObject = GameObject.Find("alienPink(Clone)");
        AlienPinkScript = AlienPinkObject.GetComponent<PinkCharacter>();
    }
    private void Update()
    {
        // Debug.Log(AlienPinkObject.gameObject.transform.position.x);
        // Debug.Log(AlienPinkScript.isSimulating);
        if(isMovingCenter){
            MoveCameraCenter();
        }else if(!AlienPinkScript.isSimulating){
            MouseUpdate();
            return;
        }else{ //isSimulating == true || isMovingCenter == false の場合 (シミュレーション中の場合)
            MouseWheel();
            // SetCameraPos(AlienPinkObject.gameObject.transform.position.x, AlienPinkObject.gameObject.transform.position.y);
            TrackCharackter();
        }
    }

    private void MouseUpdate()
    {
            // MouseWheel(scrollWheel);
        MouseWheel();

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)){ // || Input.GetMouseButtonDown(2))
            preMousePos = Input.mousePosition;
        }

        MouseDrag(Input.mousePosition);
    }

    private void MouseWheel(){
        scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheel != 0.0f){
            targetOrtho -= scrollWheel * zoomSpeed;
            targetOrtho = Mathf.Clamp (targetOrtho, minOrtho, maxOrtho);
            camera0.orthographicSize = targetOrtho;
            camera1.orthographicSize = targetOrtho;
            camera2.orthographicSize = targetOrtho;
            // camera1.orthographicSize = Mathf.MoveTowards (camera1.orthographicSize, targetOrtho, smoothSpeed * Time.deltaTime);
            // camera2.orthographicSize = Mathf.MoveTowards (camera2.orthographicSize, targetOrtho, smoothSpeed * Time.deltaTime);
        }
    }
    private void MouseDrag(Vector3 mousePos)
    {
        Vector3 diff = mousePos - preMousePos;

        if (diff.magnitude < Vector3.kEpsilon){
            return;
        }

        // cameraX = camera1.gameObject.transform.position.x;
        // cameraY = camera1.gameObject.transform.position.y;

        if (Input.GetMouseButton(2)){
            // Debug.Log(cameraX);
            // Debug.Log(cameraY);
            // if(cameraX){

            // }
            // if( -3f <= cameraX && cameraX <= 5.5f && -6f <= cameraY && cameraY <= 1.2f ){
                // Debug.Log("はいった");
                Vector3 resultPos = camera1.gameObject.transform.position - diff * Time.deltaTime * moveSpeed;
                if(IsRange(resultPos.x, -6f, 7f) && IsRange(resultPos.y, -4f, 6f)){
                    camera0.gameObject.transform.Translate(-diff * Time.deltaTime * moveSpeed);
                    camera1.gameObject.transform.Translate(-diff * Time.deltaTime * moveSpeed);
                    camera2.gameObject.transform.Translate(-diff * Time.deltaTime * moveSpeed);
                }
            // }else if(cameraX < -3f){
            //     Debug.Log(cameraX);
            //     camera1.gameObject.transform.position = new Vector3(-2.9f, cameraY, camera1.gameObject.transform.position.z);
            //     Debug.Log(cameraX);
            // }else if(cameraX > 5.5f){
            //     cameraX = 5.5f;
            // }else if(cameraY < -6f){
            //     cameraY = -6f;
            // }else if(cameraY > 1.2f){
            //     cameraY = 1.2f;
            // }
        }
        // else if (Input.GetMouseButton(1))
        //     CameraRotate(new Vector2(-diff.y, diff.x) * rotateSpeed);

        preMousePos = mousePos;
    }
    private bool IsRange(float Target, float Min, float Max){
        float Result = Mathf.Clamp(Target, Min, Max);
        if (Result == Target) return true;
        return false;
    }

    private void TrackCharackter(){ //重くなったr後で書き換える
        Vector3 currentCameraPos = camera0.gameObject.transform.position;
        currentCameraPos.x = Mathf.Lerp(currentCameraPos.x, AlienPinkObject.gameObject.transform.position.x, Time.deltaTime * moveCenterSpeed);
        currentCameraPos.y = Mathf.Lerp(currentCameraPos.y, AlienPinkObject.gameObject.transform.position.y, Time.deltaTime * moveCenterSpeed);
        SetCameraPos(currentCameraPos.x, currentCameraPos.y);
    }
    private void MoveCameraCenter(){
        // SetCameraPos(-0.026f, 1.015f);

        Vector3 currentCameraPos = camera0.gameObject.transform.position;
        currentCameraPos.x = Mathf.Lerp(currentCameraPos.x, -0.026f, Time.deltaTime * moveCenterSpeed);
        currentCameraPos.y = Mathf.Lerp(currentCameraPos.y, 1.015f, Time.deltaTime * moveCenterSpeed);
        // targetObj.position = currentCameraPos;
        SetCameraPos(currentCameraPos.x, currentCameraPos.y);
        SetCameraSize(Mathf.Lerp(camera1.orthographicSize, 6f, Time.deltaTime * moveCenterSpeed));

    }

    private void isMovingDisabler(){
        isMovingCenter = false;
    }
    public void isMovingEnabler(){ //PinkCharacterで参照される
        isMovingCenter = true;
        Invoke("isMovingDisabler", 0.5f);
    }

    private void SetCameraPos( float x, float y ){
        camera0.gameObject.transform.position = new Vector3 (x, y, camera0.gameObject.transform.position.z);
        camera1.gameObject.transform.position = new Vector3 (x, y, camera1.gameObject.transform.position.z);
        camera2.gameObject.transform.position = new Vector3 (x, y, camera2.gameObject.transform.position.z);
    }
    private void SetCameraSize(float targetOrtho){
        camera0.orthographicSize = targetOrtho;
        camera1.orthographicSize = targetOrtho;
        camera2.orthographicSize = targetOrtho;
    }
    // public void CameraRotate(Vector2 angle)
    // {
    //     transform.RotateAround(transform.position, transform.right, angle.x);
    //     transform.RotateAround(transform.position, Vector3.up, angle.y);
    // }
}
