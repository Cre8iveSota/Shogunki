using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterModel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string id;
    public string Id { get { return id; } private set { id = value; } }
    [SerializeField] private bool hasMasterOwnership;
    public bool HasMasterOwnership { get { return hasMasterOwnership; } set { hasMasterOwnership = value; } }
    [SerializeField] bool isAlive;
    public bool IsAlive { get { return isAlive; } set { isAlive = value; } }
    bool isMasterPlayerTapped = false;
    bool isClientPlayerTapped = false;
    Vector3 currentPos;

    BoardManager boardManager;
    BoardInfo boardInfo;
    GameManager gameManager;
    [SerializeField] private Role role;
    private TMP_Text roleLetter;
    private MotigomaManager motigomaManager;
    public Role Role { get { return role; } set { role = value; } }
    void Start()
    {
        boardManager = GameObject.FindGameObjectWithTag("BM").GetComponent<BoardManager>();
        gameManager = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        motigomaManager = GameObject.FindGameObjectWithTag("MM").GetComponent<MotigomaManager>();
        Vector3 firstPos = transform.position;
        this.Id = FormatTwoDigit((int)role) + PositionToString2Digit(firstPos) + FormatFirstOwnerForId(HasMasterOwnership);
        this.IsAlive = true;
        Debug.Log("child count: " + this.transform.childCount);
        foreach (Transform obj in this.transform)
        {
            if (obj.gameObject.CompareTag("RoleLetter"))
            {
                roleLetter = obj.GetComponent<TMP_Text>();
                Debug.Log("FOUND: " + roleLetter);
            }
        }
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

    public (int x, int y)[]? RoleAbility()
    {
        int directionFactor = hasMasterOwnership ? -1 : 1;
        Debug.Log($"RangeOfMovementAsRole is called as :{this.role}");
        switch (this.role)
        {
            case Role.HoheiId:
                roleLetter.GetComponent<TMP_Text>().text = "歩兵";
                return new[] { (0, 1) }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.KyoshaId:
                roleLetter.GetComponent<TMP_Text>().text = "香車";
                return new[] { (0, 1), (0, 2), (0, 3), (0, 4), (0, 5), (0, 6), (0, 7), (0, 8), (0, 9) }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.KeumaId:
                roleLetter.GetComponent<TMP_Text>().text = "佳馬";
                return new[] { (1, 2), (-1, 2) }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.GinshoId:
                roleLetter.GetComponent<TMP_Text>().text = "銀将";
                return new[] { (-1, 1), (0, 1), (1, 1), (1, -1), (-1, -1) }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.KiinshoId:
                roleLetter.GetComponent<TMP_Text>().text = "金将";
                return new[] { (-1, 1), (0, 1), (1, 1), (-1, 0), (1, 0), (0, -1) }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.KakugyoId:
                roleLetter.GetComponent<TMP_Text>().text = "角行";
                return new[] {
                     (1, 1), (2, 2), (3, 3), (4, 4), (5, 5), (6, 6), (7, 7), (8, 8), (9, 9)
                    ,(-1, 1), (-2, 2), (-3, 3), (-4, 4), (-5, 5), (-6, 6), (-7, 7), (-8, 8), (-9, 9)
                    ,(-1, -1), (-2, -2), (-3, -3), (-4, -4), (-5, -5), (-6, -6), (-7, -7), (-8, -8), (-9, -9)
                    ,(1, -1), (2, -2), (3, -3), (4, -4), (5, -5), (6, -6), (7, -7), (8, -8), (9, -9)
                }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.HishaId:
                roleLetter.GetComponent<TMP_Text>().text = "飛車";
                return new[] {
                     (0, 1), (0, 2), (0, 3), (0, 4), (0, 5), (0, 6), (0, 7), (0, 8), (0, 9)
                    ,(1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, 0), (7, 0), (8, 0), (9, 0)
                    ,(0, -1), (0, -2), (0, -3), (0, -4), (0, -5), (0, -6), (0, -7), (0, -8), (0, -9)
                    ,(-1, 0), (-2, 0), (-3, 0), (-4, 0), (-5, 0), (-6, 0), (-7, 0), (-8, 0), (-9, 0)
                }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.OhId:
                roleLetter.GetComponent<TMP_Text>().text = "王将";
                return new[] { (0, -1), (0, 1), (1, 1), (-1, 0), (1, 0), (-1, -1), (-1, 1), (1, -1) }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.NariKakuId:
                roleLetter.GetComponent<TMP_Text>().text = "龍馬";
                return new[] {
                     (1, 1), (2, 2), (3, 3), (4, 4), (5, 5), (6, 6), (7, 7), (8, 8), (9, 9)
                    ,(-1, 1), (-2, 2), (-3, 3), (-4, 4), (-5, 5), (-6, 6), (-7, 7), (-8, 8), (-9, 9)
                    ,(-1, -1), (-2, -2), (-3, -3), (-4, -4), (-5, -5), (-6, -6), (-7, -7), (-8, -8), (-9, -9)
                    ,(1, -1), (2, -2), (3, -3), (4, -4), (5, -5), (6, -6), (7, -7), (8, -8), (9, -9)
                    ,(0, -1), (0, 1), (1, 1), (-1, 0), (1, 0), (-1, -1), (-1, 0), (1, -1)
                }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.NariHishaId:
                roleLetter.GetComponent<TMP_Text>().text = "龍王";
                return new[] {
                     (0, 1), (0, 2), (0, 3), (0, 4), (0, 5), (0, 6), (0, 7), (0, 8), (0, 9)
                    ,(1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, 0), (7, 0), (8, 0), (9, 0)
                    ,(0, -1), (0, -2), (0, -3), (0, -4), (0, -5), (0, -6), (0, -7), (0, -8), (0, -9)
                    ,(-1, 0), (-2, 0), (-3, 0), (-4, 0), (-5, 0), (-6, 0), (-7, 0), (-8, 0), (-9, 0)
                    ,(0, -1), (0, 1), (1, 1), (-1, 0), (1, 0), (-1, -1), (-1, 0), (1, -1)
                }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.NarikinId:
                roleLetter.GetComponent<TMP_Text>().text = "成金";
                return new[] { (-1, 1), (0, 1), (1, 1), (-1, 0), (1, 0), (0, -1) }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
        }
        return null;
    }

    public void Move(int x, int y)
    {
        // 移動前に成ポジションにいたら成を呼ぶ
        if ((this.HasMasterOwnership == gameManager.IsMasterTurn && this.GetCurrentPos().y <= -4)
        || (this.HasMasterOwnership == !gameManager.IsMasterTurn && this.GetCurrentPos().y >= 4)
        )
        {
            gameManager.CallNari(this);
        }

        transform.position = new Vector3(x, 0.1f, y);

        // 移動後に成ポジションにいたら成を呼ぶ
        if ((this.HasMasterOwnership == gameManager.IsMasterTurn && this.GetCurrentPos().y <= -4)
        || (this.HasMasterOwnership == !gameManager.IsMasterTurn && this.GetCurrentPos().y >= 4)
        )
        {
            gameManager.CallNari(this);
        }

        // Check there is oppornent
        if (boardManager.AttackingTarget != null) Attack();

        // true: Attack and Delete and Change
        // Todo: Chnage the isTurnPlayerMoved true

        Debug.Log("Move method is still construction");
    }

    private void Attack()
    {
        Debug.Log("Attack method is still construction");
        DeleteAndChangeOwnership();
    }

    private void DeleteAndChangeOwnership()
    {
        motigomaManager.UpdateMotigomaTargetRole = boardManager.AttackingTarget.role;
        motigomaManager.UpdateMotigoma(gameManager.IsMasterTurn);
        motigomaManager.AttackedCharacter = boardManager.AttackingTarget;

        boardManager.AttackingTarget.IsAlive = false;
        boardManager.AttackingTarget.HasMasterOwnership = gameManager.IsMasterTurn;
        boardManager.AttackingTarget.gameObject.tag = gameManager.IsMasterTurn ? "MasterCharacter" : "ClientCharacter";
        boardManager.AttackingTarget.gameObject.transform.position = Vector3.up * 100;
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
        // IsAliveMotigomaがtrueのときに持ち駒を使えるようにしているため、盤上のキャラクターをタッチされた場合は持ち駒を使用していない状況と判断するためfalseにする
        motigomaManager.IsUsingMotigoma = false;
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

