using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CharacterModel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string id;
    public string Id { get { return id; } private set { id = value; } }
    [SerializeField] private bool hasMasterOwnership;
    public bool HasMasterOwnership { get { return hasMasterOwnership; } set { hasMasterOwnership = value; } }
    [SerializeField] bool isAlive;
    public bool IsAlive
    {
        get { return isAlive; }
        set
        {
            isAlive = value;
            Die(isAlive);
        }
    }
    bool isMasterPlayerTapped = false;
    bool isClientPlayerTapped = false;
    Vector3 currentPos;

    BoardManager boardManager;
    BoardInfo boardInfo;
    GameManager gameManager;
    [SerializeField] private Role role;
    private TMP_Text roleLetter;
    private MotigomaManager motigomaManager;
    public Role Role
    {
        get { return role; }
        set
        {
            role = value;
            RoleAbility();
        }
    }
    Animator animator;
    private bool hasCalledFunction = false;
    private bool hasCalledFunction2 = false;


    void Start()
    {
        boardManager = GameObject.FindGameObjectWithTag("BM").GetComponent<BoardManager>();
        gameManager = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        motigomaManager = GameObject.FindGameObjectWithTag("MM").GetComponent<MotigomaManager>();
        animator = GetComponent<Animator>();

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
                roleLetter.GetComponent<TMP_Text>().text = "桂馬";
                return new[] { (1, 2), (-1, 2) }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.GinshoId:
                roleLetter.GetComponent<TMP_Text>().text = "銀将";
                return new[] { (-1, 1), (0, 1), (1, 1), (1, -1), (-1, -1) }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.KinshoId:
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
            // case Role.NarikinId:
            //     roleLetter.GetComponent<TMP_Text>().text = "成金";
            //     return new[] { (-1, 1), (0, 1), (1, 1), (-1, 0), (1, 0), (0, -1) }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.TokinId:
                roleLetter.GetComponent<TMP_Text>().text = "と金";
                return new[] { (-1, 1), (0, 1), (1, 1), (-1, 0), (1, 0), (0, -1) }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.NariKyoId:
                roleLetter.GetComponent<TMP_Text>().text = "成香";
                return new[] { (-1, 1), (0, 1), (1, 1), (-1, 0), (1, 0), (0, -1) }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.NariKeiId:
                roleLetter.GetComponent<TMP_Text>().text = "成桂";
                return new[] { (-1, 1), (0, 1), (1, 1), (-1, 0), (1, 0), (0, -1) }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
            case Role.NariGinId:
                roleLetter.GetComponent<TMP_Text>().text = "成銀";
                return new[] { (-1, 1), (0, 1), (1, 1), (-1, 0), (1, 0), (0, -1) }.Select(p => (p.Item1, p.Item2 * directionFactor)).ToArray();
        }
        return null;
    }

    public void Move(int x, int y)
    {
        hasCalledFunction = false;
        hasCalledFunction2 = false;

        animator.SetInteger("actionNum", 1);
        // 移動前に成ポジションにいたら成を呼ぶ
        Debug.Log($"gameManager.IsCallingNari2: {gameManager.IsCallingNari} ,HasActed: {gameManager.HasActed} This role {this.role}");
        NariEnableCheck();
        Debug.Log($"gameManager.IsCallingNari22: {gameManager.IsCallingNari} ,HasActed: {gameManager.HasActed} This role {this.role}");

        transform.LookAt(new Vector3(x, 0.1f, y));
        Tween moveTween = boardManager.TouchedChara.transform.DOMove(new Vector3(x, 0.1f, y), 1 + 0.2f * Vector3.Distance(boardManager.CurrentPos, new Vector3(x, 0.1f, y)));
        moveTween.OnUpdate(() =>
        {
            float progress = moveTween.Elapsed() / (1 + 0.2f * Vector3.Distance(boardManager.CurrentPos, new Vector3(x, 0.1f, y)));

            // 進行状況が40%に達したら関数を呼び出す
            if (progress >= 0.3f && boardManager.AttackingTarget != null && !hasCalledFunction)
            {
                boardManager.TouchedChara.GetComponent<CharacterModel>().animator.SetInteger("actionNum", 2);
                hasCalledFunction = true;
            }
            if (progress >= 0.8f && !hasCalledFunction2)
            {
                // Check there is oppornent
                Attack();
                hasCalledFunction2 = true;
            }
        }).OnComplete(() =>
            {
                animator.SetInteger("actionNum", 0);
                transform.rotation = this.hasMasterOwnership ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
                // 移動後に成ポジションにいたら成を呼ぶ
                Debug.Log($"gameManager.IsCallingNari1: {gameManager.IsCallingNari} ,HasActed: {gameManager.HasActed} This role {this.role}");
                NariEnableCheck();
                Debug.Log($"gameManager.IsCallingNari11: {gameManager.IsCallingNari} ,HasActed: {gameManager.HasActed} This role {this.role}");
                if (!gameManager.IsCallingNari)
                {
                    // 時間ギリギリでキャラクターを動かしたとき、動き終わるまでにカウントダウンでターンが終了してしまう可能性がある。その場合に、
                    //ここで「HasActed = true;」を呼び出してしまうと、ターンが2度切り替わり、同じ人のターンが続くため、阻止するために、以下のチェックを入れる
                    if ((gameManager.IsMasterTurn && this.hasMasterOwnership) || (!gameManager.IsMasterTurn && !this.hasMasterOwnership))
                    {
                        gameManager.HasActed = true;
                    }
                }
                else { gameManager.CallNari(this); }
            }
        );
        Debug.Log("Move method is still construction");
    }

    private void Attack()
    {
        if (boardManager.AttackingTarget != null)
        {
            Debug.Log("Attack method is still construction");
            DeleteAndChangeOwnership();
            Debug.Log("Attack chara:" + this.id);
        }
    }

    public void AnimationEventEndAttack()
    {
        animator.SetInteger("actionNum", 0);
        Debug.Log("This id is + " + Id);
    }
    private void NariEnableCheck()
    {
        if
        (this.role == Role.NariKakuId
        || this.role == Role.NariHishaId
        || this.role == Role.TokinId
        || this.role == Role.NariKyoId
        || this.role == Role.NariKeiId
        || this.role == Role.NariGinId
        || this.role == Role.KinshoId)
        {
            return;
        }

        if ((this.HasMasterOwnership && gameManager.IsMasterTurn && this.GetCurrentPos().y <= -4)
        || (!this.HasMasterOwnership && !gameManager.IsMasterTurn && this.GetCurrentPos().y >= 4))
        {
            gameManager.IsCallingNari = true;
        }
    }

    private void DeleteAndChangeOwnership()
    {
        motigomaManager.UpdateMotigomaTargetRole = boardManager.AttackingTarget.role; //TODO なったひしゃ角が取られてマスターが使おうとしたとき使えない
        motigomaManager.UpdateMotigoma(gameManager.IsMasterTurn);
        motigomaManager.AttackedCharacter = boardManager.AttackingTarget;

        boardManager.AttackingTarget.IsAlive = false;
    }
    private void Die(bool isAlive)
    {
        if (!isAlive)
        {
            boardManager.AttackingTarget.HasMasterOwnership = gameManager.IsMasterTurn;
            boardManager.AttackingTarget.gameObject.tag = gameManager.IsMasterTurn ? "MasterCharacter" : "ClientCharacter";
            boardManager.AttackingTarget.gameObject.transform.position = new Vector3(boardManager.AttackingTarget.gameObject.transform.position.x + 100, 0f, 0);
        }
    }


    public (int x, int y) GetCurrentPos()
    {
        return ((int)transform.position.x, (int)transform.position.z);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        print($"オブジェクト{name} ({eventData.pointerPress}) がクリックされたよ！");
        if (gameManager.HasActed) return;
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
