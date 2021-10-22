using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphList : MonoBehaviour
{
    public GameObject GraphPanelPrefab;
    public GameObject ScrollViewContent;
    private Text ButtonTextComponent;

    public void AddGraphPanel(string funcText){
        GameObject CurrentGraphPanel = Instantiate(GraphPanelPrefab) as GameObject;
        CurrentGraphPanel.name = funcText + "panel";
        CurrentGraphPanel.tag = "GraphPanel";
        ButtonTextComponent = CurrentGraphPanel.GetComponentInChildren<Text>();
        // Debug.Log(ButtonText);
        ButtonTextComponent.text = funcText;
        CurrentGraphPanel.transform.SetParent(ScrollViewContent.transform, false);
    }

    public void AddCirclePanel(string funcText, float centerX, float centerY, float radius, bool isXPlus, bool isYPlus){
        GameObject CurrentGraphPanel = Instantiate(GraphPanelPrefab) as GameObject;
        string xPlus;
        string yPlus;
        if(isXPlus){
            xPlus = "1";
        }else{
            xPlus = "0";
        }
        if(isYPlus){
            yPlus = "1";
        }else{
            yPlus = "0";
        }
        CurrentGraphPanel.name = centerX.ToString() + "/" + centerY.ToString() + "/" + radius.ToString() + xPlus + yPlus;
        CurrentGraphPanel.tag = "CirclePanel";
        ButtonTextComponent = CurrentGraphPanel.GetComponentInChildren<Text>();
        // Debug.Log(ButtonText);
        ButtonTextComponent.text = funcText;
        CurrentGraphPanel.transform.SetParent(ScrollViewContent.transform, false);
    }
    // void Start()
    // {

    // }

}
