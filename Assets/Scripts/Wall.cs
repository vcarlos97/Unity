using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script que define que el muro se destruya si el jugador choca con el

public class Wall : MonoBehaviour
{

    //Variable para sustituir muro si recive golpes
    public Sprite dmgSprite;
    //Variable para golpes que resiste
    public int hp=4;

    private SpriteRenderer spriteRenderer;

    private void Awake(){
        spriteRenderer=GetComponent<SpriteRenderer>();
    }

    //Metodo para dañar el muro y que cambia imagen
    public void DamageWall(int loss){
        spriteRenderer.sprite = dmgSprite;
        hp -= loss;
        if(hp<=0){
            gameObject.SetActive(false); //Elimina el muro
        }
    }
}
