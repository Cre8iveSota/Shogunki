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
    public Vector3 CurrentPos
    {
        get { return currentPos; }
        set
        {
            currentPos = value;
            Debug.Log($"current mouse Pos: {currentPos}");
        }
    }
    private Vector3 movablePos;
    public Vector3 MovablePos { get { return movablePos; } set { movablePos = value; Debug.Log($"movable mouse Pos: {movablePos}"); } }
    private Dictionary<(int x, int y), (BoardStatus, bool? isMovablePos)> boardDataBank = new Dictionary<(int x, int y), (BoardStatus, bool? isMovablePos)>(); // if isMovablePos == null, which means depends on your Chara or not
    public IReadOnlyDictionary<(int x, int y), (BoardStatus, bool? isMovablePos)> BoardDataBank => boardDataBank;
    private GameManager gameManager;
    private CharacterModel characterModelForTransfer;
    public CharacterModel CharacterModelForTransfer
    {
        get { return characterModelForTransfer; }
        set
        {
            characterModelForTransfer = value;
            CalculateMovablePos((int)currentPos.x, (int)currentPos.y, characterModelForTransfer);
            CallChangeColorMovablePos();
        }
    }
    [SerializeField] private Vector3 gridUnitSize = new Vector3(2, 0, 2);
    private GameObject[] grids;
    private Dictionary<(int x, int y), GameObject> gridsPosDictionary = new Dictionary<(int x, int y), GameObject>();

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

    private void CalculateMovablePos(int x, int y, CharacterModel characterModel)
    {
        var movablePositions = characterModel.RangeOfMovementAsRole();

        if (movablePositions != null)
        {
            foreach (var pos in movablePositions)
            {
                int movablePosX = (int)(currentPos.x + pos.x * gridUnitSize.x);
                int movablePosY = (int)(currentPos.z + pos.y * gridUnitSize.z);
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
                        AddBoardData(movablePosX, movablePosY, BoardStatus.Empty, true); //  this means some same team meamber on the grid, so this record of isMovable is gonna be false;
                        Debug.Log($"Add boardDataBank as movablePos, x:{movablePosX} , y: {movablePosY}, BoardStatus: {BoardStatus.Empty}, isMovablePos: {true}");
                    }
                }
                else
                {
                    if (
                        status == BoardStatus.Empty
                        || (status == BoardStatus.ClientCharacterExist && gameManager.IsMasterTurn)
                        || (status == BoardStatus.MasterCharacterExist && !gameManager.IsMasterTurn)
                        )
                    {
                        // ↑ not implemented for these situations that (status == BoardStatus.ClientCharacterExist && !gameManager.IsMasterTurn) and (status == BoardStatus.MasterCharacterExist && gameManager.IsMasterTurn) due to unnecessary so far
                        AddBoardData(movablePosX, movablePosY, BoardStatus.Empty, true); //  this means some same team meamber on the grid, so this record of isMovable is gonna be false;
                        Debug.Log($"Add boardDataBank as movablePos, x:{movablePosX} , y: {movablePosY}, BoardStatus: {BoardStatus.Empty}, isMovablePos: {true}");
                    }
                    else
                    {
                        Debug.Log("SOMETHING WRONG");
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
        Debug.Log("start CallChangeColorMovablePos()");
        Debug.Log("boardDataBank count: " + boardDataBank.Count());
        // isMovablePos == true  にするところを確認する
        List<(int x, int y)> movableKeys = boardDataBank
        .Where(entry => entry.Value.isMovablePos == true)
        .Select(entry => entry.Key)
        .ToList();
        Debug.Log("movableKeys count: " + movableKeys.Count());
        foreach ((int, int) movableKey in movableKeys)
        {
            Debug.Log("Color target movableKey is" + movableKey);
            if (gridsPosDictionary.ContainsKey(movableKey))
            {
                MovablePos = new Vector3(movableKey.Item1, 0, movableKey.Item2);
                Debug.Log("Call Grid's Change Color Method");
                gridsPosDictionary[movableKey].GetComponent<BoardInfo>().ColorPos(false, true);
            }
        }
    }
}
