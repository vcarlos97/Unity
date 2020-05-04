using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script que define el movimiento de los personajes

public abstract class MovingObject : MonoBehaviour
{
    public float moveTime = 0.1f; //Tiempo que va a tardar en cambiar de casilla
    public LayerMask blockingLayer; //Variable de objeto bloqueante

    private float movementSpeed; //Velocidad de movimiento
    private BoxCollider2D boxCollider; //Variable box collider
    private Rigidbody2D rb2D; //Variable rigidBody

    //Protected Virtual para poder sobreescribirlos en la clase que herede de movingObject(jugador)
    protected virtual void Awake(){
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        movementSpeed = 1f/moveTime;
    }

    //Metodo para ver si se puede mover un objeto
    //Devolvemos true/false y la variable hit
    //Protected porque queremos que sea privada para cualquier objeto que no herede de movingObject
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit){
        Vector2 start = transform.position; //Donde esta ahora el objeto
        Vector2 end = start + new Vector2(xDir,yDir); //Posicion a la que me quiero mover

        //Hacemos raycast para ver si hay un bloqueo en la posicion que queremos ir
        boxCollider.enabled = false; //Esto es por si colisionamos con un enemigo que nos haga daño
        hit = Physics2D.Linecast(start,end,blockingLayer);
        boxCollider.enabled = true; //Volvemos a habilitarlo

        if(hit.transform == null){ //No hay objeto
            //Hacer el movimiento usando la corutina SmoothMovement
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        return false; //Hay objeto
    }

    //Corutina de movimiento
    protected IEnumerator SmoothMovement(Vector2 end){
        //Variable de distancia entre 2 puntos
        float remainingDistance = Vector2.Distance(rb2D.position,end);
        while (remainingDistance > float.Epsilon){
            Vector2 newPosition = Vector2.MoveTowards(rb2D.position, end, remainingDistance);
            rb2D.MovePosition(newPosition);
            remainingDistance = Vector2.Distance(rb2D.position,end);
            yield return null;
        }
    }

    //Metodo abstracto para notificar que no se puede mover
    protected abstract void OnCantMove(GameObject go);

    //Metodo que llama a Move y intenta mover objeto
    protected virtual void AttemptMove(int xDir, int yDir){
        RaycastHit2D hit;
        bool canMove = Move(xDir,yDir,out hit);
        if(canMove) return;

        //Notificacion de que no se ha podido mover y devuelve contra que objeto se ha chocado
        OnCantMove(hit.transform.gameObject);
    }
}
