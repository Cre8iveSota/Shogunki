using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterModel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string id;
    public string Id { get { return id; } private set { id = value; } }
    [SerializeField] private bool hasMasterOwnership;
    public bool HasMasterOwnership { get { return hasMasterOwnership; } set { hasMasterOwnership = value; } }
    bool isMasterPlayerTapped = false;
    bool isClientPlayerTapped = false;
    bool isAlive = true;
    Vector3 currentPos;

    BoardManager boardManager;
    BoardInfo boardInfo;
    [SerializeField] private Role role;
    public Role Role { get { return role; } set { role = value; } }
    void Start()
    {
        boardManager = GameObject.FindGameObjectWithTag("BM").GetComponent<BoardManager>();
        Vector3 firstPos = transform.position;
        this.Id = FormatTwoDigit((int)role) + PositionToString2Digit(firstPos) + FormatFirstOwnerForId(HasMasterOwnership);
    }

    #region Create Id Methods
    string PositionToString2Digit(Vector3 pos)
    {
        string x = FormatFloatForIdPos(pos.x);
        string y = FormatFloatForIdPos(pos.z);
        return x + y;
    }

    string FormatFloatForIdPos(float value)
    {
        // 100以上の場合
        if (100f <= value)
        {
            return "00";
        }

        if (10 <= value && value < 100)
        {
            return value.ToString().Substring(0, 2);
        }

        if (0 <= value && value < 10)
        {
            return "0" + value.ToString().Substring(0, 1);
        }

        if (-10 < value && value < 0)
        {
            return "1" + value.ToString().Substring(1, 1);
        }

        if (value <= 10)
        {
            return "00";
        }

        return "xx";
    }

    string FormatFirstOwnerForId(bool isMaster)
    {
        if (isMaster) return "0";
        else return "1";
    }

    public string FormatTwoDigit(int number)
    {
        if (number < 10)
        {
            return "0" + number.ToString();
        }
        else
        {
            return number.ToString();
        }
    }
    #endregion

    public (int x, int y)[]? RangeOfMovementAsRole()
    {
        int directionFactor = hasMasterOwnership ? -1 : 1;
        Debug.Log($"RangeOfMovementAsRole is called as :{this.role}");
        switch (this.role)
        {
            case Role.HoheiId:
                return new[] { (0, 1) }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.KyoshaId:
                return new[] { (0, 1), (0, 2), (0, 3), (0, 4), (0, 5), (0, 6), (0, 7), (0, 8), (0, 9) }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.KeumaId:
                return new[] { (1, 2), (-1, 2) }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.GinshoId:
                return new[] { (-1, 1), (0, 1), (1, 1), (1, -1), (-1, -1) }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.KiinshoId:
                return new[] { (-1, 1), (0, 1), (1, 1), (-1, 0), (1, 0), (0, -1) }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.KakugyoId:
                return new[] {
                     (1, 1), (2, 2), (3, 3), (4, 4), (5, 5), (6, 6), (7, 7), (8, 8), (9, 9)
                    ,(-1, 1), (-2, 2), (-3, 3), (-4, 4), (-5, 5), (-6, 6), (-7, 7), (-8, 8), (-9, 9)
                    ,(-1, -1), (-2, -2), (-3, -3), (-4, -4), (-5, -5), (-6, -6), (-7, -7), (-8, -8), (-9, -9)
                    ,(1, -1), (2, -2), (3, -3), (4, -4), (5, -5), (6, -6), (7, -7), (8, -8), (9, -9)
                }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.HishaId:
                return new[] {
                     (0, 1), (0, 2), (0, 3), (0, 4), (0, 5), (0, 6), (0, 7), (0, 8), (0, 9)
                    ,(1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, 0), (7, 0), (8, 0), (9, 0)
                    ,(0, -1), (0, -2), (0, -3), (0, -4), (0, -5), (0, -6), (0, -7), (0, -8), (0, -9)
                    ,(-1, 0), (-2, 0), (-3, 0), (-4, 0), (-5, 0), (-6, 0), (-7, 0), (-8, 0), (-9, 0)
                }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.OhId:
                return new[] { (0, -1), (0, 1), (1, 1), (-1, 0), (1, 0), (-1, -1), (-1, 1), (1, -1) }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.NariKakuId:
                return new[] {
                     (1, 1), (2, 2), (3, 3), (4, 4), (5, 5), (6, 6), (7, 7), (8, 8), (9, 9)
                    ,(-1, 1), (-2, 2), (-3, 3), (-4, 4), (-5, 5), (-6, 6), (-7, 7), (-8, 8), (-9, 9)
                    ,(-1, -1), (-2, -2), (-3, -3), (-4, -4), (-5, -5), (-6, -6), (-7, -7), (-8, -8), (-9, -9)
                    ,(1, -1), (2, -2), (3, -3), (4, -4), (5, -5), (6, -6), (7, -7), (8, -8), (9, -9)
                    ,(0, -1), (0, 1), (1, 1), (-1, 0), (1, 0), (-1, -1), (-1, 0), (1, -1)
                }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.NariHishaId:
                return new[] {
                     (0, 1), (0, 2), (0, 3), (0, 4), (0, 5), (0, 6), (0, 7), (0, 8), (0, 9)
                    ,(1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, 0), (7, 0), (8, 0), (9, 0)
                    ,(0, -1), (0, -2), (0, -3), (0, -4), (0, -5), (0, -6), (0, -7), (0, -8), (0, -9)
                    ,(-1, 0), (-2, 0), (-3, 0), (-4, 0), (-5, 0), (-6, 0), (-7, 0), (-8, 0), (-9, 0)
                    ,(0, -1), (0, 1), (1, 1), (-1, 0), (1, 0), (-1, -1), (-1, 0), (1, -1)
                }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.NarikinId:
                return new[] { (-1, 1), (0, 1), (1, 1), (-1, 0), (1, 0), (0, -1) }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
        }
        return null;
    }

    public void Move(int x, int y)
    {
        transform.position = new Vector3(x, 0, y);
        // Todo: Chnage the isTurnPlayerMoved true
        Debug.Log("Move method is still construction");
    }

    public void Attack()
    {
        Debug.Log("Attack method is still construction");
        // Todo: Change the status of isAlive and ownership
        // Todo: CallDeleteFromBoard;
    }
    public (int x, int y) GetCurrentPos()
    {
        return ((int)transform.position.x, (int)transform.position.z);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        print($"オブジェクト{name} ({eventData.pointerPress}) がクリックされたよ！");
        boardManager.TouchedChara = eventData.pointerPress;
        boardManager.CurrentPos = currentPos;
        boardManager.CharacterModelForTransfer = GetComponent<CharacterModel>();
        if (boardManager.CurrentPos == currentPos) { boardInfo.ColorPos(true, false); }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Grid")
        {
            currentPos = other.transform.position;
            boardInfo = other.GetComponent<BoardInfo>();
        }
    }
}

