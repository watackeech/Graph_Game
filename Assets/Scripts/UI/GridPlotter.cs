using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridPlotter : MonoBehaviour
{
    public GameObject GridPrefabs;
    public GameObject AxisCountPrefab;
    public int centerX = 0;
    public int centerY = 0;
    public int xStartRelative = -22;
    public int xEndRelative = 22;
    public int yStartRelative = -14;
    public int yEndRelative = 14;

    private int xStart;
    private int xEnd;
    private int yStart;
    private int yEnd;
    public float firstWidth = 0.12f;
    public float secondWidth = 0.08f;
    public float thirdWidth = 0.06f;

    private float LineWidth;
    // public Gradient LineColor;

    Grids currentGrid;

    void Start()
    {
        xStart = xStartRelative + centerX;
        xEnd = xEndRelative + centerX;
        yStart = yStartRelative + centerY;
        yEnd = yEndRelative + centerY;
        DrawGrid();
    }

    void DrawGrid()
    {
        //x軸
        // for(int i = -5; i < 6; i++)
        Debug.Log(xStart);
        for(int i = xStart; i < xEnd+1; i++)
        {
            currentGrid = Instantiate(GridPrefabs, this.transform).GetComponent<Grids>();
            if(i == centerX)
            {
                currentGrid.SetLineWidth(firstWidth);
                currentGrid.SetLineAlpha(0.9f);
            }else if((i - centerX) % 5 == 0){
                currentGrid.SetLineWidth(secondWidth);
                currentGrid.SetLineAlpha(0.6f);
                GameObject currentAxisCount = Instantiate(AxisCountPrefab, new Vector3(i, centerY - 0.5f, 0), Quaternion.identity);
                currentAxisCount.transform.SetParent(this.gameObject.transform);
                currentAxisCount.transform.localScale = new Vector3(0.18f, 0.18f, 1);
                currentAxisCount.GetComponent<Text>().text = (i - centerX).ToString();
            }else{
                currentGrid.SetLineWidth(thirdWidth);
                currentGrid.SetLineAlpha(0.3f);
            }
            // currentGrid.AddPoint(new Vector3(-9, i, 0));
            // currentGrid.AddPoint(new Vector3(9, i, 0));
            currentGrid.AddPoint(new Vector3(i, yStart, 0));
            currentGrid.AddPoint(new Vector3(i, yEnd, 0));
        }

        //y軸
        // for(int i = -9; i < 10; i++)
        for(int i = yStart; i < yEnd+1; i++)
        {
            currentGrid = Instantiate(GridPrefabs, this.transform).GetComponent<Grids>(); ;
            if(i == centerY)
            {
                currentGrid.SetLineWidth(firstWidth);
                currentGrid.SetLineAlpha(0.9f);
            }else if((i - centerY) % 5 == 0){
                currentGrid.SetLineWidth(secondWidth);
                currentGrid.SetLineAlpha(0.6f);
                GameObject currentAxisCount = Instantiate(AxisCountPrefab, new Vector3(0.5f + centerX, i, 0), Quaternion.identity);
                currentAxisCount.transform.SetParent(this.gameObject.transform);
                currentAxisCount.transform.localScale = new Vector3(0.18f, 0.18f, 1);
                currentAxisCount.GetComponent<Text>().text = (i - centerY).ToString();
            }else{
                currentGrid.SetLineWidth(thirdWidth);
                currentGrid.SetLineAlpha(0.3f);
            }
            // currentGrid.AddPoint(new Vector3(i, -5, 0));
            // currentGrid.AddPoint(new Vector3(i, 5, 0));
            currentGrid.AddPoint(new Vector3(xStart, i, 0));
            currentGrid.AddPoint(new Vector3(xEnd, i, 0));
        }

    }
}
