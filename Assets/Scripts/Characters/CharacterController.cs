using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public GameObject PinkCharacter;
    PinkCharacter pinkScript;
    private GameObject CurrentCharacter;
    // Start is called before the first frame update

    void Awake() //SceneViewCameraでキャラを取得するため
    {
        CurrentCharacter = Instantiate(PinkCharacter, this.transform);
        pinkScript = CurrentCharacter.GetComponent<PinkCharacter>();
    }

    public void DrawCharacter(){
        CurrentCharacter = Instantiate(PinkCharacter, this.transform);
    }

    public void PinkStartSimulation(){
        pinkScript.StartSimulation();
    }
    public void ResetCharacters(){
        pinkScript.Reset();
    }
    public void RebirthCharacters(){
        pinkScript.Rebirth();
    }
}
