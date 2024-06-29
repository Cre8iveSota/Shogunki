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
    public bool IsMasterTurn
    {
        get { return isMasterTurn; }
        set
        {
            isMasterTurn = value;
            cameraChanger.HitCommanderView(false);
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
    // Start is called before the first frame update
    void Awake()
    {
        cameraChanger = GameObject.FindGameObjectWithTag("CC").GetComponent<CameraChanger>();
    }
    void Start()
    {
        GameEndPanel.SetActive(false);
        nari.SetActive(false);
        senteTimeLimit = 60;
        goteTimeLimit = 60;
        IsMasterTurn = true;
    }

    // Update is called once per frame
    void Update()
    {
        timeCnt += Time.deltaTime;
        timeCntTurn += Time.deltaTime;
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
        TurnChange(IsMasterTurn);
        isCallingNari = false;
        nariPanel.GetComponent<Image>().raycastTarget = false;
        nariPanel.GetComponent<Image>().color = new Color(255, 255, 255, 0);
    }

    private void CountDownTimer(bool isMaster)
    {
        if (isMaster)
        {
            senteTimerNum.text = $"{(int)(senteTimeLimit - timeCntTurn) / 60:0}:{(senteTimeLimit - timeCntTurn) % 60:00}";
            if (senteTimeLimit - timeCntTurn <= 0)
            {
                CloseNariWindow();
                TurnChange(IsMasterTurn);
            }
        }
        else
        {
            goteTimerNum.text = $"{(int)(goteTimeLimit - timeCntTurn) / 60:0}:{(goteTimeLimit - timeCntTurn) % 60:00}";
            if (goteTimeLimit - timeCntTurn <= 0)
            {
                CloseNariWindow();
                TurnChange(IsMasterTurn);
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

    public void TurnChange(bool isMasterTurnEnd)
    {
        timeCntTurn = 0;
        senteTimeLimit = 59;
        goteTimeLimit = 59;
        IsMasterTurn = !isMasterTurnEnd;
        isCallingNari = false;
        if (IsMasterTurn)
        {
            senteTimerText.color = Color.black;
            senteTimerNum.color = Color.black;
            goteTimerText.color = Color.gray;
            goteTimerNum.color = Color.gray;
        }
        else
        {
            senteTimerText.color = Color.gray;
            senteTimerNum.color = Color.gray;
            goteTimerText.color = Color.black;
            goteTimerNum.color = Color.black;
        }
    }

    public void GameEnd(bool isMasterWin)
    {
        Time.timeScale = 0;
        GameEndPanel.SetActive(true);
        WinnerText.text = isMasterWin ? "先手勝利" : "後手勝利";
    }
}
