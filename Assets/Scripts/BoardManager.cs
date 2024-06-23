using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            foreach (((int x, int y), GameObject obj) in gridsPosDictionary)
            {
                obj.GetComponent<BoardInfo>().RegisterIntoBoardDataBank(!gameManager.IsMasterTurn); // use next turn
            }
        }
    }
    private List<(int x, int y)> movablePos = new List<(int x, int y)>();
    public List<(int x, int y)> MovablePos { get { return movablePos; } set { movablePos = value; Debug.Log($"movable mouse Pos: {movablePos}"); } }
    public List<(int, int)> pastMovablePos = new List<(int x, int y)>();
    private (int x, int y) expectedMovingPos;
    public (int x, int y) ExpectedMovingPos
    {
        get { return expectedMovingPos; }
        set
        {
            expectedMovingPos = value;
            Debug.Log($"Start finding the pos status of:({expectedMovingPos.x},{expectedMovingPos.y})");
            // check there is oppornent beofre chara move
            if (TryGetBoardData(expectedMovingPos.x, expectedMovingPos.y, out BoardStatus status, out bool? isMovablePos))
            {
                Debug.Log($"Start finding of attacking target gameManager.IsMasterTurn: {gameManager.IsMasterTurn}, status: {status}");
                // before define expected character, character next moving pos is detected, that is why you should check there is existing your own character
                if (gameManager.IsMasterTurn && status == BoardStatus.MasterCharacterExist)
                {
                    GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("ClientCharacter");
                    foreach (GameObject gameObject in gameObjects)
                    {
                        CharacterModel character = gameObject.GetComponent<CharacterModel>();
                        if (character.GetCurrentPos() == (expectedMovingPos.x, expectedMovingPos.y))
                        {
                            AttackingTarget = character;
                            break;
                        }
                        else
                        {
                            Debug.Log("Attacking target is not found");
                            AttackingTarget = null;
                        }
                    }
                }
                else if (!gameManager.IsMasterTurn && status == BoardStatus.ClientCharacterExist)
                {
                    GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("MasterCharacter");
                    foreach (GameObject gameObject in gameObjects)
                    {
                        CharacterModel character = gameObject.GetComponent<CharacterModel>();
                        if (character.GetCurrentPos() == (expectedMovingPos.x, expectedMovingPos.y))
                        {
                            AttackingTarget = character;
                            break;
                        }
                        else
                        {
                            Debug.Log("Attacking target is not found");
                            AttackingTarget = null;
                        }
                    }
                }
            }
            characterModelForTransfer.Move(expectedMovingPos.x, expectedMovingPos.y);
            ResetAllGridAsMovableIsFalse();
            CallChangeColorMovablePos();
            foreach (((int x, int y), GameObject obj) in gridsPosDictionary)
            {
                obj.GetComponent<BoardInfo>().RegisterIntoBoardDataBank(!gameManager.IsMasterTurn); // use next turn
            }
        }
    }
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
                Debug.Log($"expected last pos : x: {(int)lastPos.x},y: {(int)lastPos.z}");
                CalculateMovablePos((int)lastPos.x, (int)lastPos.z, characterModelForTransfer, false);
            }
            characterModelForTransfer = value;
            Debug.Log($"real current pos : x: {(int)currentPos.x},y: {(int)currentPos.z}");
            CalculateMovablePos((int)currentPos.x, (int)currentPos.z, characterModelForTransfer, true);
            CallChangeColorMovablePos();
        }
    }
    private CharacterModel attackingTarget;
    public CharacterModel AttackingTarget
    {
        get { return attackingTarget; }
        set
        {
            attackingTarget = value;
            if (attackingTarget != null)
            {
                Debug.Log($"attacking target is found: {attackingTarget.Id}");
            }
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
            foreach ((int, int) item in MovablePos)
            {
                Debug.Log($"Movable data show - : {item}");
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
    public void AddBoardData(int x, int y, BoardStatus status, bool? isMovablePos)
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
        if (boardDataBank.TryGetValue((x, y), out var value))
        {
            Debug.Log($"TrayBoardData search pos data- x: {x}, y: {y}, status: {value.Item1}, isMovablePos: {value.isMovablePos}");
            status = value.Item1; // I am not sure but for some reason made me use value.Item1 instead of value.status. Now, "value.Item1" is working as what I hoped, so I am leaving this problem
            isMovablePos = value.isMovablePos;
            Debug.Log("TrayBoardData return true");
            return true;
        }
        else
        {
            Debug.Log($"TrayBoardData search pos data- x: {x}, y: {y}, but TryGetBoardData return false");
            status = default;
            isMovablePos = default;
            return false;
        }
    }

    private void CalculateMovablePos(int x, int y, CharacterModel characterModel, bool isCurrent)
    {
        var possibleMovablePos = characterModel.RoleAbility();

        UpdateMovablePos(possibleMovablePos, isCurrent);
    }

    private void UpdateMovablePos((int x, int y)[] possibleMovablePos, bool isCurrent)
    {
        ResetAllGridAsMovableIsFalse();
        List<(int, int)> unMovableList = new List<(int, int)>();
        if (possibleMovablePos != null)
        {
            foreach (var pos in possibleMovablePos)
            {
                int movablePosX = isCurrent ? (int)(currentPos.x + pos.x * gridUnitSize.x) : (int)(lastPos.x + pos.x * gridUnitSize.x);
                int movablePosY = isCurrent ? (int)(currentPos.z + pos.y * gridUnitSize.z) : (int)(lastPos.z + pos.y * gridUnitSize.z);
                Debug.Log($"Movable position: ({movablePosX}, {movablePosY})");
                if (TryGetBoardData(movablePosX, movablePosY, out BoardStatus status, out bool? isMovable))
                {
                    Debug.Log($"BoardDataBank data show before checking status- x: {movablePosX} - y: {movablePosY}, status: {status}, isMovable: {isMovable}");
                    if (
                        status == BoardStatus.Empty
                    || (status == BoardStatus.ClientCharacterExist && gameManager.IsMasterTurn)
                    || (status == BoardStatus.MasterCharacterExist && !gameManager.IsMasterTurn)
                    )
                    {
                        // ↑ not implemented for these situations that (status == BoardStatus.ClientCharacterExist && !gameManager.IsMasterTurn) and (status == BoardStatus.MasterCharacterExist && gameManager.IsMasterTurn) due to unnecessary so far
                        AddBoardData(movablePosX, movablePosY, status, isCurrent); //  this means some same team meamber on the grid, so this record of isMovable is gonna be false;
                        Debug.Log($"Add boardDataBank as movablePos, x:{movablePosX} , y: {movablePosY}, BoardStatus: {status}, isMovablePos: {isCurrent}");
                    }
                    else if (
                        (status == BoardStatus.ClientCharacterExist && !gameManager.IsMasterTurn)
                    || (status == BoardStatus.MasterCharacterExist && gameManager.IsMasterTurn)
                    )
                    {
                        unMovableList.Add((movablePosX, movablePosY));
                        Debug.Log($"unMovableList add :({movablePosX},{movablePosY})");
                    }
                }
            }


            // Check the movablePos is continious
            List<(int x, int y)> movablePosOnBoard = new List<(int, int)>();
            if (characterModelForTransfer.Role == Role.KakugyoId || characterModelForTransfer.Role == Role.NariKakuId)
            {
                for (int posXposY = 1; posXposY < 9; posXposY++)
                {
                    if (TryGetBoardData((int)(currentPos.x + gridUnitSize.x * posXposY), (int)(currentPos.z + gridUnitSize.z * posXposY), out BoardStatus st, out bool? ismovablePos))
                    {
                        Debug.Log($"Status of Board: ({(int)(currentPos.x + gridUnitSize.x * posXposY)},{(int)(currentPos.z + gridUnitSize.z * posXposY)}), st: {st}, bool: {ismovablePos} ");
                        if ((st == BoardStatus.ClientCharacterExist && gameManager.IsMasterTurn)
                         || (st == BoardStatus.MasterCharacterExist && !gameManager.IsMasterTurn)
                         )
                        {
                            movablePosOnBoard.Add(((int)(currentPos.x + gridUnitSize.x * posXposY), (int)(currentPos.z + gridUnitSize.z * posXposY)));
                            break;
                        }
                        else if ((st == BoardStatus.ClientCharacterExist && !gameManager.IsMasterTurn)
                         || (st == BoardStatus.MasterCharacterExist && gameManager.IsMasterTurn))
                        {
                            break;
                        }
                        else if (st == BoardStatus.Empty)
                        {
                            movablePosOnBoard.Add(((int)(currentPos.x + gridUnitSize.x * posXposY), (int)(currentPos.z + gridUnitSize.z * posXposY)));
                        }
                    }
                }
                for (int posXnegaY = 1; posXnegaY < 9; posXnegaY++)
                {
                    if (TryGetBoardData((int)(currentPos.x + gridUnitSize.x * posXnegaY), (int)(currentPos.z - gridUnitSize.z * posXnegaY), out BoardStatus st, out bool? ismovablePos))
                    {
                        Debug.Log($"Status of Board: ({(int)(currentPos.x + gridUnitSize.x * posXnegaY)},{(int)(currentPos.z - gridUnitSize.z * posXnegaY)}), st: {st}, bool: {ismovablePos} ");
                        if ((st == BoardStatus.ClientCharacterExist && gameManager.IsMasterTurn)
                         || (st == BoardStatus.MasterCharacterExist && !gameManager.IsMasterTurn)
                         )
                        {
                            movablePosOnBoard.Add(((int)(currentPos.x + gridUnitSize.x * posXnegaY), (int)(currentPos.z - gridUnitSize.z * posXnegaY)));
                            break;
                        }
                        else if ((st == BoardStatus.ClientCharacterExist && !gameManager.IsMasterTurn)
                         || (st == BoardStatus.MasterCharacterExist && gameManager.IsMasterTurn))
                        {
                            break;
                        }
                        else if (st == BoardStatus.Empty)
                        {
                            movablePosOnBoard.Add(((int)(currentPos.x + gridUnitSize.x * posXnegaY), (int)(currentPos.z - gridUnitSize.z * posXnegaY)));
                        }
                    }
                }
                for (int negaXposY = 1; negaXposY < 9; negaXposY++)
                {
                    if (TryGetBoardData((int)(currentPos.x - gridUnitSize.x * negaXposY), (int)(currentPos.z + gridUnitSize.z * negaXposY), out BoardStatus st, out bool? ismovablePos))
                    {
                        Debug.Log($"Status of Board: ({(int)(currentPos.x - gridUnitSize.x * negaXposY)},{(int)(currentPos.z + gridUnitSize.z * negaXposY)}), st: {st}, bool: {ismovablePos} ");
                        if (
                          (st == BoardStatus.ClientCharacterExist && gameManager.IsMasterTurn)
                         || (st == BoardStatus.MasterCharacterExist && !gameManager.IsMasterTurn
                         )
                         )
                        {
                            movablePosOnBoard.Add(((int)(currentPos.x - gridUnitSize.x * negaXposY), (int)(currentPos.z + gridUnitSize.z * negaXposY)));
                            break;
                        }
                        else if (
                          (st == BoardStatus.ClientCharacterExist && !gameManager.IsMasterTurn)
                         || (st == BoardStatus.MasterCharacterExist && gameManager.IsMasterTurn))
                        {
                            break;
                        }
                        else if (st == BoardStatus.Empty)
                        {
                            movablePosOnBoard.Add(((int)(currentPos.x + -gridUnitSize.x * negaXposY), (int)(currentPos.z + gridUnitSize.z * negaXposY)));
                        }
                    }
                }
                for (int negaXnegaY = -1; negaXnegaY > -9; negaXnegaY--)
                {
                    if (TryGetBoardData((int)(currentPos.x + gridUnitSize.x * negaXnegaY), (int)(currentPos.z + gridUnitSize.z * negaXnegaY), out BoardStatus st, out bool? ismovablePos))
                    {
                        Debug.Log($"Status of Board: ({(int)(currentPos.x + gridUnitSize.x * negaXnegaY)},{(int)(currentPos.z + gridUnitSize.z * negaXnegaY)}), st: {st}, bool: {ismovablePos} ");
                        if ((st == BoardStatus.ClientCharacterExist && gameManager.IsMasterTurn)
                         || (st == BoardStatus.MasterCharacterExist && !gameManager.IsMasterTurn)
                         )
                        {
                            movablePosOnBoard.Add(((int)(currentPos.x + gridUnitSize.x * negaXnegaY), (int)(currentPos.z + gridUnitSize.z * negaXnegaY)));
                            break;
                        }
                        else if ((st == BoardStatus.ClientCharacterExist && !gameManager.IsMasterTurn)
                         || (st == BoardStatus.MasterCharacterExist && gameManager.IsMasterTurn))
                        {
                            break;
                        }
                        else if (st == BoardStatus.Empty)
                        {
                            movablePosOnBoard.Add(((int)(currentPos.x + gridUnitSize.x * negaXnegaY), (int)(currentPos.z + gridUnitSize.z * negaXnegaY)));
                        }
                    }
                }

                if (characterModelForTransfer.Role == Role.NariKakuId)
                {
                    if (TryGetBoardData((int)(currentPos.x + gridUnitSize.x * 1), (int)(currentPos.z + gridUnitSize.z * 0), out BoardStatus st, out bool? ismovablePos))
                    {
                        if ((st == BoardStatus.ClientCharacterExist && gameManager.IsMasterTurn)
                         || (st == BoardStatus.MasterCharacterExist && !gameManager.IsMasterTurn)
                         || st == BoardStatus.Empty)
                        {
                            movablePosOnBoard.Add(((int)(currentPos.x + gridUnitSize.x * 1), (int)(currentPos.z + gridUnitSize.z * 0)));
                        }
                    }
                    if (TryGetBoardData((int)(currentPos.x + gridUnitSize.x * -1), (int)(currentPos.z + gridUnitSize.z * 0), out BoardStatus st1, out bool? ismovablePos1))
                    {
                        if ((st1 == BoardStatus.ClientCharacterExist && gameManager.IsMasterTurn)
                         || (st1 == BoardStatus.MasterCharacterExist && !gameManager.IsMasterTurn)
                         || st1 == BoardStatus.Empty)
                        {
                            movablePosOnBoard.Add(((int)(currentPos.x + gridUnitSize.x * -1), (int)(currentPos.z + gridUnitSize.z * 0)));
                        }
                    }
                    if (TryGetBoardData((int)(currentPos.x + gridUnitSize.x * 0), (int)(currentPos.z + gridUnitSize.z * 1), out BoardStatus st2, out bool? ismovablePos2))
                    {
                        if ((st2 == BoardStatus.ClientCharacterExist && gameManager.IsMasterTurn)
                         || (st2 == BoardStatus.MasterCharacterExist && !gameManager.IsMasterTurn)
                         || st2 == BoardStatus.Empty)
                        {
                            movablePosOnBoard.Add(((int)(currentPos.x + gridUnitSize.x * 0), (int)(currentPos.z + gridUnitSize.z * 1)));
                        }
                    }
                    if (TryGetBoardData((int)(currentPos.x + gridUnitSize.x * 0), (int)(currentPos.z + gridUnitSize.z * -1), out BoardStatus st3, out bool? ismovablePos3))
                    {
                        if ((st3 == BoardStatus.ClientCharacterExist && gameManager.IsMasterTurn)
                         || (st3 == BoardStatus.MasterCharacterExist && !gameManager.IsMasterTurn)
                         || st3 == BoardStatus.Empty)
                        {
                            movablePosOnBoard.Add(((int)(currentPos.x + gridUnitSize.x * 0), (int)(currentPos.z + gridUnitSize.z * -1)));
                        }
                    }
                }

                List<(int x, int y)> movableKeysOld = boardDataBank
              .Where(entry => entry.Value.isMovablePos == true)
              .Select(entry => entry.Key)
              .ToList();
                foreach ((int x, int y) pos in movableKeysOld)
                {
                    if (TryGetBoardData(pos.x, pos.y, out BoardStatus st, out bool? ismovablePos))
                    {
                        AddBoardData(pos.x, pos.y, st, false);
                    }
                }
                foreach ((int x, int y) pos in movablePosOnBoard)
                {
                    if (TryGetBoardData(pos.x, pos.y, out BoardStatus st, out bool? ismovablePos))
                    {
                        AddBoardData(pos.x, pos.y, st, true);
                    }
                }
            }
            if (characterModelForTransfer.Role == Role.HishaId || characterModelForTransfer.Role == Role.NariHishaId)
            {
                for (int posX = 1; posX < 9; posX++)
                {
                    if (TryGetBoardData((int)(currentPos.x + gridUnitSize.x * posX), (int)currentPos.z, out BoardStatus st, out bool? ismovablePos))
                    {
                        Debug.Log($"Status of Board: ({(int)(currentPos.x + gridUnitSize.x * posX)},{(int)currentPos.z}), st: {st}, bool: {ismovablePos} ");
                        if ((st == BoardStatus.ClientCharacterExist && gameManager.IsMasterTurn)
                         || (st == BoardStatus.MasterCharacterExist && !gameManager.IsMasterTurn)
                         )
                        {
                            movablePosOnBoard.Add(((int)(currentPos.x + gridUnitSize.x * posX), (int)currentPos.z));
                            break;
                        }
                        else if ((st == BoardStatus.ClientCharacterExist && !gameManager.IsMasterTurn)
                         || (st == BoardStatus.MasterCharacterExist && gameManager.IsMasterTurn))
                        {
                            break;
                        }
                        else if (st == BoardStatus.Empty)
                        {
                            movablePosOnBoard.Add(((int)(currentPos.x + gridUnitSize.x * posX), (int)currentPos.z));
                        }
                    }
                }
                for (int posY = 1; posY < 9; posY++)
                {
                    if (TryGetBoardData((int)currentPos.x, (int)(currentPos.z + gridUnitSize.z * posY), out BoardStatus st, out bool? ismovablePos))
                    {
                        Debug.Log($"Status of Board: ({(int)currentPos.x},{(int)(currentPos.z + gridUnitSize.z * posY)}), st: {st}, bool: {ismovablePos} ");
                        if ((st == BoardStatus.ClientCharacterExist && gameManager.IsMasterTurn)
                         || (st == BoardStatus.MasterCharacterExist && !gameManager.IsMasterTurn)
                         )
                        {
                            movablePosOnBoard.Add(((int)currentPos.x, (int)(currentPos.z + gridUnitSize.z * posY)));
                            break;
                        }
                        else if ((st == BoardStatus.ClientCharacterExist && !gameManager.IsMasterTurn)
                         || (st == BoardStatus.MasterCharacterExist && gameManager.IsMasterTurn))
                        {
                            break;
                        }
                        else if (st == BoardStatus.Empty)
                        {
                            movablePosOnBoard.Add(((int)currentPos.x, (int)(currentPos.z + gridUnitSize.z * posY)));
                        }
                    }
                }
                for (int negaX = -1; negaX > -9; negaX--)
                {
                    if (TryGetBoardData((int)(currentPos.x + gridUnitSize.x * negaX), (int)currentPos.z, out BoardStatus st, out bool? ismovablePos))
                    {
                        Debug.Log($"Status of Board: ({(int)(currentPos.x + gridUnitSize.x * negaX)},{(int)currentPos.z}), st: {st}, bool: {ismovablePos} ");
                        if (
                          (st == BoardStatus.ClientCharacterExist && gameManager.IsMasterTurn)
                         || (st == BoardStatus.MasterCharacterExist && !gameManager.IsMasterTurn
                         )
                         )
                        {
                            movablePosOnBoard.Add(((int)(currentPos.x + gridUnitSize.x * negaX), (int)currentPos.z));
                            break;
                        }
                        else if (
                          (st == BoardStatus.ClientCharacterExist && !gameManager.IsMasterTurn)
                         || (st == BoardStatus.MasterCharacterExist && gameManager.IsMasterTurn))
                        {
                            break;
                        }
                        else if (st == BoardStatus.Empty)
                        {
                            movablePosOnBoard.Add(((int)(currentPos.x + gridUnitSize.x * negaX), (int)currentPos.z));
                        }
                    }
                }
                for (int negaY = -1; negaY > -9; negaY--)
                {
                    if (TryGetBoardData((int)currentPos.x, (int)(currentPos.z + gridUnitSize.z * negaY), out BoardStatus st, out bool? ismovablePos))
                    {
                        Debug.Log($"Status of Board: ({(int)currentPos.x},{(int)(currentPos.z + gridUnitSize.z * negaY)}), st: {st}, bool: {ismovablePos} ");
                        if ((st == BoardStatus.ClientCharacterExist && gameManager.IsMasterTurn)
                         || (st == BoardStatus.MasterCharacterExist && !gameManager.IsMasterTurn)
                         )
                        {
                            movablePosOnBoard.Add(((int)currentPos.x, (int)(currentPos.z + gridUnitSize.z * negaY)));
                            break;
                        }
                        else if ((st == BoardStatus.ClientCharacterExist && !gameManager.IsMasterTurn)
                         || (st == BoardStatus.MasterCharacterExist && gameManager.IsMasterTurn))
                        {
                            break;
                        }
                        else if (st == BoardStatus.Empty)
                        {
                            movablePosOnBoard.Add(((int)currentPos.x, (int)(currentPos.z + gridUnitSize.z * negaY)));
                        }
                    }
                }

                if (characterModelForTransfer.Role == Role.NariHishaId)
                {
                    if (TryGetBoardData((int)(currentPos.x + gridUnitSize.x * 1), (int)(currentPos.z + gridUnitSize.z * 1), out BoardStatus st, out bool? ismovablePos))
                    {
                        if ((st == BoardStatus.ClientCharacterExist && gameManager.IsMasterTurn)
                         || (st == BoardStatus.MasterCharacterExist && !gameManager.IsMasterTurn)
                         || st == BoardStatus.Empty)
                        {
                            movablePosOnBoard.Add(((int)(currentPos.x + gridUnitSize.x * 1), (int)(currentPos.z + gridUnitSize.z * 1)));
                        }
                    }
                    if (TryGetBoardData((int)(currentPos.x + gridUnitSize.x * -1), (int)(currentPos.z + gridUnitSize.z * -1), out BoardStatus st1, out bool? ismovablePos1))
                    {
                        if ((st1 == BoardStatus.ClientCharacterExist && gameManager.IsMasterTurn)
                         || (st1 == BoardStatus.MasterCharacterExist && !gameManager.IsMasterTurn)
                         || st1 == BoardStatus.Empty)
                        {
                            movablePosOnBoard.Add(((int)(currentPos.x + gridUnitSize.x * -1), (int)(currentPos.z + gridUnitSize.z * -1)));
                        }
                    }
                    if (TryGetBoardData((int)(currentPos.x + gridUnitSize.x * 1), (int)(currentPos.z + gridUnitSize.z * -1), out BoardStatus st2, out bool? ismovablePos2))
                    {
                        if ((st2 == BoardStatus.ClientCharacterExist && gameManager.IsMasterTurn)
                         || (st2 == BoardStatus.MasterCharacterExist && !gameManager.IsMasterTurn)
                         || st2 == BoardStatus.Empty)
                        {
                            movablePosOnBoard.Add(((int)(currentPos.x + gridUnitSize.x * 1), (int)(currentPos.z + gridUnitSize.z * -1)));
                        }
                    }
                    if (TryGetBoardData((int)(currentPos.x + gridUnitSize.x * -1), (int)(currentPos.z + gridUnitSize.z * 1), out BoardStatus st3, out bool? ismovablePos3))
                    {
                        if ((st3 == BoardStatus.ClientCharacterExist && gameManager.IsMasterTurn)
                         || (st3 == BoardStatus.MasterCharacterExist && !gameManager.IsMasterTurn)
                         || st3 == BoardStatus.Empty)
                        {
                            movablePosOnBoard.Add(((int)(currentPos.x + gridUnitSize.x * -1), (int)(currentPos.z + gridUnitSize.z * 1)));
                        }
                    }
                }

                List<(int x, int y)> movableKeysOld = boardDataBank
              .Where(entry => entry.Value.isMovablePos == true)
              .Select(entry => entry.Key)
              .ToList();
                foreach ((int x, int y) pos in movableKeysOld)
                {
                    if (TryGetBoardData(pos.x, pos.y, out BoardStatus st, out bool? ismovablePos))
                    {
                        AddBoardData(pos.x, pos.y, st, false);
                    }
                }
                foreach ((int x, int y) pos in movablePosOnBoard)
                {
                    if (TryGetBoardData(pos.x, pos.y, out BoardStatus st, out bool? ismovablePos))
                    {
                        AddBoardData(pos.x, pos.y, st, true);
                    }
                }
            }
            if (characterModelForTransfer.Role == Role.KyoshaId)
            {
                if (!gameManager.IsMasterTurn)
                {
                    for (int posY = 1; posY < 9; posY++)
                    {
                        if (TryGetBoardData((int)currentPos.x, (int)(currentPos.z + gridUnitSize.z * posY), out BoardStatus st, out bool? ismovablePos))
                        {
                            Debug.Log($"Status of Board: ({(int)currentPos.x},{(int)(currentPos.z + gridUnitSize.z * posY)}), st: {st}, bool: {ismovablePos} ");
                            if ((st == BoardStatus.ClientCharacterExist && gameManager.IsMasterTurn)
                             || (st == BoardStatus.MasterCharacterExist && !gameManager.IsMasterTurn)
                             )
                            {
                                movablePosOnBoard.Add(((int)currentPos.x, (int)(currentPos.z + gridUnitSize.z * posY)));
                                break;
                            }
                            else if ((st == BoardStatus.ClientCharacterExist && !gameManager.IsMasterTurn)
                             || (st == BoardStatus.MasterCharacterExist && gameManager.IsMasterTurn))
                            {
                                break;
                            }
                            else if (st == BoardStatus.Empty)
                            {
                                movablePosOnBoard.Add(((int)currentPos.x, (int)(currentPos.z + gridUnitSize.z * posY)));
                            }
                        }
                    }
                }
                if (gameManager.IsMasterTurn)
                {
                    for (int posY = -1; posY > -9; posY--)
                    {
                        if (TryGetBoardData((int)currentPos.x, (int)(currentPos.z + gridUnitSize.z * posY), out BoardStatus st, out bool? ismovablePos))
                        {
                            Debug.Log($"Status of Board: ({(int)currentPos.x},{(int)(currentPos.z + gridUnitSize.z * posY)}), st: {st}, bool: {ismovablePos} ");
                            if ((st == BoardStatus.ClientCharacterExist && gameManager.IsMasterTurn)
                             || (st == BoardStatus.MasterCharacterExist && !gameManager.IsMasterTurn)
                             )
                            {
                                movablePosOnBoard.Add(((int)currentPos.x, (int)(currentPos.z + gridUnitSize.z * posY)));
                                break;
                            }
                            else if ((st == BoardStatus.ClientCharacterExist && !gameManager.IsMasterTurn)
                             || (st == BoardStatus.MasterCharacterExist && gameManager.IsMasterTurn))
                            {
                                break;
                            }
                            else if (st == BoardStatus.Empty)
                            {
                                movablePosOnBoard.Add(((int)currentPos.x, (int)(currentPos.z + gridUnitSize.z * posY)));
                            }
                        }
                    }
                }
                List<(int x, int y)> movableKeysOld = boardDataBank
              .Where(entry => entry.Value.isMovablePos == true)
              .Select(entry => entry.Key)
              .ToList();
                foreach ((int x, int y) pos in movableKeysOld)
                {
                    if (TryGetBoardData(pos.x, pos.y, out BoardStatus st, out bool? ismovablePos))
                    {
                        AddBoardData(pos.x, pos.y, st, false);
                    }
                }
                foreach ((int x, int y) pos in movablePosOnBoard)
                {
                    if (TryGetBoardData(pos.x, pos.y, out BoardStatus st, out bool? ismovablePos))
                    {
                        AddBoardData(pos.x, pos.y, st, true);
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
                MovablePos.Add(movableKey);
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

    private void ResetAllGridAsMovableIsFalse()
    {
        foreach (((int x, int y), GameObject obj) in gridsPosDictionary)
        {
            Debug.Log("Call Grid's Change Color Method for umbovable");
            if (TryGetBoardData(x, y, out BoardStatus st, out bool? ismovablePos))
            {
                AddBoardData(x, y, st, false);
            }
        }
    }
}
