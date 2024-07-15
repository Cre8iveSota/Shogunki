using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool isMasterTurn;
    [SerializeField] private GameObject nari, nariPanel;
    [SerializeField] private TMP_Text senteTimerText, goteTimerText;
    [SerializeField] private TMP_Text senteTimerNum, goteTimerNum;
    [SerializeField] private GameObject masterCamera, masterGridCamera;
    [SerializeField] private GameObject clientCamera, clientGridCamera;
    [SerializeField] private GameObject GameEndPanel;
    [SerializeField] private TMP_Text WinnerText;
    [SerializeField] private GameObject tebanStarterPanel;
    [SerializeField] private TMP_Text nextPlayerText;
    public bool IsMasterTurn
    {
        get { return isMasterTurn; }
        set
        {
            isMasterTurn = value;
            cameraChanger.HitCommanderView(false);

            foreach (CharacterModel model in boardManager.allChara)
            {
                model.ShowUpTurnPlayerCharacter();
            }
        }
    }
    private float timeCnt = 0;
    private float timeCntTurn = 0;
    private int entireTime;
    public int EntireTime { get { return entireTime; } }
    CharacterModel targetNari;
    public int senteTimeLimit;
    public int goteTimeLimit;
    private bool isCallingNari;
    public bool IsCallingNari
    {
        get { return isCallingNari; }
        set
        {
            isCallingNari = value;
            if (!isCallingNari)
            {
                nari.SetActive(false);
            }
        }
    }
    private CameraChanger cameraChanger;
    private bool hasActed;
    public bool HasActed
    {
        get { return hasActed; }
        set
        {
            hasActed = value;
            if (hasActed)
            {
                if ((IsMasterTurn && senteTimeLimit > 1f) || (!IsMasterTurn && goteTimeLimit > 1f))
                {
                    StartCoroutine(WaitFunc(.5f, TurnChange));
                }
            }
        }
    }

    BoardManager boardManager;
    bool countEnable;

    // Start is called before the first frame update
    void Awake()
    {
        cameraChanger = GameObject.FindGameObjectWithTag("CC").GetComponent<CameraChanger>();
        boardManager = GameObject.FindGameObjectWithTag("BM").GetComponent<BoardManager>();
    }
    void Start()
    {
        Time.timeScale = 1;
        GameEndPanel.SetActive(false);
        nari.SetActive(false);
        senteTimeLimit = 60;
        goteTimeLimit = 60;
        IsMasterTurn = true;
        HasActed = false;
        tebanStarterPanel.SetActive(false);

        InvisibleCharacter();
    }

    // Update is called once per frame
    void Update()
    {
        timeCnt += Time.deltaTime;
        if (countEnable) timeCntTurn += Time.deltaTime;
        entireTime = (int)timeCnt;
        CountDownTimer(IsMasterTurn);
    }

    /// this method is called after moved character every time
    public void CallNari(CharacterModel chara)
    {
        if (chara.Role == Role.KinshoId || chara.Role == Role.NarikinId || chara.Role == Role.NariKakuId || chara.Role == Role.NariHishaId || chara.Role == Role.TokinId || chara.Role == Role.NariKyoId || chara.Role == Role.NariKeiId || chara.Role == Role.NariGinId)
        {
            Debug.Log("This chara is already Nari");
            return;
        }
        nari.SetActive(true);
        nariPanel.GetComponent<Image>().raycastTarget = true;
        nariPanel.GetComponent<Image>().color = new Color(255, 255, 255, 180f / 255f);

        targetNari = chara;

        // TODO: after press buttom deactivate it
        Debug.LogWarning("Not Implemented");
    }
    public void ReactNariButton(bool yes)
    {
        if (yes)
        {
            if (targetNari == null) { Debug.LogError("NOT FIND THE targetNari"); return; }
            // change role
            switch (targetNari.Role)
            {
                case Role.HoheiId:
                    targetNari.Role = Role.TokinId;
                    break;
                case Role.KyoshaId:
                    targetNari.Role = Role.NariKyoId;
                    break;
                case Role.KeumaId:
                    targetNari.Role = Role.NariKeiId;
                    break;
                case Role.GinshoId:
                    targetNari.Role = Role.NariGinId;
                    break;
                case Role.KakugyoId:
                    targetNari.Role = Role.NariKakuId;
                    break;
                case Role.HishaId:
                    targetNari.Role = Role.NariHishaId;
                    break;
            }
            // Update role text and movement range
            targetNari.RoleAbility();
            nari.SetActive(false);
        }
        else
        {
            nari.SetActive(false);
        }
        HasActed = true;
        IsCallingNari = false;
        nariPanel.GetComponent<Image>().raycastTarget = false;
        nariPanel.GetComponent<Image>().color = new Color(255, 255, 255, 0);
    }
    public void ReactTebanPlayerButton()
    {
        boardManager.LoadCharas();
        tebanStarterPanel.SetActive(false);
        countEnable = true;
        visibleMovigCollider();
    }

    private void visibleMovigCollider()
    {
        if (IsMasterTurn)
        {
            senteTimerText.color = Color.black;
            senteTimerNum.color = Color.black;
            goteTimerText.color = Color.gray;
            goteTimerNum.color = Color.gray;
            foreach (CharacterModel model in boardManager.masterChara)
            {
                model.FireAllMC();
            }
        }
        else
        {
            senteTimerText.color = Color.gray;
            senteTimerNum.color = Color.gray;
            goteTimerText.color = Color.black;
            goteTimerNum.color = Color.black;
            foreach (CharacterModel model in boardManager.clientChara)
            {
                model.FireAllMC();
            }
        }
    }

    private void CountDownTimer(bool isMaster)
    {
        if (isMaster)
        {
            senteTimerNum.text = $"{(int)(senteTimeLimit - timeCntTurn) / 60:0}:{(senteTimeLimit - timeCntTurn) % 60:00}";
            if (senteTimeLimit - timeCntTurn <= 0)
            {
                CloseNariWindow();
                TurnChange();
            }
        }
        else
        {
            goteTimerNum.text = $"{(int)(goteTimeLimit - timeCntTurn) / 60:0}:{(goteTimeLimit - timeCntTurn) % 60:00}";
            if (goteTimeLimit - timeCntTurn <= 0)
            {
                CloseNariWindow();
                TurnChange();
            }
        }
    }

    private void CloseNariWindow()
    {
        if (isCallingNari)
        {
            IsCallingNari = false;
            nariPanel.GetComponent<Image>().raycastTarget = false;
            nariPanel.GetComponent<Image>().color = new Color(255, 255, 255, 0);
        }
    }

    public void TurnChange()
    {
        countEnable = false;

        timeCntTurn = 0;
        senteTimeLimit = 59;
        goteTimeLimit = 59;
        IsCallingNari = false;

        ShowTurnPlayerWindow();

        IsMasterTurn = !IsMasterTurn;
        InvisibleCharacter();
        HasActed = false;
        Debug.Log("TURN CHANGED HAVE BEEN CALLED");
        // CallVisibleChara(isMasterTurn);
    }

    private void InvisibleCharacter()
    {
        if (!IsMasterTurn)
        {
            foreach (CharacterModel obj in boardManager.masterChara)
            {
                obj.visible.SetActive(false);
                Debug.Log($"master invisible name cara id {obj.Id}");
            }
        }
        else
        {
            foreach (CharacterModel obj in boardManager.clientChara)
            {
                obj.visible.SetActive(false);
                Debug.Log($"client invisible name cara id {obj.Id}");
            }
        }
    }
    private void ShowTurnPlayerWindow()
    {
        tebanStarterPanel.SetActive(true);
        if (isMasterTurn)
        {
            nextPlayerText.text = "後手の手番です。準備完了ボタンを押して下さい。";
        }
        else
        {
            nextPlayerText.text = "先手の手番です。準備完了ボタンを押して下さい。";
        }
    }
    public void GameEnd(bool isMasterWin)
    {
        Time.timeScale = 0;
        GameEndPanel.SetActive(true);
        WinnerText.text = isMasterWin ? "先手勝利" : "後手勝利";
    }

    IEnumerator WaitFunc(float seconds, Action func)
    {
        // 指定された秒数待機
        yield return new WaitForSeconds(seconds);

        // 関数を実行
        func();
    }
}
