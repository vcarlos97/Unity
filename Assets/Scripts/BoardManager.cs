using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{

    //Columnas y filas del mapa
    public int columns = 8;
    public int rows = 8;

    //Arrays de suelo y muros para inicializar el mapa
    //Arrays para los prefabs y GameObject para la salida
    public GameObject[] floorTiles, outerWallTiles, wallTiles, foodTiles, enemyTiles; //Losas
    public GameObject exit;

    //Variable privada que ejercera de padre de los arrays para tener jerarquia limpia en el proyecto
    private Transform boardHolder;

    //Lista de casillas disponibles para añadir objetos
    private List<Vector2> gridPositions = new List<Vector2>();

    //Iniciamos la lista de casillas disponibles para añadir objetos
    //Usamos 1 y columns-1 para que el jugador siempre pueda llegar a la salida
    void InitializeList(){
        gridPositions.Clear();
        for(int x = 1; x < columns - 1; x++){
            for(int y = 1; y < rows - 1; y ++){
                gridPositions.Add(new Vector2(x,y));
            }
        }
    }

    //Obtendra numeros aleatorios para las posiciones de la lista gridPositions
    Vector2 RandomPosition(){
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector2 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex); //Borramos la posicion escogida para que no aparezcan objetos encima del otro
        return randomPosition;
    }

    //Metodo que instancia objetos (entre un min y un max)
    void LayoutObjectAtRandom(GameObject[] tileArray, int min, int max){

        int objectCount = Random.Range(min,max+1);
        for(int i=0;i<objectCount;i++){
            Vector2 randomPosition = RandomPosition();
            GameObject tileChoice = GetRandomInArray(tileArray);
            GameObject instance = Instantiate(tileChoice, randomPosition, Quaternion.identity);
            //Para jerarquia limpia
            instance.transform.SetParent(boardHolder);
        }
    }

    //Inicializa la escena y le pasamos por parametros el nivel
    public void SetupScene(int level){
        BoardSetup();
        InitializeList();
        LayoutObjectAtRandom(wallTiles,5,9);
        LayoutObjectAtRandom(foodTiles,1,5);
        int enemyCount = level;
        LayoutObjectAtRandom(enemyTiles,enemyCount,enemyCount);
        GameObject exitInstance = Instantiate(exit, new Vector2(columns-1,rows-1), Quaternion.identity);
        exitInstance.transform.SetParent(boardHolder);
    }

    //Inicializa el mapa
    void BoardSetup(){

        //Inicializamos boardHolder
        boardHolder = new GameObject("Board").transform;

        //ponemos -1 y +1 para asignar los bordes del mapa
        for(int x = -1; x < columns + 1; x++){
            for(int y = -1; y < rows + 1; y ++){
                //Obtiene un prefab aleatorio del suelo
                GameObject toInstantiate = GetRandomInArray(floorTiles);

                //Obtiene prefab aleatorio de los muros
                if(x == -1 || y == -1 || x == columns || y == rows){
                    toInstantiate = GetRandomInArray(outerWallTiles);
                }

                //Llamamos a los objetos que hemos creado y fijamos quien es su superior en la jerarquia
                GameObject instance = Instantiate(toInstantiate, new Vector2(x,y), Quaternion.identity);
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    //Obtiene un prefab aleatorio del array
    GameObject GetRandomInArray(GameObject[] array){
        return array[Random.Range(0, array.Length)];
    }
}
