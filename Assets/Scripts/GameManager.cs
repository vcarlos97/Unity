using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    //Usamos Singleton
    public static GameManager instance = null;
    public float turnDelay = 0.1f; //Espera entre movimientos de los enemigos (1 turno). Una decima porque es lo que tarda en moverse un objeto

    public BoardManager boardScript; //Variable del mapa
    public int playerFoodPoints = 100; //Variable de la vida que se mantiene entre escenas
    [HideInInspector] public bool playersTurn = true; //Variable que no aparece en inspector y dice si es el turno del jugador
    public float levelStartDelay = 2f; //Segundos que estara visible el canvas del dia
    public bool doingSetup; //Para saber si aun se esta preparando la escena. Se pondra true al quitar la pantalla del nivel

    private List<Enemy> enemies = new List<Enemy>(); //Lista de enemigos
    private bool enemiesMoving; //Para ver si los enemigos se mueven o no
    private int level = 0; //Variable que va contando en que nivel estamos

    //Variables para el canvas
    private GameObject levelImage; //Imagen negra con el texto
    private Text levelText; //Texto del dia

    private void Awake(){
        //Iniciamos Singleton
        if(instance==null){
            instance=this;
        }else if(instance != this){
            Destroy(gameObject);
        }

        //Que no se destruya el objecto gameManager al cambiar de escenas
        DontDestroyOnLoad(gameObject);

        boardScript = GetComponent<BoardManager>();
    }

    //Inicia escena
    private void Start(){
        //No llamamos al InitGame porque lo llamamos en el OnFinishedLevelLoading
    }

    //Se llama cada vez que creemos una escena(nivel) en pantalla
    void InitGame(){

        //CODIGO PARA MOSTRAR LA IMAGEN ANTES DE INICIAR NIVEL
        doingSetup = true; //Para que el jugador no se mueva mientras preparamos la escena
        levelImage = GameObject.Find("LevelImage"); //Referencia a la imagen
        levelText = GameObject.Find("LevelText").GetComponent<Text>(); //Referencia al texto del nivel
        levelText.text = "Day " + level; //Actualiza el texto para el nivel actual
        levelImage.SetActive(true); //Activamos la imagen

        enemies.Clear(); //Vaciamos lista enemigos
        boardScript.SetupScene(level); //Inicializa mapa en el nivel que le pasamos

        Invoke("HideLevelImage", levelStartDelay);  //Borra la imagen y empieza nivel
    }

    //Metodo para esconder la imagen
    private void HideLevelImage(){
        levelImage.SetActive(false);
        doingSetup = false;
    }

    //Funcion fin del juego porque se queda sin vida
    public void GameOver(){
        levelText.text = "After " + level + " days, you starved."; //Te notifica hasta que nivel has llegado
        levelImage.SetActive(true); //Muestra escena
        enabled = false; //Fin juego
    }

    //Creamos una corutina para mover enemigos por turnos (esperando el tiempo de delay)
    //Hay que esperar el tiempo que esta el jugador moviendose
    IEnumerator MoveEnemies(){
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if(enemies.Count==0){
            yield return new WaitForSeconds(turnDelay); //Para que el jugador no se mueva instantaneo
        }
        //Recorremos lista enemigos para que se vayan moviendo
        for(int i=0; i<enemies.Count; i++){
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        playersTurn=true; //Decimos que es el turno de jugador para que se ejecute su Update
        enemiesMoving = false;
    }

    //Llamamos a la corutina cuando no sea el turno del jugador, cuando los enemigos no se muevan y cuando la escena este lista
    private void Update(){
        if (playersTurn || enemiesMoving || doingSetup) return; //Si le toca al jugador o se mueven los enemigos salimos del metodo
        StartCoroutine(MoveEnemies());
    }

    //Añadimos enemigos a la lista
    public void AddEnemyToList(Enemy enemy){
        enemies.Add(enemy);
    }

    //Si se activa la escena llamamos al metodo de nivel finalizado
    private void OnEnable(){
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable(){
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    //Usar este metodo como delegado para que cuando se recargue el nivel se llame a un metodo
    //Siempre que se recargue la escena se llama a este metodo
    //El metodo llama a InitGame para el siguiente nivel
    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode){
        level++;
        InitGame();
    }
}
