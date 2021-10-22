using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grids : MonoBehaviour
{
    public LineRenderer lineRenderer;
    [HideInInspector] public int pointsCount = 0; //頂点の数

    public void SetLineWidth(float width)
    {
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
    }

    public void SetLineAlpha(float alpha)
    {
        // lineRenderer.colorGradient = LineColor;
        lineRenderer.startColor = new Color(0f,1f,1f, alpha);
        lineRenderer.endColor = new Color(0f,1f,1f, alpha);;
    }

    public void AddPoint(Vector2 newPoint)
    {
        // points.Add(newPoint);
        pointsCount++;
        //LineRenderer
        lineRenderer.positionCount = pointsCount;
        lineRenderer.SetPosition(pointsCount - 1, newPoint);
    }
}
