using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputScript : MonoBehaviour
{
    public int inputMode = 0;
    public GraphDrawer GraphDrawer;
    public BallGraphDrawer BallGraphDrawer;
    [System.NonSerialized]
    public string funcString = "";
    private InputField FunctionInput;
    private InputField Input1;
    private InputField Input2;
    private InputField Input3;
    private Toggle Toggle1;
    private Toggle Toggle2;

    void Start()
    {
        if(inputMode == 0){
            FunctionInput = GameObject.Find("FunctionInput").gameObject.GetComponent<InputField>();
        }else if(inputMode == 1){
            Input1 = GameObject.Find("Input1").gameObject.GetComponent<InputField>();
            Input2 = GameObject.Find("Input2").gameObject.GetComponent<InputField>();
            Toggle1 = GameObject.Find("Toggle1").gameObject.GetComponent<Toggle>();
            Input1.text = "1";
            Input2.text = "0";
        }else if(inputMode == 2){
            Input1 = GameObject.Find("Input1").gameObject.GetComponent<InputField>();
            Input2 = GameObject.Find("Input2").gameObject.GetComponent<InputField>();
            Input3 = GameObject.Find("Input3").gameObject.GetComponent<InputField>();
            Toggle1 = GameObject.Find("Toggle1").gameObject.GetComponent<Toggle>();
            Toggle2 = GameObject.Find("Toggle2").gameObject.GetComponent<Toggle>();
            Input1.text = "1";
            Input2.text = "0";
            Input3.text = "0";
        }
    }

    public void UpdateFuncString(){
        // Debug.Log(currentFuncString);
        if(inputMode == 0){
            funcString = FunctionInput.text;
            if(funcString.Length > 0){
                SendFuncString(funcString);
            }
        }else if(inputMode == 1){
            string a = InputChecker1(Input1);
            string b = InputChecker0(Input2);
            string toggle1 = ToggleChecker(Toggle1);
            funcString = $"{a}x{toggle1}{b}";
            SendFuncString(funcString);
        }else if(inputMode == 2){
            string a = InputChecker1(Input1);
            string b = InputChecker0(Input2);
            string c = InputChecker0(Input3);
            string toggle1 = ToggleChecker(Toggle1);
            string toggle2 = ToggleChecker(Toggle2);
            funcString = $"{a}(x {toggle1} {b})^2 {toggle2} {c}";
            SendFuncString(funcString);
        }
    }

    private string InputChecker0(InputField input){
        if(input.text == ""){
            return "0";
        }else{
            return input.text;
        }
    }
    private string InputChecker1(InputField input){
        if(input.text == ""){
            return "1";
        }else{
            return input.text;
        }
    }

    private string ToggleChecker(Toggle toggle){
        if(toggle.isOn){
            return "+";
        }else{
            return "-";
        }
    }

    private void SendFuncString(string funcString){
        if(GraphDrawer != null){
            GraphDrawer.currentFuncString = funcString;
            GraphDrawer.Draw();
        }else if(BallGraphDrawer != null){
            BallGraphDrawer.currentFuncString = funcString;
            BallGraphDrawer.Draw();
        }
    }
}
