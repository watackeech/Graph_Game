using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public EdgeCollider2D edgeCollider;
    public Rigidbody2D rb;

    [HideInInspector] public List<Vector2> points = new List<Vector2>(); //EdgeCollider用のList
    [HideInInspector] public int pointsCount = 0; //頂点の数

    private float CircleColliderRadius;


    public void ChangeLineColor(Gradient LineColor)
    {
        lineRenderer.colorGradient = LineColor;
    }

    public void SetLineColor(Gradient LineColor)
    {
        lineRenderer.colorGradient = LineColor;
    }

    public void SetLineWidth(float width)
    {
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;

        edgeCollider.edgeRadius = width / 2f;
        CircleColliderRadius = width / 2f;
    }

    public void AddPoint(Vector2 newPoint)
    {
        points.Add(newPoint);
        pointsCount++;

        // Add CircleCollider to the point
        CircleCollider2D circleCollider = this.gameObject.AddComponent<CircleCollider2D>();
        circleCollider.offset = newPoint;
        circleCollider.radius = CircleColliderRadius;

        //LineRenderer
        lineRenderer.positionCount = pointsCount;
        lineRenderer.SetPosition(pointsCount - 1, newPoint);

        //EdgeCollider
        if (pointsCount > 1)
            edgeCollider.points = points.ToArray();
    }

    // public void onPhysic()
    // {
    //     rb.simulated = true;
    // }

    public void onDynamic(){
        rb.constraints = RigidbodyConstraints2D.None; //制限解除
    }

    public void onCollider(){
        rb.constraints = RigidbodyConstraints2D.FreezeAll; //全動きを制限
        // rb.constraints = RigidbodyConstraints2D.FreezePosition; //位置固定回転アリ
        rb.simulated = true;
        // edgeCollider.enabled = true;
        // circleCollider.enabled = true;
    }
    void OnTriggerEnterh2D( Collider2D col ){ //画面外にグラフが出たらオブジェクト削除
        if(col.gameObject.tag == "DeleteGraph"){
            Destroy(this.gameObject);
        }
    }
    public void deleteThisGraph(){
        Destroy(this.gameObject);
    }
}
