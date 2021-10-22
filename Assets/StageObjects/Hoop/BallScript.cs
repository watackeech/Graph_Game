using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    private GameObject graphDrawerObject;
    private BallGraphDrawer graphDrawerScript;

    void Start(){
        graphDrawerObject = GameObject.Find("GraphDrawer");
        graphDrawerScript = graphDrawerObject.GetComponent<BallGraphDrawer>();
    }

    void OnTriggerEnter2D( Collider2D col ){
        if(col.gameObject.tag == "DeadArea"){
            graphDrawerScript.Draw();
            Destroy(this.gameObject);
        }else if(col.gameObject.tag == "Shooter"){
            return;
        }else{
            Debug.Log(col.gameObject.tag);
            graphDrawerScript.ReleaseBall();
        }
    }
}
