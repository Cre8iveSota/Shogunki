using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private GameObject tochedChara;
    public GameObject TouchedChara { get { return tochedChara; } set { tochedChara = value; Debug.Log($"changed touched chara: {value}"); } }
    private Vector3 currentPos;
    private Vector3 lastPos;
    public Vector3 CurrentPos
    {
        get { return currentPos; }
        set
        {
            lastPos = currentPos;
            currentPos = value;
            Debug.Log($"current mouse Pos: {currentPos}");
        }
    }
    private BiDirectionalDictionary<(int x, int y), int> movablePos = new BiDirectionalDictionary<(int x, int y), int>();
    public BiDirectionalDictionary<(int x, int y), int> MovablePos { get { return movablePos; } set { movablePos = value; Debug.Log($"movable mouse Pos: {movablePos}"); } }
    private Dictionary<(int x, int y), (BoardStatus, bool? isMovablePos)> boardDataBank = new Dictionary<(int x, int y), (BoardStatus, bool? isMovablePos)>(); // if isMovablePos == null, which means depends on your Chara or not
    public IReadOnlyDictionary<(int x, int y), (BoardStatus, bool? isMovablePos)> BoardDataBank => boardDataBank;
    private GameManager gameManager;
    private CharacterModel characterModelForTransfer;
    public CharacterModel CharacterModelForTransfer
    {
        get { return characterModelForTransfer; }
        set
        {
            // last movable pos is gonna be deleted
            if (characterModelForTransfer != null)
            {
                Debug.Log($"expected last pos : x: {(int)lastPos.x},y: {(int)lastPos.y}");
                CalculateMovablePos((int)lastPos.x, (int)lastPos.y, characterModelForTransfer, false);
            }
            characterModelForTransfer = value;
            Debug.Log($"real current pos : x: {(int)currentPos.x},y: {(int)currentPos.y}");
            CalculateMovablePos((int)currentPos.x, (int)currentPos.y, characterModelForTransfer, true);
            CallChangeColorMovablePos();
        }
    }
    [SerializeField] private Vector3 gridUnitSize = new Vector3(2, 0, 2);
    private GameObject[] grids;
    private Dictionary<(int x, int y), GameObject> gridsPosDictionary = new Dictionary<(int x, int y), GameObject>();
    public List<(int, int)> pastMovablePos = new List<(int x, int y)>();

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        if (gameManager == null) Debug.LogError("GameManager not found");
        grids = GameObject.FindGameObjectsWithTag("Grid");
        if (grids == null) Debug.LogError("Grid not found");
        else
        {
            foreach (GameObject grid in grids)
            {
                Vector3 gridPos = grid.transform.position;
                gridsPosDictionary.Add(((int)gridPos.x, (int)gridPos.z), grid);
                Debug.Log($"Dictionary grids got entity- x:{(int)gridPos.x}, z: {(int)gridPos.z}, obj: {grid}");
                grid.GetComponent<BoardInfo>()?.RegisterIntoBoardDataBank(gameManager.IsMasterTurn);
            }
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var item in BoardDataBank)
            {
                Debug.Log($"BoardDataBank data show - Key: {item.Key} - Value: {item.Value}  ");
            }
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            foreach ((int, int) item in MovablePos.Keys)
            {
                if (MovablePos.TryGetByKey(item, out int value))
                {
                    Debug.Log($"BoardDataBank data show - Key: {item} - Value:  {value} ");
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            foreach ((int, int) item in pastMovablePos)
            {
                Debug.Log($"pastMovablePos data show - {item} - ");
            }
        }
    }
    public void AddBoardData(int x, int y, BoardStatus status, bool isMovablePos)
    {
        var key = (x, y);
        boardDataBank[key] = (status, isMovablePos); // 既存のキーなら更新、なければ追加
    }

    public bool RemoveBoardData(int x, int y)
    {
        return boardDataBank.Remove((x, y));
    }
    public bool TryGetBoardData(int x, int y, out BoardStatus status, out bool? isMovablePos)
    {
        Debug.Log($"TrayBoardData search pos data- x: {x}, y: {y}");
        if (boardDataBank.TryGetValue((x, y), out var value))
        {
            status = value.Item1; // I am not sure but for some reason made me use value.Item1 instead of value.status. Now, "value.Item1" is working as what I hoped, so I am leaving this problem
            isMovablePos = value.isMovablePos;
            Debug.Log("TrayBoardData return true");
            return true;
        }
        else
        {
            Debug.Log("TryGetBoardData return false");
            status = default;
            isMovablePos = default;
            return false;
        }
    }

    private void CalculateMovablePos(int x, int y, CharacterModel characterModel, bool isCurrent)
    {
        var movablePositions = characterModel.RangeOfMovementAsRole();

        UpdateMovablePos(movablePositions, isCurrent);
    }

    private void UpdateMovablePos((int x, int y)[] movablePositions, bool isCurrent)
    {
        if (movablePositions != null)
        {
            foreach (var pos in movablePositions)
            {
                int movablePosX = isCurrent ? (int)(currentPos.x + pos.x * gridUnitSize.x) : (int)(lastPos.x + pos.x * gridUnitSize.x);
                int movablePosY = isCurrent ? (int)(currentPos.z + pos.y * gridUnitSize.z) : (int)(lastPos.z + pos.y * gridUnitSize.z);
                Debug.Log($"Movable position: ({movablePosX}, {movablePosY})");
                if (TryGetBoardData(movablePosX, movablePosY, out BoardStatus status, out bool? isMovable))
                {
                    Debug.Log("status :" + status);
                    Debug.Log($"BoardDataBank data show before checking status- x: {movablePosX} - y: {movablePosY}");
                    if (
                        status == BoardStatus.Empty
                        || (status == BoardStatus.ClientCharacterExist && gameManager.IsMasterTurn)
                        || (status == BoardStatus.MasterCharacterExist && !gameManager.IsMasterTurn)
                    )
                    {
                        // ↑ not implemented for these situations that (status == BoardStatus.ClientCharacterExist && !gameManager.IsMasterTurn) and (status == BoardStatus.MasterCharacterExist && gameManager.IsMasterTurn) due to unnecessary so far
                        AddBoardData(movablePosX, movablePosY, BoardStatus.Empty, isCurrent); //  this means some same team meamber on the grid, so this record of isMovable is gonna be false;
                        Debug.Log($"Add boardDataBank as movablePos, x:{movablePosX} , y: {movablePosY}, BoardStatus: {BoardStatus.Empty}, isMovablePos: {isCurrent}");
                    }
                }
            }
        }
        else
        {
            Debug.Log("No movable positions available for this role.");
        }
    }

    private void CallChangeColorMovablePos()
    {
        Debug.Log("boardDataBank count: " + boardDataBank.Count());

        int maxTimeValue = MovablePos.Values.Any() ? MovablePos.Values.Max() : 0;
        Debug.Log($"The latest time is: {maxTimeValue}");

        List<(int x, int y)> movableKeys = boardDataBank
            .Where(entry => entry.Value.isMovablePos == true)
            .Select(entry => entry.Key)
            .ToList();

        Debug.Log("movableKeys count: " + movableKeys.Count());
        foreach ((int x, int y) movableKey in movableKeys)
        {
            Debug.Log("Color target movableKey is" + movableKey);
            if (gridsPosDictionary.ContainsKey(movableKey))
            {
                Debug.Log("gameManager.EntireTime: " + gameManager.EntireTime);

                MovablePos.Add(movableKey, gameManager.EntireTime);
                Debug.Log("Call Grid's Change Color Method");
                gridsPosDictionary[movableKey].GetComponent<BoardInfo>().ColorPos(false, true);
            }
        }

        List<(int x, int y)> unmovableKeys = boardDataBank
            .Where(entry => entry.Value.isMovablePos == false)
            .Select(entry => entry.Key)
            .ToList();

        foreach ((int x, int y) unmovableKey in unmovableKeys)
        {
            Debug.Log("Color target ummovableKey is" + unmovableKey);
            if (gridsPosDictionary.ContainsKey(unmovableKey))
            {
                Debug.Log("Call Grid's Change Color Method for umbovable");
                gridsPosDictionary[unmovableKey].GetComponent<BoardInfo>().ColorPos(false, false);
            }
        }
    }
}
