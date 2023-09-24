using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class GameController : MonoBehaviour
{
    [SerializeField]
    Image Menu;
    [SerializeField]
    private Text msgGameOver;
    [SerializeField]
    private Text msgCamp;
    [SerializeField]
    private Text msgSpace;
    [SerializeField] // SerializeField makes private variables visible in the inspector
    private GameObject block; // Prefab of the block
    [SerializeField] // SerializeField makes private variables visible in the inspector
    private AudioClip audioWin;
    [SerializeField] 
    private AudioClip audioGameOver;

    public static int gridWidth = 10;
    public static int gridHeight = 13;

    public static Element[,] elements = new Element[gridWidth, gridHeight]; // Array of elements
    public static string GameState; //Stop, GameOver, Win, Play
    private AudioSource playSound;
    // Start is called before the first frame update
    void Start()
    {
        this.CreateField();
        GameController.GameState = "Stop";
        playSound = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (GameController.GameState == "Game Over"){
            Menu.enabled = true;
            msgGameOver.gameObject.active = true;
            msgGameOver.text="Game Over!!!";
            GameController.GameState = "Stop";
            playSound.clip = audioGameOver;
            playSound.Play();
            print("Game Over!!!");
        }
         if (GameController.GameState == "Win"){
            Menu.enabled = true;
            msgGameOver.gameObject.active = true;
            msgGameOver.text="Wim!!!";
            GameController.GameState = "Stop";
            playSound.clip = audioWin;
            playSound.Play();
            print("Win!!!");
         }
         if (GameController.GameState == "Stop"){
            msgCamp.gameObject.active = true;
            msgSpace.gameObject.active = true;
            if (Input.GetKeyDown(KeyCode.Space)){
                StartGame();
            }
         }
    }
   
    private void StartGame(){
        Menu.enabled = false;
        msgSpace.gameObject.active = false;
        msgCamp.gameObject.active = false;
        msgGameOver.gameObject.active = false;
        foreach (Element item in elements){
            item.RestartDefault();
        }
        GameController.GameState = "Play";
    }

    // Create the mine field
    private void CreateField()
    {
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                Instantiate(block, new Vector3(i, j, 0f), Quaternion.identity);
            }
        }
    }

    public static void UncoverMines()
    {
        foreach (Element item in elements)
        {
            if(item.IsMine())
            {
                item.LoadTexture(0);
            }
        }
    }

    // Is the element at the given position a mine?
    public static bool MineAt(int x, int y)
    {
        // Coordinates in range? Then check for mine.
        if (x >= 0 && y >= 0 && x < gridWidth && y < gridHeight)
        {
            return elements[x, y].IsMine();
        }
        return false;
    }

    public static int AdjacentMines(int x, int y)
    {
        int count = 0;
        for(int i = x - 1; i <= x + 1; i++)
            for(int j = y - 1; j <= y + 1; j++)
                if(MineAt(i, j)) count++;
        return count;
    }

    public static IEnumerator DFSFFuncover(int x, int y, bool[,] visited)
    {
        // Coordinates in range?
        if (x >= 0 && y >= 0 && x < gridWidth && y < gridHeight)
        {
            if (visited[x, y]) yield break;

            visited[x, y] = true;

            yield return new WaitForSeconds(0.2f);

            elements[x, y].LoadTexture(AdjacentMines(x, y));

            if (AdjacentMines(x, y) > 0) yield break;

            yield return DFSFFuncover(x - 1, y, visited);
            yield return DFSFFuncover(x - 1, y + 1, visited);
            yield return DFSFFuncover(x, y + 1, visited);
            yield return DFSFFuncover(x + 1, y + 1, visited);
            yield return DFSFFuncover(x + 1, y, visited);
            yield return DFSFFuncover(x + 1, y - 1, visited);
            yield return DFSFFuncover(x, y - 1, visited);
            yield return DFSFFuncover(x - 1, y - 1, visited);

        }
    }

    public static bool IsFinished()
    {
        // Try to find a covered element that is no mine
        foreach (Element item in elements)
            if(item.IsCovered() && !item.IsMine())
                return false;
                
        // There are none => all are mines => game won.
        return true;
    }
}
