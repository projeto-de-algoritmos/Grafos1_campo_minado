using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] // SerializeField makes private variables visible in the inspector
    private GameObject block; // Prefab of the block

    public static int gridWidth = 10;
    public static int gridHeight = 13;

    public static Element[,] elements = new Element[gridWidth, gridHeight]; // Array of elements
    public static string GameState; //Stop, GameOver, Win, Play
    // Start is called before the first frame update
    void Start()
    {
        this.CreateField();
        GameController.GameState = "Play";
    }

    // Update is called once per frame
    void Update()
    {
        
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
