using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    public int Rows = 4;
    public int Columns = 4;

    public GameObject Wall;
    public GameObject Floor;

    // Start is called before the first frame update
    void Start()
    {
        float size = Wall.transform.localScale.x;

        //Create the maze grid itself
        for (int i = 0; i < Rows; i++){
            for (int j = 0; j < Columns; j++){
                //This is gonna go for all the columns and rows of the game, so 4x4=16 times and will create 16 floors..
                GameObject floor = Instantiate(Floor, new Vector3(j * size, 0, -i * size), Quaternion.identity);
                floor.name = "Floor_" + i + "_" + j;    //Rename the floor obj so you can see where they are in the board. 

                //Add Walls
                GameObject upWall = Instantiate(Wall, new Vector3(j * size, 1.75f, -i * size + 1.25f), Quaternion.identity);
                upWall.name = "UpWall_" + i + "_" + j;

                GameObject downWall = Instantiate(Wall, new Vector3(j * size, 1.75f, -i * size - 1.25f), Quaternion.identity);
                downWall.name = "DownWall_" + i + "_" + j;

                GameObject leftWall = Instantiate(Wall, new Vector3(j * size - 1.25f, 1.75f, -i * size), Quaternion.Euler(0, 90, 0)); //Rotate 90*
                leftWall.name = "LeftWall_" + i + "_" + j;

                GameObject rightWall = Instantiate(Wall, new Vector3(j * size + 1.25f, 1.75f, -i * size), Quaternion.Euler(0, 90, 0));
                rightWall.name = "LeftWall_" + i + "_" + j;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
