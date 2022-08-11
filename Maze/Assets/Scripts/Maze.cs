using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    public int Rows = 4;
    public int Columns = 4;

    public GameObject Wall;
    public GameObject Floor;

    //Create the grid? Two-Dimensional Array
    private MazeCell[,] grid;
    private int currentRow = 0;
    private int currentColumn = 0;

    // Start is called before the first frame update
    void Start(){

        //Grid with all the walls & floors
        CreateGrid();

        //Algorithm carves path from top left to bottom right
        HuntAndKill();
    }

    void CreateGrid(){
        float size = Wall.transform.localScale.x;
        grid = new MazeCell[Rows, Columns]; //Initalize the MazeCell

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


                //Create all the MazeCells / the grid
                grid[i, j] = new MazeCell();

                grid[i, j].UpWall = upWall;
                grid[i, j].DownWall = downWall;
                grid[i, j].LeftWall = leftWall;
                grid[i, j].RightWall = rightWall;


                //Make all the new gameobjects a child of Maze-object
                floor.transform.parent = transform;
                upWall.transform.parent = transform;
                downWall.transform.parent = transform;
                leftWall.transform.parent = transform;
                rightWall.transform.parent = transform;
            }
        }
    }

    void HuntAndKill(){
        //Mark the first cell of the random walk as visited.
        grid[currentRow, currentColumn].Visited = true;

        Walk();
        Hunt();                                    
        
    }

    void Walk(){
        while(AreThereUnvistedNeighbors()){
            //Choose a random direction to walk
            int direction = Random.Range(0,4);                                                      

            //Check up
            if(direction == 0){

                //check if row-up is visited
                if (IsCellUnvisitedAndWithinBoundaries(currentRow -1, currentColumn)){
                // if (currentRow > 0 && !grid[currentRow - 1, currentColumn].Visited){     //This is the same as the line above. 
                    
                    //Destroy if there is a wall above the cell.
                    if(grid[currentRow, currentColumn].UpWall){
                        //mark the visited cell and destroy the (up) wall between the two cells
                        Destroy(grid[currentRow, currentColumn].UpWall);
                    }

                    currentRow--; //We are going one row up. 
                    grid[currentRow, currentColumn].Visited = true;

                    //Destroy if there is a wall beneath the cell.
                    if(grid[currentRow, currentColumn].DownWall){
                        //mark the visited cell and destroy the (down) wall between the two cells
                        Destroy(grid[currentRow, currentColumn].DownWall);
                    }
                }
            }
            //Check down
            else if(direction == 1){
                if (IsCellUnvisitedAndWithinBoundaries(currentRow + 1, currentColumn)){             //if (currentRow < Rows - 1 && !grid[currentRow + 1, currentColumn].Visited){

                    if(grid[currentRow, currentColumn].DownWall){
                        Destroy(grid[currentRow, currentColumn].DownWall);
                    }

                    currentRow++; //We are going one row down. 
                    grid[currentRow, currentColumn].Visited = true;

                    
                    if(grid[currentRow, currentColumn].UpWall){
                        Destroy(grid[currentRow, currentColumn].UpWall);
                    }
                }
            }
            //Check left
            else if(direction == 2){
                if (IsCellUnvisitedAndWithinBoundaries(currentRow, currentColumn - 1)){         //if (currentColumn > 0 && !grid[currentRow, currentColumn - 1].Visited){
                    
                    if(grid[currentRow, currentColumn].LeftWall){
                        Destroy(grid[currentRow, currentColumn].LeftWall);
                    }

                    currentColumn--; //We are going one row up. 
                    grid[currentRow, currentColumn].Visited = true;

                    if(grid[currentRow, currentColumn].RightWall){
                        Destroy(grid[currentRow, currentColumn].RightWall);
                    }
                }
            }
            //Check right
            else if(direction == 3){
                if (IsCellUnvisitedAndWithinBoundaries(currentRow, currentColumn +1)){         //if (currentColumn < Columns -1 && !grid[currentRow, currentColumn +1].Visited){

                    if(grid[currentRow, currentColumn].RightWall){
                        Destroy(grid[currentRow, currentColumn].RightWall);
                    }

                    currentColumn++; //We are going one row down. 
                    grid[currentRow, currentColumn].Visited = true;

                    if(grid[currentRow, currentColumn].LeftWall){
                        Destroy(grid[currentRow, currentColumn].LeftWall);
                    }
                }
            }
        }
    }

    //Scan the grid looking for an unvisited cell that is adjacented to a visited cell
    void Hunt(){
        for (int i = 0; i<Rows; i++){
            for (int j = 0; j < Columns; j++){
                
                //Scan the grid until you find a cell that is unvisited and has an unvisited neighbor
                if(!grid[i,j].Visited && AreThereVisitedNeighbors(i,j)){
                    currentRow = i;
                    currentColumn = j;
                    grid[currentRow, currentColumn].Visited = true;
                    //Randomly destroy a wall
                    DestroyAdjacentWall();
                    return;
                }
            }
        }
    }

    void DestroyAdjacentWall(){
        bool destroyed = false;

        while (!destroyed){
            int direction = Random.Range(0,4);

            //Check up
            if(direction == 0){
                if(currentRow > 0 && grid[currentRow -1, currentColumn].Visited){

                    Debug.Log("Destroyed down wall." + (currentRow - 1)+ " " + currentColumn);

                    if (grid[currentRow, currentColumn].UpWall){
                        Destroy(grid[currentRow, currentColumn].UpWall);
                    }

                    if (grid[currentRow-1, currentColumn].DownWall){
                        Destroy(grid[currentRow - 1, currentColumn].DownWall);
                    }

                    destroyed = true;
                }
            }

            //Check down
            else if (direction ==1){
                if(currentRow < Rows -1 && grid[currentRow +1, currentColumn].Visited){
                    
                    Debug.Log("Destroyed up wall." + (currentRow + 1)+ " " + currentColumn);

                    if (grid[currentRow, currentColumn].DownWall){
                        Destroy(grid[currentRow, currentColumn].DownWall);
                    }

                    if (grid[currentRow + 1, currentColumn].UpWall){
                        Destroy(grid[currentRow + 1, currentColumn].UpWall);
                    }
                    
                    destroyed = true;
                }
            }

            //Check left
            else if (direction ==2){
                if(currentColumn > 0 && grid[currentRow, currentColumn -1].Visited){
                    
                    Debug.Log("Destroyed right wall." + currentRow + " " + (currentColumn -1));

                    if (grid[currentRow, currentColumn].LeftWall){
                        Destroy(grid[currentRow, currentColumn].LeftWall);
                    }

                    if (grid[currentRow, currentColumn -1].RightWall){
                        Destroy(grid[currentRow, currentColumn -1].RightWall);
                    }

                    
                    destroyed = true;
                }
            }

            //Check right
            else if (direction ==3){
                if(currentColumn < Columns -1 && grid[currentRow, currentColumn + 1].Visited){
                    
                    Debug.Log("Destroyed left wall." + currentRow + " " + (currentColumn -1));
                    
                    if (grid[currentRow, currentColumn].RightWall){
                        Destroy(grid[currentRow, currentColumn].RightWall);
                    }

                    if (grid[currentRow, currentColumn +1].LeftWall){
                        Destroy(grid[currentRow, currentColumn +1].LeftWall);
                    }
                    destroyed = true;
                }
            }
        }
    }

        //Check if there are unvisted neighbors
    bool AreThereUnvistedNeighbors(){
        
        //Check up
        if (IsCellUnvisitedAndWithinBoundaries(currentRow -1, currentColumn)){
            return true;
        }

        //Check down
        if (IsCellUnvisitedAndWithinBoundaries(currentRow +1, currentColumn)){
            return true;
        }

        //Check left
        if (IsCellUnvisitedAndWithinBoundaries(currentRow, currentColumn + 1)){
            return true;
        }

        //Check right
        if (IsCellUnvisitedAndWithinBoundaries(currentRow, currentColumn - 1)){
            return true;
        }

        //Dead-end? Return false. 
        return false;
    }

    public bool AreThereVisitedNeighbors(int row, int column){

        //Check Up
        if( row > 0 && grid[row -1, column].Visited){
            return true;
        }

        //Check down
        if(row < Rows -1 && grid[row +1, column].Visited){
            return true;
        }

        //Check left
        if(column > 0 && grid[row, column -1].Visited){
            return true;
        }

        //Check Right
        if(column < Columns -1 && grid[row, column +1].Visited){
            return true;
        }

        return false;
    }

    //Do Boundary Check and unVisited Check
    bool IsCellUnvisitedAndWithinBoundaries(int row, int column){
        if (row >=0 && row < Rows && column >=0 && column < Columns && !grid[row, column].Visited){
            return true;
        }

        return false;
    }
}
