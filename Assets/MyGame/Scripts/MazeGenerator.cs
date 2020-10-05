using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
	[System.Serializable]
	public class Cell
	{
		public bool visited;
		public GameObject north; //1
		public GameObject east;  //2
		public GameObject west;  //3
		public GameObject south; //4

		public Cell()
		{
			north = null;
			east = null;
			west = null;
			south = null;
		}
	}

	public GameObject wall;
	public float wallLength = 1.0f;
	public int xSize = 5;
	public int ySize = 5;
	private Vector3 initialPos;
	private GameObject wallHolder;
	private Cell[] cells;
	private int currentCell = 0;
	private int totalCells;
	private int visitedCells = 0;
	private bool startedBuilding = false;
	private int currentNeighbour = 0;
	private List<int> lastCells;
	private int backingUp = 0;
	private int wallToBreak = 0;

	// Use this for initialization
	void Start()
	{
		CreateWalls();
	}

	void CreateMaze()
	{
		print("in create maze");
		//GiveMeNeighbour ();
		Debug.Log("visitedCells" + visitedCells);
		Debug.Log("totalCells" + totalCells);
		while (visitedCells < totalCells)
		{
			Debug.Log("startedBuilding " + startedBuilding);
			if (startedBuilding)
			{
				GiveMeNeighbour();
				if (cells[currentNeighbour].visited == false && cells[currentCell].visited == true)
				{
					BreakWall();

					cells[currentNeighbour].visited = true;
					visitedCells++;
					lastCells.Add(currentCell);
					currentCell = currentNeighbour;
					if (lastCells.Count > 0)
					{
						backingUp = lastCells.Count - 1;
					}
				}
			}
			else
			{
				currentCell = Random.Range(0, totalCells);
				cells[currentCell].visited = true;
				visitedCells++;
				startedBuilding = true;
			}
			//Invoke ("CreateMaze", 0.0f);
		}

		Debug.Log("Finished");
	}

	void BreakWall()
	{
		switch (wallToBreak)
		{
			case 1:
				Destroy(cells[currentCell].north);
				Debug.Log("breaknorth in cell " + currentCell);
				break;
			case 2:
				Destroy(cells[currentCell].east);
				Debug.Log("breakeast in cell " + currentCell);
				break;
			case 3:
				Destroy(cells[currentCell].west);
				Debug.Log("breakwest in cell " + currentCell);
				break;
			case 4:
				Destroy(cells[currentCell].south);
				Debug.Log("breaknsouth in cell " + currentCell);
				break;
		}
	}

	void GiveMeNeighbour()
	{

		Debug.Log("in GiveMeNeighbours");
		Debug.Log("currentCell " + currentCell);
		int length = 0;
		int[] neighbours = new int[4];
		int[] connectingWall = new int[4];
		int check = 0;
		check = ((currentCell + 1) / xSize);
		check -= 1;
		check *= xSize;
		check += xSize;
		//west
		if (currentCell + 1 < totalCells && (currentCell + 1) != check)
		{
			if (cells[currentCell + 1].visited == false)
			{
				neighbours[length] = currentCell + 1;
				connectingWall[length] = 3;
				length++;
			}
		}

		//east
		if (currentCell - 1 >= 0 && currentCell != check)
		{
			if (cells[currentCell - 1].visited == false)
			{
				neighbours[length] = currentCell - 1;
				connectingWall[length] = 2;
				length++;
			}
		}

		//north
		if (currentCell + xSize < totalCells)
		{
			if (cells[currentCell + xSize].visited == false)
			{
				neighbours[length] = currentCell + xSize;
				connectingWall[length] = 1;
				length++;
			}
		}

		//south
		if (currentCell - xSize >= 0)
		{
			if (cells[currentCell - xSize].visited == false)
			{
				neighbours[length] = currentCell - xSize;
				connectingWall[length] = 4;
				length++;
			}
		}

		if (length != 0)
		{
			int theChosenOne = Random.Range(0, length);
			currentNeighbour = neighbours[theChosenOne];
			wallToBreak = connectingWall[theChosenOne];
		}
		else
		{
			if (backingUp > 0)
			{
				currentCell = lastCells[backingUp];
				backingUp--;
			}
		}
		for (int i = 0; i < length; i++)
		{
			Debug.Log(neighbours[i]);
		}
	}

	void CreateCells()
	{
		print("in create cells");
		lastCells = new List<int>();
		lastCells.Clear();
		totalCells = xSize * ySize;
		GameObject[] allWalls;
		int children = wallHolder.transform.childCount;
		allWalls = new GameObject[children];
		cells = new Cell[xSize * ySize];
		int eastWestProcess = 0;
		int childProcess = 0;
		int termCount = 0;

		//Gets All the children
		for (int i = 0; i < children; i++)
		{
			allWalls[i] = wallHolder.transform.GetChild(i).gameObject;
		}

		//Assigns walls to the cells
		for (int cellprocess = 0; cellprocess < cells.Length; cellprocess++)
		{
			cells[cellprocess] = new Cell();
			cells[cellprocess].east = allWalls[eastWestProcess];
			cells[cellprocess].south = allWalls[childProcess + (xSize + 1) * ySize];
			if (termCount == xSize)
			{
				eastWestProcess += 2;
				termCount = 0;
			}
			else
			{
				eastWestProcess++;
			}

			termCount++;
			childProcess++;
			cells[cellprocess].west = allWalls[eastWestProcess];
			cells[cellprocess].north = allWalls[(childProcess + (xSize + 1) * ySize) + xSize - 1];
		}

		CreateMaze();
	}

	void CreateWalls()
	{
		print("in create walls");
		wallHolder = new GameObject();
		wallHolder.name = "Maze";

		initialPos = new Vector3((-xSize / 2) + wallLength / 2, 0.0f, (-ySize / 2) + wallLength / 2);
		Vector3 myPos = initialPos;
		GameObject tmpWall;

		//For x Axis
		for (int i = 0; i < ySize; i++)
		{
			for (int j = 0; j <= xSize; j++)
			{
				myPos = new Vector3(initialPos.x + (j * wallLength) - wallLength / 2, 0.0f, initialPos.z + (i * wallLength) - wallLength / 2);
				tmpWall = Instantiate(wall, myPos, Quaternion.identity) as GameObject;
				tmpWall.transform.parent = wallHolder.transform;

			}
		}

		//For y Axis
		for (int i = 0; i <= ySize; i++)
		{
			for (int j = 0; j < xSize; j++)
			{
				myPos = new Vector3(initialPos.x + (j * wallLength), 0.0f, initialPos.z + (i * wallLength) - wallLength);
				tmpWall = Instantiate(wall, myPos, Quaternion.Euler(0.0f, 90.0f, 0.0f)) as GameObject;
				tmpWall.transform.parent = wallHolder.transform;
			}
		}
		CreateCells();
	}
}
