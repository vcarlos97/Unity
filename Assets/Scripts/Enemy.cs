using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Hereda tambien de MovingObject porque tambien se mueven
public class Enemy : MovingObject
{
    public int playerDamage; //Puntos de vida que quitamos al jugador

    private Animator animator; //Variable para las animaciones
    private Transform target; //Referencia el transform del jugador, para que el enemigo sepa donde esta el jugador (objetivo)
    private bool skipMove; //Los enemigos se mueven intermitentemente, 1 turno si 1 turno no

    //Asignamos referencia al animator y llamamos al metodo awake de la clase herencia
    protected override void Awake(){
        animator = GetComponent<Animator>();
        base.Awake();
    }

    //Metodo que se llama desde el MovingObject cuando no se puede mover
    protected override void OnCantMove (GameObject go){
        Player hitPlayer = go.GetComponent<Player>();
        if(hitPlayer != null){ //Si el objeto que bloquea es el jugador, le restamos vida
            hitPlayer.LoseFood(playerDamage);
            animator.SetTrigger("enemyAttack"); //Llamamos a la animacion
        }
    }

    //Referenciamos el objetivo y llamamos al Start heredado
    //Añadimos enemigo a la lista cuando se cree
    protected override void Start(){
        GameManager.instance.AddEnemyToList(this);
        target = GameObject.FindGameObjectWithTag("Player").transform; //Asignamos jugador como objetivo
        base.Start();
    }

    //Miramos si es el turno de moverse y probamos si se puede mover
    protected override void AttemptMove(int xDir, int yDir){
        if(skipMove){ //Comprobamos si se tiene que mover
            skipMove=false;
            return;
        }
        base.AttemptMove(xDir,yDir);
        skipMove = true; //Decimos que el siguiente turno no se mueva
    }

    //Metodo para mover al enemigo siguiendo al jugador
    //Este metodo lo llamara el GameManager para decirle a cada enemigo que se mueva
    //Si estan en la misma columna, ira para arriba o para abajo
    //Si no, ira izquierda o derecha, dependiendo de donde este el jugador
    public void MoveEnemy(){
        int xDir = 0;
        int yDir = 0;
        if(Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon){ //Comprobamos si el jugador esta en la misma columna
            yDir = target.position.y > transform.position.y ? 1 : -1; //Miramos si esta arriba o abajo. Si esta arriba 1, si no -1
        } else { //Si no esta en la misma columna, se mueve en horizontal
            xDir = target.position.x > transform.position.x ? 1 : -1; //Derecha=1, izquierda=-1
        }
        AttemptMove(xDir,yDir); //Intenta movimiento
    }
}
