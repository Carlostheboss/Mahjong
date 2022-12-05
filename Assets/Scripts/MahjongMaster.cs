using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MahjongMaster : MonoBehaviour
{
    //Public variables
    // 1 - Blue, 2- Green, 3 - Orange, 4 - Purple, 5 - Red, 6 - Yellow
    public GameObject[] TileTypes;
    public GameObject WinScreen;
    public GameObject LoseScreen;
    public GameObject PauseScreen;
    public GameObject ReshufleButton;
    public GameObject ParticleEffectPoints;
    public GameObject ParticleEffectSelected;
    public bool timerIsRunning = false;
    public Text timeText;
    public Text PointText;
    public Transform PivotPoint;

    //Private variables
    private GameObject SpawnedParticle;
    private bool Redone;
    private float timeRemaining = 300;
    private int ReshufleCount;
    private bool Check;
    private float DistanceBetweenCubes = 1.1f;
    private int SizeofGrid = 4;
    private int MaxSize = 3;
    private GameObject ClickedObject = null;
    private GameObject ClickedObjectRep = null;
    //Game Board
    public GameObject[,,] Grid = new GameObject[4, 4, 4];
    public GameObject[,,] ReshufledGrid = new GameObject[4, 4, 4];
    private Vector3 PressPoint;
    private Quaternion StartRotation;
    private Ray ray;
    private RaycastHit hit;
    private int Points;

    void Start()
    {
        timerIsRunning = true;
        SpawnCube();
        CheckClickable();  
    }
    private void Update()
    {
        Canyouclick();
        RotateCube();
        CheckTimer();
        if (Input.GetMouseButtonDown(1))
        {
            PauseGame();
        }
        WinLoseCondiitions();
    }

    //Resets the game
    public void ResetGame()
    {
        for (int i = 0; i < 4 * 4 * 4; i++)
        {
            int row = i / (4 * 4);
            int col = i / 4 - row * 4;
            int height = i % 4;

            if (Grid[row, col, height].gameObject.activeSelf)
            {

                Destroy(Grid[row, col, height]);
            }
        }
        CheckClickable();
        ReshufleCount = 0;
        ReshufleButton.SetActive(true);
        timeRemaining = 300;
        Points = 0;
        LoseScreen.SetActive(false);
    }

    //Spawns the game board using a mathematical algorithm that only uses one for loop
    public void SpawnCube()
    {
        for (int i = 0; i < 4 * 4 * 4; i++)
        {
            int row = i / (4 * 4);
            int col = i / 4 - row * 4;
            int height = i % 4;
            Grid[row, col, height] = Instantiate(TileTypes[Random.Range(0, 6)],new Vector3((float)row * DistanceBetweenCubes, (float)col * DistanceBetweenCubes, (float)height * DistanceBetweenCubes), Quaternion.identity , this.transform);
        }
    }

    //Checking win lose conditions
    public void WinLoseCondiitions()
    {
        if(Points == 3200)
        { WinScreen.SetActive(true);
        }
        if(timeRemaining == 0)
        { LoseScreen.SetActive(true); 
        }
    }

    //Checking if tile is clickable
    public void CheckClickable()
    {
        
        for (int i = 0; i < SizeofGrid * SizeofGrid * SizeofGrid; i++)
        {
            int row = i / (SizeofGrid * SizeofGrid);
            int col = i / SizeofGrid - row * SizeofGrid;
            int height = i % SizeofGrid;
            
            if(Grid[row, col, height].gameObject.activeSelf == true)
            {            
                Grid[row, col, height].gameObject.GetComponent<TileLogic>().Clickable = true;

                if (row - 1 >= 0 && row + 1 <= MaxSize)
                {
                    if (Grid[row - 1, col, height].gameObject.GetComponent<MeshRenderer>().enabled == true && Grid[row + 1, col, height].gameObject.GetComponent<MeshRenderer>().enabled == true)
                    {
                        Grid[row, col, height].gameObject.GetComponent<TileLogic>().Clickable = false;
                    }

                }
                if (height - 1 >= 0 && height + 1 <= MaxSize)
                {
                    if (Grid[row, col, height -1 ].gameObject.GetComponent<MeshRenderer>().enabled == true && Grid[row, col, height +1].gameObject.GetComponent<MeshRenderer>().enabled == true)
                    {
                        Grid[row, col, height].gameObject.GetComponent<TileLogic>().Clickable = false;
                    }
                }      
            }
            
        }
    }

    //Checking if tile is clickable after reshufling
    public void CheckClickableReshufled()
    {
        
        for (int i = 0; i < SizeofGrid * SizeofGrid * SizeofGrid; i++)
        {
            int row = i / (SizeofGrid * SizeofGrid);
            int col = i / SizeofGrid - row * SizeofGrid;
            int height = i % SizeofGrid;

            if (ReshufledGrid[row, col, height].gameObject.activeSelf == true)
            {
                ReshufledGrid[row, col, height].gameObject.GetComponent<TileLogic>().Clickable = true;

                if (row - 1 >= 0 && row + 1 <= MaxSize)
                {
                    if (ReshufledGrid[row - 1, col, height].gameObject.GetComponent<MeshRenderer>().enabled == true && ReshufledGrid[row + 1, col, height].gameObject.GetComponent<MeshRenderer>().enabled == true)
                    {
                        ReshufledGrid[row, col, height].gameObject.GetComponent<TileLogic>().Clickable = false;
                    }

                }
                if (height - 1 >= 0 && height + 1 <= MaxSize)
                {
                    if (ReshufledGrid[row, col, height - 1].gameObject.GetComponent<MeshRenderer>().enabled == true && ReshufledGrid[row, col, height + 1].gameObject.GetComponent<MeshRenderer>().enabled == true)
                    {
                        ReshufledGrid[row, col, height].gameObject.GetComponent<TileLogic>().Clickable = false;
                    }
                }
            }

        }
    }

    //Rotates the game board depending on mouse position relative to the screen
    public void RotateCube()
    {
        float SceneWidth;
        SceneWidth = Screen.width;
        if (Input.GetMouseButtonDown(0))
        {
            PressPoint = Input.mousePosition;
            StartRotation = transform.parent.localRotation;
        }
        else if (Input.GetMouseButton(0))
        {
            float CurrentDistanceBetweenMousePositions = (Input.mousePosition - PressPoint).x;
            transform.parent.localRotation = StartRotation * Quaternion.Euler(PivotPoint.up * (CurrentDistanceBetweenMousePositions / SceneWidth) * 360);
        }
    }

    //Runs the game timer
    public void CheckTimer()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
            }
        }
    }

    //Display time in the correct string format
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    //Checks if you can match two tiles together of the same color
    public void Canyouclick()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0) && Check == false && ClickedObject != hit.collider.gameObject && hit.collider.gameObject.GetComponent<TileLogic>().Clickable == true ) 
            {
                SpawnedParticle = Instantiate(ParticleEffectSelected, hit.collider.gameObject.transform.position, transform.parent.localRotation, this.transform);
                ClickedObject = hit.collider.gameObject; 
                Check = true;
            }
            if (Input.GetMouseButtonDown(0) && Check == true && ClickedObject.tag != hit.collider.gameObject.tag && ClickedObject != hit.collider.gameObject && hit.collider.gameObject.GetComponent<TileLogic>().Clickable == true )
            {
                Destroy(SpawnedParticle);
                SpawnedParticle = Instantiate(ParticleEffectSelected, hit.collider.gameObject.transform.position, transform.parent.localRotation, this.transform);
                ClickedObject = hit.collider.gameObject;            
            }
            else if (Input.GetMouseButtonDown(0) && Check == true && ClickedObject != hit.collider.gameObject && ClickedObject.tag == hit.collider.gameObject.tag && hit.collider.gameObject.GetComponent<TileLogic>().Clickable == true)
            {
                Destroy(SpawnedParticle);
                Instantiate(ParticleEffectPoints, hit.collider.gameObject.transform.position, Quaternion.identity, this.transform);
                Points += 100;
                PointText.text = "Points:" + " " + Points.ToString();
                hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = false;
                ClickedObject.GetComponent<MeshRenderer>().enabled = false;
                hit.collider.gameObject.SetActive(false);
                ClickedObject.SetActive(false); 
                if(!Redone)
                CheckClickable();
                else
                CheckClickableReshufled();
                Check = false;
            }        
        }
    }
    //Reshufles the game board
    public void Reshufle()
    {
        Redone = true;
        int RandomNumber = Random.Range(0, 6);
        int RandomNumber2 = Random.Range(0, 6);
        transform.parent.localRotation = Quaternion.identity;
        for (int i = 0; i < 4 * 4 * 4; i++)
        {
            int row = i / (4 * 4);
            int col = i / 4 - row * 4;
            int height = i % 4;
          
            if (!Grid[row, col, height].gameObject.activeSelf)
            {
                ReshufledGrid[row, col, height] = Instantiate(TileTypes[Random.Range(0, 6)], new Vector3((float)row * DistanceBetweenCubes, (float)col * DistanceBetweenCubes, (float)height * DistanceBetweenCubes), Quaternion.identity, this.transform);
                ReshufledGrid[row, col, height].GetComponent<MeshRenderer>().enabled = false;
                ReshufledGrid[row, col, height].SetActive(false);
               
            }
            if (Grid[row, col, height].gameObject.activeSelf)
            {

                Destroy(Grid[row, col, height]);
                if (ReshufleCount > 1 && ReshufleCount < 3)
                    ReshufledGrid[row, col, height] = Instantiate(TileTypes[RandomNumber], new Vector3((float)row * DistanceBetweenCubes, (float)col * DistanceBetweenCubes, (float)height * DistanceBetweenCubes), Quaternion.identity, this.transform);
                else if (ReshufleCount <= 1)
                    ReshufledGrid[row, col, height] = Instantiate(TileTypes[RandomNumber2], new Vector3((float)row * DistanceBetweenCubes, (float)col * DistanceBetweenCubes, (float)height * DistanceBetweenCubes), Quaternion.identity, this.transform);


                if (ReshufleCount >= 3)
                {
                    ReshufledGrid[row, col, height] = Instantiate(TileTypes[Random.Range(0, 6)], new Vector3((float)row * DistanceBetweenCubes, (float)col * DistanceBetweenCubes, (float)height * DistanceBetweenCubes), Quaternion.identity, this.transform);
                }
                ReshufleCount++;
            }
           
        }
        CheckClickableReshufled();
       
        ReshufleButton.SetActive(false);
    }
    //Pauses the game
    public void PauseGame()
    {
        Time.timeScale = 0;
        PauseScreen.SetActive(true);
    }
    //Unpauses the game
    public void ResumeGame()
    {
        PauseScreen.SetActive(false);
        Time.timeScale = 1;
    }
}
