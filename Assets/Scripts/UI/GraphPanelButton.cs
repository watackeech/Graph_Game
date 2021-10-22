using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphPanelButton : MonoBehaviour
{
    private Text childText;
    private InputField funcInput;
    private GameObject targetGraph;
    private GraphDrawer graphDrawerScript;
    private Toggle graphToggle;
    private Toggle circleToggle;
    private InputField centerXField;
    private InputField centerYField;
    private InputField radiusField;
    private Toggle xPlusChecker;
    private Toggle yPlusChecker;
    private CircleDrawer circleDrawerScript;

    private void Awake(){
        GetComponent<Button>().onClick.AddListener ((UnityEngine.Events.UnityAction) this.OnClick);
    }

    private void Start()
    {
        childText = this.gameObject.GetComponentInChildren<Text>();
        if(GameObject.Find("FunctionInput")){
            funcInput = GameObject.Find("FunctionInput").GetComponent<InputField>();
        }
        graphDrawerScript = GameObject.Find("GraphDrawer").GetComponent<GraphDrawer>();
        // Debug.Log(childText.text);
        //グラフ・円タブの切り替え用
        graphToggle = GameObject.Find("GraphToggle").GetComponent<Toggle>();
        circleToggle = GameObject.Find("CircleToggle").GetComponent<Toggle>();

        //円の管理
        if(this.gameObject.tag == "CirclePanel"){
            centerXField = GameObject.Find("InputX").GetComponent<InputField>();
            centerYField = GameObject.Find("InputY").GetComponent<InputField>();
            radiusField = GameObject.Find("InputR").GetComponent<InputField>();
            xPlusChecker = GameObject.Find("x+-").GetComponent<Toggle>();
            yPlusChecker = GameObject.Find("y+-").GetComponent<Toggle>();
            circleDrawerScript = GameObject.Find("CircleDrawer").GetComponent<CircleDrawer>();
        }
    }

    void OnClick(){
        // Debug.Log("おされた！！");
        if(this.gameObject.tag == "GraphPanel"){
            // Debug.Log("グラフ！");
            graphToggle.isOn = true;
            funcInput.text = childText.text;
            // 物理on/offスイッチの切り替えもしたい
            //xの定義域も
            targetGraph = GameObject.Find(childText.text);
            Destroy(targetGraph);
            graphDrawerScript.Draw();
            Destroy(this.gameObject);
        }else if(this.gameObject.tag == "CirclePanel"){
            circleToggle.isOn = true;
            string rawCircleString = this.gameObject.name;
            int firstBreak = rawCircleString.IndexOf("/");
            int secondBreak = rawCircleString.LastIndexOf("/");
            string centerX = rawCircleString.Substring(0, firstBreak);
            string centerY = rawCircleString.Substring(firstBreak + 1, secondBreak - firstBreak - 1);
            string radius = rawCircleString.Substring(secondBreak + 1, rawCircleString.Length - 2 - secondBreak - 1); //isXPlus / isYPlusの情報分は抜く
            string xPlus = rawCircleString.Substring(rawCircleString.Length - 2, 1);
            string yPlus = rawCircleString.Substring(rawCircleString.Length - 1, 1);
            if(xPlus == "1"){
                xPlusChecker.isOn = true;
            }else{
                xPlusChecker.isOn = false;
            }
            if(yPlus == "1"){
                yPlusChecker.isOn = true;
            }else{
                yPlusChecker.isOn = false;
            }
            centerXField.text = centerX;
            centerYField.text = centerY;
            radiusField.text = radius;
            Debug.Log($"{centerX}, {centerY}, 半径:{radius}, ｘは{xPlus}, ｙは{yPlus}");
            GameObject targetCircle = GameObject.Find(centerX + centerY + radius + xPlusChecker.isOn.ToString() + yPlusChecker.isOn.ToString());
            Destroy(targetCircle);
            circleDrawerScript.Draw();
            Destroy(this.gameObject);
        }
    }
}
// 01/34/67
// 8