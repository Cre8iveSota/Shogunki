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
    CharacterModel cModelMaster, cModelClient;
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
        cModelMaster = masterKingAsForCharaViewDefault.GetComponent<CharacterModel>();
        cModelClient = clientKingAsForCharaViewDefault.GetComponent<CharacterModel>();
    }

    void Start()
    {
        // This is a gurantee for recoverting Awake Method. This is worked as avoiding error
        cModelMaster = masterKingAsForCharaViewDefault?.GetComponent<CharacterModel>();
        cModelClient = clientKingAsForCharaViewDefault?.GetComponent<CharacterModel>();
    }
    public void HitGodView()
    {
        godView.SetActive(true);
        masterCommanderView.SetActive(false);
        clientCommanderView.SetActive(false);
        characterView.SetActive(false);
    }

    public void HitCommanderView(bool isFront)
    {
        if (isFront)
        {
            if (gameManager.IsMasterTurn)
            {
                godView.SetActive(false);
                masterCommanderView.SetActive(true);
                masterCommanderView.transform.position = new Vector3(0, 0, -10);
                clientCommanderView.SetActive(false);
                characterView.SetActive(false);
            }
            else
            {
                godView.SetActive(false);
                masterCommanderView.SetActive(false);
                clientCommanderView.SetActive(true);
                clientCommanderView.transform.position = new Vector3(0, 0, 10);
                characterView.SetActive(false);
            }
        }
        else
        {
            if (gameManager.IsMasterTurn)
            {
                godView.SetActive(false);
                masterCommanderView.SetActive(true);
                masterCommanderView.transform.position = new Vector3(0, 0, 0);
                clientCommanderView.SetActive(false);
                characterView.SetActive(false);
            }
            else
            {
                godView.SetActive(false);
                masterCommanderView.SetActive(false);
                clientCommanderView.SetActive(true);
                clientCommanderView.transform.position = new Vector3(0, 0, 0);
                characterView.SetActive(false);
            }
        }
    }
    public void HitCharacterView()
    {
        godView.SetActive(false);
        masterCommanderView.SetActive(false);
        clientCommanderView.SetActive(false);
        characterView.SetActive(true);
        _vc.Follow = boardManager.TouchedChara != null ? boardManager.TouchedChara.transform : gameManager.IsMasterTurn ? masterKingAsForCharaViewDefault.transform : clientKingAsForCharaViewDefault.transform;
        if (gameManager.IsMasterTurn && cModelMaster.IsAlive)
        {
            _vc.LookAt = clientKingAsForCharaViewDefault.transform;
        }
        else if (!gameManager.IsMasterTurn && cModelClient.IsAlive)
        {
            _vc.LookAt = masterKingAsForCharaViewDefault.transform;
        }
    }
}
