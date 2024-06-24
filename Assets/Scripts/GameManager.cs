using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool isMasterTurn;
    [SerializeField] private GameObject nari;
    [SerializeField] private TMP_Text senteTimer, goteTimer;
    public bool IsMasterTurn { get { return isMasterTurn; } set { isMasterTurn = value; } }
    private float timeCnt = 0;
    private int entireTime;
    public int EntireTime { get { return entireTime; } }
    CharacterModel targetNari;
    // Start is called before the first frame update
    void Start()
    {
        isMasterTurn = true;
        nari.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        timeCnt += Time.deltaTime;
        entireTime = (int)timeCnt;
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
    }
}
