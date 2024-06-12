using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private GameObject tochedChara;
    public GameObject TouchedChara { get { return tochedChara; } set { tochedChara = value; Debug.Log($"changed touched chara: {value}"); } }
    private Dictionary<(int x, int y), BoardStatus> boardDataBank = new Dictionary<(int x, int y), BoardStatus>();
    private Transform currentPos;
    public Transform CurrentPos { get { return currentPos; } set { currentPos = value; Debug.Log($"current mouse Pos: {currentPos.position}"); } }

    public IReadOnlyDictionary<(int x, int y), BoardStatus> BoardDataBank => boardDataBank;

    public void AddBoardData(int x, int y, BoardStatus status)
    {
        var key = (x, y);
        boardDataBank[key] = status; // 既存のキーなら更新、なければ追加
    }

    public bool RemoveBoardData(int x, int y)
    {
        return boardDataBank.Remove((x, y));
    }

    public bool TryGetBoardData(int x, int y, out BoardStatus status)
    {
        return boardDataBank.TryGetValue((x, y), out status);
    }
}
