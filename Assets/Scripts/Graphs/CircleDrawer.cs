using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleDrawer : MonoBehaviour
{
    public GameObject CirclePrefabs;

    public float LineWidth;
    // public Gradient LineColor;
    public Gradient PreviewLineColor;
    public Gradient StaticLineColor;
    public Gradient DynamicLineColor;
    public GameObject InputCenterXObject;
    public GameObject InputCenterYObject;
    public GameObject InputRadiusObject;
    public Text rangeErrorText;
    public GameObject GraphDrawerObject;
    public GameObject startSimulationButton;

    InputField CenterXField;
    InputField CenterYField;
    InputField RadiusField;

    Circle currentCircleScript;
    private GameObject currentCircleObject;
    private bool isPhysicsOn = false;
    private bool isCircleTabOn = false;
    private bool isXPlus = false;
    private bool isYPlus = false;
    private float centerX = 0;
    private float centerY = 0;
    private float radius = 2;
    private GraphList graphListScript;

    public void Draw()
    {
        DeletePreview();

        // Debug.Log("ドロー！");
        // Debug.Log(isCircleTabOn);
        if(isCircleTabOn){
            DrawCircle(new Vector3(centerX, centerY, 0), radius);
            // DrawCircle(new Vector3(centerX, centerY, 0), radius);
            currentCircleObject.tag = "Preview"; //onSimulateと区別
        }
    }

    private void Start()
    {
        CenterXField = InputCenterXObject.gameObject.GetComponent<InputField>();
        CenterYField = InputCenterYObject.gameObject.GetComponent<InputField>();
        RadiusField = InputRadiusObject.gameObject.GetComponent<InputField>();
        graphListScript = GraphDrawerObject.GetComponent<GraphList>();
        CenterXField.text = "0";
        CenterYField.text = "0";
        RadiusField.text = "2";
    }

    private void DrawCircle(Vector3 centerPoint, float radius)
    {
        if(radius < 21){
            InstantiateCircle();

            float angle = 2 * Mathf.PI / 30;

            for (int i = 0; i < 30; i++)
            {
                Matrix4x4 rotationMatrix = new Matrix4x4(
                                                        new Vector4(Mathf.Cos(angle * i), Mathf.Sin(angle * i), 0, 0),
                                                        new Vector4(-1 * Mathf.Sin(angle * i), Mathf.Cos(angle * i), 0, 0),
                                                        new Vector4(0, 0, 1, 0),
                                                        new Vector4(0, 0, 0, 1)
                                                        );
                Vector3 initialRelativePosition = new Vector3(0, radius, 0);
                currentCircleScript.AddPoint(centerPoint + rotationMatrix.MultiplyPoint(initialRelativePosition));
            }
        }else{
            rangeErrorText.text = "半径が大きすぎるよ！";
        }
    }

    public void physicsSwitch(){
        isPhysicsOn = !isPhysicsOn;
        // Debug.Log($"{isPhysicsOn}が現在の設定");
    }

    public void CircleTabOn(){
        isCircleTabOn = !isCircleTabOn;
        // Debug.Log($"{isCircleTabOn}が今のタブ状況");
    }

    public void DeleteAllCircles(){
        foreach(Transform n in gameObject.transform){
            GameObject.Destroy(n.gameObject);
        }
        // Draw();
    }

    public void SwitchX(){
        isXPlus = !isXPlus;
        centerX = (-1f)*centerX;
        Draw();
    }

    public void SwitchY(){
        isYPlus = !isYPlus;
        centerY = (-1f)*centerY;
        Draw();
    }


    public void StartSimulation(){
        // if(inputField.text.Length > 0){
            if(currentCircleObject){
                if(isPhysicsOn){
                    currentCircleObject.tag = "DynamicCircle";
                    currentCircleScript.SetLineColor(DynamicLineColor);
                }else{
                    currentCircleObject.tag = "onSimulate";
                    currentCircleScript.SetLineColor(StaticLineColor);
                }
                currentCircleScript.onCollider();
                string circleFunc = GetCurrentCircleFunction();
                graphListScript.AddCirclePanel(circleFunc, centerX, centerY, radius, isXPlus, isYPlus);
                currentCircleObject.name = centerX.ToString() + centerY.ToString() + radius.ToString() + isXPlus.ToString() + isYPlus.ToString();
                // if(isPhysicsOn){
                //     currentCircleScript.onPhysic();
                //     Draw();
                // }else{
                //     currentCircleScript.onCollider();
                // }
            }
        // }else{
        //     rangeErrorText.text = "式を入力しよう！";
        // }
    }

    private string GetCurrentCircleFunction(){
        if(isXPlus && isYPlus){
            if(centerX == 0f && centerY == 0f){
                return $"x^2+y^2={radius}";
            }else if(centerX == 0){
                return $"x^2+(y+{centerY})^2={radius}";
            }else if(centerY == 0){
                return $"(x+{centerX})^2+y^2={radius}";
            }
            return $"(x+{centerX})^2+(y+{centerY})^2={radius}";
        }else if(!isXPlus && isYPlus){
            if(centerX == 0f && centerY == 0f){
                return $"x^2+y^2={radius}";
            }else if(centerX == 0){
                return $"x^2+(y+{centerY})^2={radius}";
            }else if(centerY == 0){
                return $"(x-{centerX})^2+y^2={radius}";
            }
            return $"(x-{centerX})^2+(y+{centerY})^2={radius}";
        }else if(isXPlus && !isYPlus){
            if(centerX == 0f && centerY == 0f){
                return $"x^2+y^2={radius}";
            }else if(centerX == 0){
                return $"x^2+(y-{centerY})^2={radius}";
            }else if(centerY == 0){
                return $"(x+{centerX})^2+y^2={radius}";
            }
            return $"(x+{centerX})^2+(y-{centerY})^2={radius}";
        }else{ //!isXPlus && !isYPlus
            if(centerX == 0f && centerY == 0f){
                return $"x^2+y^2={radius}";
            }else if(centerX == 0){
                return $"x^2+(y-{centerY})^2={radius}";
            }else if(centerY == 0){
                return $"(x-{centerX})^2+y^2={radius}";
            }
            return $"(x-{centerX})^2+(y-{centerY})^2={radius}";
        }
    }

    public void StartDynamic(){
        if(startSimulationButton.GetComponent<Toggle>().isOn){
            GameObject[] dynamicCircles = GameObject.FindGameObjectsWithTag("DynamicCircle");
            foreach(GameObject dynamicC in dynamicCircles){
                dynamicC.GetComponent<Circle>().onDynamic();
                dynamicC.tag = "onSimulate";
            }
        }else{
            Debug.Log(isCircleTabOn);
            if(isCircleTabOn){
                DrawCircle(new Vector3(centerX, centerY, 0), radius);
                // DrawCircle(new Vector3(centerX, centerY, 0), radius);
                currentCircleObject.tag = "Preview"; //onSimulateと区別
            }
        }
    }

    void InstantiateCircle(){
            // rangeErrorText.text = "";
            currentCircleObject = Instantiate(CirclePrefabs, this.transform); //インスタンス化
            currentCircleScript = currentCircleObject.GetComponent<Circle>(); //CircleプレハブのCircleスクリプトを取得
            currentCircleScript.SetLineColor(PreviewLineColor);
            currentCircleScript.SetLineWidth(LineWidth);
    }

    public void SetCenterX(){
        if(float.TryParse(CenterXField.text, out centerX)){
            if(isXPlus){
                centerX = (-1f)*centerX;
            }
            rangeErrorText.text = "";
            Draw();
        }else{
            rangeErrorText.text = "数値の確認をしよう！";
        }
    }
    public void SetCenterY(){
        if(float.TryParse(CenterYField.text, out centerY)){
            if(isYPlus){
                centerX = (-1f)*centerY;
            }
            rangeErrorText.text = "";
            Draw();
        }else{
            rangeErrorText.text = "数値の確認をしよう！";
        }
    }
    public void SetRadius(){
        if(float.TryParse(RadiusField.text, out radius)){
            rangeErrorText.text = "";
            Draw();
        }else{
            rangeErrorText.text = "数値の確認をしよう！";
        }
    }

    public void DeletePreview(){
        // if(currentCircleObject && currentCircleObject.tag == "Preview"){
        //     Destroy(GameObject.FindWithTag("Preview")); //TabがGraphに移っているならPreviewを削除
        // }
        GameObject[] previews = GameObject.FindGameObjectsWithTag("Preview");
        foreach (GameObject preview in previews)
        {
            Destroy(preview);
        }
    }

}
