using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraChanger : MonoBehaviour
{
    [SerializeField] GameObject godView;
    [SerializeField] GameObject masterCommanderView, clientCommanderView;
    [SerializeField] GameObject characterView;
    [SerializeField] GameObject masterKingAsForCharaViewDefault, clientKingAsForCharaViewDefault;
    CharacterModel cModelMater, cModelClient;
    GameManager gameManager;
    CinemachineVirtualCamera _vc;
    BoardManager boardManager;
    private bool isExpectedPosUpdated;
    public bool IsExpectedPosUpdated
    {
        get { return isExpectedPosUpdated; }
        set
        {
            isExpectedPosUpdated = value;
            if (boardManager.AttackingTarget != null && boardManager.AttackingTarget.IsAlive) { _vc.LookAt = boardManager.AttackingTarget.gameObject.transform; }
            isExpectedPosUpdated = false;
        }
    }
    private bool isTouchedCharaUpdated;
    public bool IsTouchedCharaUpdated
    {
        get { return isTouchedCharaUpdated; }
        set
        {
            isTouchedCharaUpdated = value;
            _vc.Follow = boardManager.TouchedChara.transform;
            _vc.LookAt = boardManager.TouchedChara.transform;
            isTouchedCharaUpdated = false;
        }
    }
    void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        _vc = GameObject.FindGameObjectWithTag("VC").GetComponent<CinemachineVirtualCamera>();
        boardManager = GameObject.FindGameObjectWithTag("BM").GetComponent<BoardManager>();
        cModelMater = masterKingAsForCharaViewDefault.GetComponent<CharacterModel>();
        cModelClient = clientKingAsForCharaViewDefault.GetComponent<CharacterModel>();
    }

    void Start()
    {

    }
    public void HitGodView()
    {
        godView.SetActive(true);
        masterCommanderView.SetActive(false);
        clientCommanderView.SetActive(false);
        characterView.SetActive(false);
    }

    public void HitCommanderView()
    {
        if (gameManager.IsMasterTurn)
        {
            godView.SetActive(false);
            masterCommanderView.SetActive(true);
            clientCommanderView.SetActive(false);
            characterView.SetActive(false);
        }
        else
        {
            godView.SetActive(false);
            masterCommanderView.SetActive(false);
            clientCommanderView.SetActive(true);
            characterView.SetActive(false);
        }
    }
    public void HitCharacterView()
    {
        godView.SetActive(false);
        masterCommanderView.SetActive(false);
        clientCommanderView.SetActive(false);
        characterView.SetActive(true);
        _vc.Follow = boardManager.TouchedChara.transform;
        if (gameManager.IsMasterTurn && cModelMater.IsAlive)
        {
            _vc.LookAt = clientKingAsForCharaViewDefault.transform;
        }
        else if (!gameManager.IsMasterTurn && cModelClient.IsAlive)
        {
            _vc.LookAt = masterKingAsForCharaViewDefault.transform;
        }
    }
}
