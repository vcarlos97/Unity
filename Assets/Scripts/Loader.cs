using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    //Script que instancia un GameManager si no esta creado
    public GameObject gameManager;

    private void Awake(){
        if(GameManager.instance==null){
            Instantiate(gameManager);
        }
    }
}
