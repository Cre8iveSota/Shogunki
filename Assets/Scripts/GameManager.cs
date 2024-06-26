using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool isMasterTurn;
    [SerializeField] private GameObject nari;
    [SerializeField] private TMP_Text senteTimerText, goteTimerText;
    [SerializeField] private TMP_Text senteTimerNum, goteTimerNum;
    public bool IsMasterTurn { get { return isMasterTurn; } set { isMasterTurn = value; } }
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
    // Start is called before the first frame update
    void Start()
    {
        isMasterTurn = true;
        nari.SetActive(false);
        senteTimeLimit = 60;
        goteTimeLimit = 60;
    }

    // Update is called once per frame
    void Update()
    {
        timeCnt += Time.deltaTime;
        timeCntTurn += Time.deltaTime;
        entireTime = (int)timeCnt;
        CountDownTimer(isMasterTurn);
    }

    /// this method is called after moved character every time
    public void CallNari(CharacterModel chara)
    {
        if (chara.Role == Role.KiinshoId || chara.Role == Role.NarikinId || chara.Role == Role.NariKakuId || chara.Role == Role.NariHishaId)
        {
            Debug.Log("This chara is already Nari");
            return;
        }
        nari.SetActive(true);
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
                case Role.KyoshaId:
                case Role.KeumaId:
                case Role.GinshoId:
                    targetNari.Role = Role.NarikinId;
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
        TurnChange(isMasterTurn);
        isCallingNari = false;
    }

    private void CountDownTimer(bool isMaster)
    {
        if (isMaster)
        {
            senteTimerNum.text = $"{(int)(senteTimeLimit - timeCntTurn) / 60:0}:{(senteTimeLimit - timeCntTurn) % 60:00}";
        }
        else
        {
            goteTimerNum.text = $"{(int)(goteTimeLimit - timeCntTurn) / 60:0}:{(goteTimeLimit - timeCntTurn) % 60:00}";
        }
    }

    public void TurnChange(bool isMasterTurnEnd)
    {
        timeCntTurn = 0;
        senteTimeLimit = 59;
        goteTimeLimit = 59;
        isMasterTurn = !isMasterTurnEnd;
        isCallingNari = false;
        if (isMasterTurn)
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
}
