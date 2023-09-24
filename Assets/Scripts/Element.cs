using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    [SerializeField]
    private bool mine; // Is this element a mine?

    [SerializeField]
    private Sprite[] emptyTextures; // List of empty textures i.e. empty, 1, 2, 3, 4, 5, 6, 7, 8

    [SerializeField]
    private Sprite mineTexture; // Mine texture

    // Start is called before the first frame update
    void Start()
    {
        mine = Random.value < 0.15; // 15% chance of being a mine
        int x = (int)transform.position.x;
        int y = (int)transform.position.y;

        GameController.elements[x, y] = this; // Register in grid
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsMine()
    {
        return this.mine;
    }

    public void LoadTexture(int adjacentCount)
    {

        if (mine)
        {
            GetComponent<SpriteRenderer>().sprite = mineTexture;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = emptyTextures[adjacentCount];
        }
    }

    private void OnMouseUpAsButton()
    {       
        if (GameController.GameState == "Play"){
        if (mine)
        {
            // tela de game over
            this.LoadTexture(0);
            print("Game Over!!!");
            GameController.UncoverMines();
            GameController.GameState = "Game Over";
        }
        else
        {
            // lógica proximidade mina
            int x = (int)transform.position.x;
            int y = (int)transform.position.y;

            int adjacentMines = GameController.AdjacentMines(x, y);

            this.LoadTexture(adjacentMines);
            
            bool[,] visited = new bool[GameController.gridWidth, GameController.gridHeight];

            StartCoroutine(GameController.DFSFFuncover(x, y, visited));
            
            // GameController.DFSFFuncover(x, y, new bool[GameController.gridWidth, GameController.gridHeight]);

            if(GameController.IsFinished())
            {
                print("You Win!!!");
                GameController.GameState = "Win";
            }
        }
        }
    }

    public bool IsCovered()
    {
        if(GetComponent<SpriteRenderer>().sprite.texture.name == "default")
        {
            return true;
        }
        return false;
    }
}
