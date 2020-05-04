using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Script jugador que hereda de MovingObject para que se mueva
public class Player : MovingObject
{
    public int wallDamage = 1; //Daño que le haremos al muro cuando el jugador se acerca
    public int pointsPerFood = 10; //Puntos por comida
    public int pointsPerSoda = 20; //Puntos por comida
    public float restartLevelDelay = 1f;//Segundos para esperar antes de cargar la siguiente escena
    public Text foodText; //Texto con la vida

    private Animator animator; //Animacion del jugador
    private int food; //Contador de comida/vida, variable que usaremos mientras jugamos el nivel

    //Funcion abstracta implementada
    //Referencias al animator
    protected override void Awake(){
        animator = GetComponent<Animator>();
        base.Awake();
    }

    //Start heredado
    protected override void Start(){
        food = GameManager.instance.playerFoodPoints; //Asignamos vida a la de gameManager para conservar entre escenas
        foodText.text = "Food: " + food; //Muestra vida en pantalla
        base.Start();
    }

    //Funcion que desactiva jugador para cambiar de escena/nivel
    private void OnDisable(){
        GameManager.instance.playerFoodPoints = food; //Le pasa la vida actual despues de jugar el nivel para mantenerlo
    }

    //Metodo que comprueba si hemos terminado la partida porque nos quedamos sin vida
    void CheckIfGameOver(){
        if (food<=0) GameManager.instance.GameOver(); //Si se queda sin vida terminamos partida
    }

    //Funcion heredada que mueve al jugador y resta comida
    protected override void AttemptMove(int xDir, int yDir){
        food--; //Decrementamos 1 la vida
        foodText.text = "Food: " + food; //Actualiza vida en pantalla
        base.AttemptMove(xDir,yDir); //Devuelve true si se puede mover
        CheckIfGameOver(); //Comprobamos si tenemos vida
        GameManager.instance.playersTurn=false; //Notificamos que termina turno de jugador
    }

    //Update is called once per frame
    void Update(){
        //Si no es el turno del jugador o la escena esta cargando, no hace nada
        if (!GameManager.instance.playersTurn || GameManager.instance.doingSetup) return;

        //Lee que letra del teclado pulsamos
        //HABRA QUE ADAPTARLO PARA ANDROID!!!!!
        int horizontal=0;
        int vertical=0;
        horizontal = (int) (Input.GetAxisRaw ("Horizontal")); //0, 1 para derecha, -1 para izquierda
        vertical = (int) (Input.GetAxisRaw ("Vertical")); //0, 1 arriba, -1 abajo

        if(horizontal!= 0) vertical=0; //Para evitar que se mueva en diagonal
        if(horizontal != 0 || vertical != 0) AttemptMove(horizontal, vertical); //Probar si nos podemos mover
    }

    //Funcion abstracta implementada que nos activa las acciones de los objetos
    protected override void OnCantMove(GameObject go){
        Wall hitWall = go.GetComponent<Wall>(); //Probamos si es un muro lo que nos impide pasar
        if(hitWall != null){
            hitWall.DamageWall(wallDamage); //Dañamos al muro
            animator.SetTrigger("playerChop"); //Indicamos que se active la animacion
        }
    }

    //Metodo para reiniciar la escena
    void Restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Metodo para perder vida cuando te golpee un enemigo
    public void LoseFood(int loss){
        food -= loss; //Resta vida
        foodText.text = "-" + loss + " Food: " + food; //Actualiza vida en pantalla
        animator.SetTrigger("playerHit"); //Activa animacion
        CheckIfGameOver(); //Comprueba que tengamos vida
    }

    //Comprueba si esta encima de un objeto
    private void OnTriggerEnter2D (Collider2D other){
        if(other.CompareTag("Exit")){ //Si llega a la salida
            Invoke("Restart", restartLevelDelay); //Invocamos al cambio de escena con el delay asignado
            enabled = false; //Para que no se mueva mas
        }
        else if(other.CompareTag("Food")){
            food += pointsPerFood; //Sumamos comida
            foodText.text = "+" + pointsPerSoda + " Food: " + food; //Actualiza vida en pantalla
            other.gameObject.SetActive(false); //Desactivamos el objeto
        }
        else if(other.CompareTag("Soda")){
            food += pointsPerSoda; //Sumamos soda
            foodText.text = "+" + pointsPerFood + " Food: " + food; //Actualiza vida en pantalla
            other.gameObject.SetActive(false); //Desactivamos el objeto
        }
    }
}
