using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MotigomaManager : MonoBehaviour
{
    private List<CharacterModel> masterMotigoma = new List<CharacterModel>();
    public List<CharacterModel> MasterMotigoma
    {
        get { return masterMotigoma; }
        set { masterMotigoma = value; }
    }
    private List<CharacterModel> clientMotigoma = new List<CharacterModel>();
    public List<CharacterModel> ClientMotigoma
    {
        get { return clientMotigoma; }
        set { clientMotigoma = value; }
    }
    [SerializeField] private TMP_Text motigomaMasterHoheiText, motigomaMasterKyoshaText, motigomaMasterKeumaText, motigomaMasterGinshoText, motigomaMasterKinshoText, motigomaMasterKakugyoText, motigomaMasterHishaText;
    public int motigomaMasterHohei, motigomaMasterKyosha, motigomaMasterKeuma, motigomaMasterGinsho, motigomaMasterKinsho, motigomaMasterKakugyo, motigomaMasterHisha;
    [SerializeField] private TMP_Text motigomaClientHoheiText, motigomaClientKyoshaText, motigomaClientKeumaText, motigomaClientGinshoText, motigomaClientKinshoText, motigomaClientKakugyoText, motigomaClientHishaText;
    public int motigomaClientHohei, motigomaClientKyosha, motigomaClientKeuma, motigomaClientGinsho, motigomaClientKinsho, motigomaClientKakugyo, motigomaClientHisha;
    [SerializeField] GameObject senteMotigomaList, goteteMotigomaList;
    private Role? updateMotigomaTargetRole = null;
    public Role? UpdateMotigomaTargetRole { get { return updateMotigomaTargetRole; } set { updateMotigomaTargetRole = value; } }
    private bool isUsingMotigoma;
    public bool IsUsingMotigoma { get { return isUsingMotigoma; } set { isUsingMotigoma = value; } }
    GameManager gameManager;
    BoardManager boardManager;
    // 現状getter や setterはsummonTargetMotigomaに対して過剰品質のためpublicで行う
    public CharacterModel summonTargetMotigomaMaster, summonTargetMotigomaClient;
    private CharacterModel attackedCharacter;
    public CharacterModel AttackedCharacter
    {
        get { return attackedCharacter; }
        set
        {
            attackedCharacter = value;
            if (gameManager.IsMasterTurn) MasterMotigoma.Add(attackedCharacter);
            else ClientMotigoma.Add(attackedCharacter);
        }
    }
    private MotigomaButtonDisplayController motigomaButtonDisplayController;
    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        boardManager = GameObject.FindGameObjectWithTag("BM").GetComponent<BoardManager>();
        motigomaButtonDisplayController = GetComponent<MotigomaButtonDisplayController>();

        ShowTheNumberOfMotigoma();
    }
    public void ShowMotigoma(bool isMaster)
    {
        if (isMaster)
        {
            Debug.Log($"Master Motigoma List: count: {MasterMotigoma.Count}, type:{MasterMotigoma[0].Role}");
        }
        else
        {
            Debug.Log($"Master Motigoma List: count: {ClientMotigoma.Count}, type:{clientMotigoma[0].Role}");
        }
    }

    public void UpdateMotigoma(bool isMaster)
    {

        senteMotigomaList.SetActive(isMaster);
        goteteMotigomaList.SetActive(!isMaster);

        switch (updateMotigomaTargetRole)
        {
            case Role.HoheiId:
            case Role.TokinId:
                if (isMaster) motigomaMasterHohei++;
                else motigomaClientHohei++;
                break;
            case Role.KyoshaId:
            case Role.NariKyoId:
                if (isMaster) motigomaMasterKyosha++;
                else motigomaClientKyosha++;
                break;
            case Role.KeumaId:
            case Role.NariKeiId:
                if (isMaster) motigomaMasterKeuma++;
                else motigomaClientKeuma++;
                break;
            case Role.GinshoId:
            case Role.NariGinId:
                if (isMaster) motigomaMasterGinsho++;
                else motigomaClientGinsho++;
                break;
            case Role.KinshoId:
            case Role.NarikinId:
                if (isMaster) motigomaMasterKinsho++;
                else motigomaClientKinsho++;
                break;
            case Role.KakugyoId:
            case Role.NariKakuId:
                if (isMaster) motigomaMasterKakugyo++;
                else motigomaClientKakugyo++;
                break;
            case Role.HishaId:
                if (isMaster) motigomaMasterHisha++;
                else motigomaClientHisha++;
                break;
            case null:
            default:
                Debug.Log("NOTHING TO DO");
                break;
        }
        ShowTheNumberOfMotigoma();

        // Assign NULL to prevent unexpected update whenever this method is called
        UpdateMotigomaTargetRole = null;
    }

    private void ShowTheNumberOfMotigoma()
    {
        motigomaButtonDisplayController.ComtrolButtonDisplay();
        motigomaMasterHoheiText.text = $"歩兵\n{motigomaMasterHohei}";
        motigomaClientHoheiText.text = $"歩兵\n{motigomaClientHohei}";
        motigomaMasterKyoshaText.text = $"香車\n{motigomaMasterKyosha}";
        motigomaClientKyoshaText.text = $"香車\n{motigomaClientKyosha}";
        motigomaMasterKeumaText.text = $"佳馬\n{motigomaMasterKeuma}";
        motigomaClientKeumaText.text = $"佳馬\n{motigomaClientKeuma}";
        motigomaMasterGinshoText.text = $"銀将\n{motigomaMasterGinsho}";
        motigomaClientGinshoText.text = $"銀将\n{motigomaClientGinsho}";
        motigomaMasterKinshoText.text = $"金将\n{motigomaMasterKinsho}";
        motigomaClientKinshoText.text = $"金将\n{motigomaClientKinsho}";
        motigomaMasterKakugyoText.text = $"角行\n{motigomaMasterKakugyo}";
        motigomaClientKakugyoText.text = $"角行\n{motigomaClientKakugyo}";
        motigomaMasterHishaText.text = $"飛車\n{motigomaMasterHisha}";
        motigomaClientHishaText.text = $"飛車\n{motigomaClientHisha}";
    }


    // UseMotigomaClient とUseMotigomaMasterを一緒にした関数を作成しようとした場合、引数が2つ必要になった。引数が§§２つ以上だとonButtonClickで呼び出せないため、マスター用とクライアント用でボタンを分割
    public void UseMotigomaClient(int roleNum)
    {
        if (gameManager.IsMasterTurn) return;
        if (0 <= roleNum && roleNum <= 6)
        {
            InsertSummonTargetMotigoma(false, roleNum);
        }
        CheckUsablePos(roleNum);
    }
    public void UseMotigomaMaster(int roleNum)
    {
        Debug.Log($"UseMotigomaMaster is called: roleNum: {roleNum}, gameManager.IsMasterTurn: {gameManager.IsMasterTurn}");
        if (!gameManager.IsMasterTurn) return;
        if (0 <= roleNum && roleNum <= 6)
        {
            InsertSummonTargetMotigoma(true, roleNum);
        }
        CheckUsablePos(roleNum);
    }

    private void InsertSummonTargetMotigoma(bool isMaster, int roleNum)
    {
        if (isMaster)
        {
            Debug.Log($"Start InsertSummonTargetMotigoma as isMaster true, masterMotigoma Count: {masterMotigoma.Count}");
            foreach (CharacterModel charaModel in masterMotigoma)
            {
                string roleString = (int)charaModel.Role > 9 ? ((int)charaModel.Role).ToString().Substring(1, 1) : ((int)charaModel.Role).ToString();
                Debug.Log($"Role: {charaModel.Role}, roleString: {roleString}");
                Debug.Log($"InsertSummonTargetMotigoma: masterMotigoma count: {masterMotigoma.Count}, charaModel.Role: {(int)charaModel.Role}, roleNum: {roleNum} ");
                if ((int)charaModel.Role == roleNum)
                {
                    charaModel.Role = (Role)int.Parse(roleString);
                    summonTargetMotigomaMaster = charaModel;
                    return;
                }
            }
        }
        if (!isMaster)
        {
            Debug.Log($"Start InsertSummonTargetMotigoma as isMaster false, clientMotigoma Count: {clientMotigoma.Count}");
            foreach (CharacterModel charaModel in clientMotigoma)
            {
                string roleString = (int)charaModel.Role > 9 ? ((int)charaModel.Role).ToString().Substring(1, 1) : ((int)charaModel.Role).ToString();
                Debug.Log($"Role: {charaModel.Role}, roleString: {roleString}");
                if (int.Parse(roleString) == roleNum)
                {
                    charaModel.Role = (Role)int.Parse(roleString);
                    summonTargetMotigomaClient = charaModel;
                    return;
                }
            }
        }
        // summonTargetMotigomaMaster = null;
        // summonTargetMotigomaClient = null;
    }

    private void CheckUsablePos(int roleNum)
    {
        switch (roleNum)
        {
            case (int)Role.HoheiId:
                // 二歩 チェック
                IsUsingMotigoma = true;
                ChangeBoardColorEmpty(Color.green, gameManager.IsMasterTurn, true);

                break;
            case (int)Role.KyoshaId:
            case (int)Role.KeumaId:
            case (int)Role.GinshoId:
            case (int)Role.KinshoId:
            case (int)Role.KakugyoId:
            case (int)Role.HishaId:
                IsUsingMotigoma = true;
                ChangeBoardColorEmpty(Color.green, gameManager.IsMasterTurn, false);
                break;
        }
    }


    private void ChangeBoardColorEmpty(Color color, bool isMaster, bool checkNifu)
    {
        List<int> hoheiPosX = new List<int>();
        if (checkNifu && isMaster)
        {
            GameObject[] chracters = GameObject.FindGameObjectsWithTag("MasterCharacter");
            InsertHoheiPos(hoheiPosX, chracters);
        }
        if (checkNifu && !isMaster)
        {
            GameObject[] chracters = GameObject.FindGameObjectsWithTag("ClientCharacter");
            InsertHoheiPos(hoheiPosX, chracters);
        }

        // Get empty grid
        //call color() 
        foreach (GameObject grid in boardManager.GetEmptyCoordinate())
        {
            grid.GetComponent<BoardInfo>().ColoringGrid(color);

            if (hoheiPosX.Contains((int)grid.transform.position.x))
            {
                grid.GetComponent<BoardInfo>().ColoringGrid(Color.white);
            }
        }
    }

    private static void InsertHoheiPos(List<int> hoheiPosX, GameObject[] chracters)
    {
        foreach (GameObject obj in chracters)
        {
            CharacterModel charaModel = obj.GetComponent<CharacterModel>();
            if (charaModel.Role == Role.HoheiId)
            {
                hoheiPosX.Add(charaModel.GetCurrentPos().x);
            }
        }
    }

    public void PutMotigoma(bool isMaster, int gridPosX, int gridPosZ)
    {
        if (isMaster)
        {
            Debug.Log($"Master SUMMON TARGET is +{summonTargetMotigomaMaster.name}");
            if (summonTargetMotigomaMaster.Id.Substring(summonTargetMotigomaMaster.Id.Length - 1) == "1")
            {
                summonTargetMotigomaMaster.transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
            summonTargetMotigomaMaster.transform.position = new Vector3(gridPosX, 0.1f, gridPosZ);
            summonTargetMotigomaMaster.IsAlive = true;
            MasterMotigoma.Remove(summonTargetMotigomaMaster);
            Debug.Log($"PutMotigoma isMaster true, masterMotigoma Count: {masterMotigoma.Count}");
            DecreaseMotigomaNum(isMaster, summonTargetMotigomaMaster.Role);
            ShowTheNumberOfMotigoma();
        }
        else
        {
            Debug.Log("Client SUMMON TARGET is +" + summonTargetMotigomaClient.Id);
            Debug.Log("ID last:" + summonTargetMotigomaClient.Id.Substring(summonTargetMotigomaClient.Id.Length - 1));
            if (summonTargetMotigomaClient.Id.Substring(summonTargetMotigomaClient.Id.Length - 1) == "0")
            {
                summonTargetMotigomaClient.transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
            summonTargetMotigomaClient.transform.position = new Vector3(gridPosX, 0.1f, gridPosZ);
            summonTargetMotigomaClient.IsAlive = true;
            ClientMotigoma.Remove(summonTargetMotigomaClient);
            Debug.Log($"PutMotigoma isMaster true, clientMotigoma Count: {clientMotigoma.Count}");
            DecreaseMotigomaNum(isMaster, summonTargetMotigomaClient.Role);
            ShowTheNumberOfMotigoma();
        }
        ChangeBoardColorEmpty(Color.white, gameManager.IsMasterTurn, false);
    }

    private void DecreaseMotigomaNum(bool isMaster, Role role)
    {
        switch (role)
        {
            case Role.HoheiId:
                if (isMaster) motigomaMasterHohei--;
                else motigomaClientHohei--;
                break;
            case Role.KyoshaId:
                if (isMaster) motigomaMasterKyosha--;
                else motigomaClientKyosha--;
                break;
            case Role.KeumaId:
                if (isMaster) motigomaMasterKeuma--;
                else motigomaClientKeuma--;
                break;
            case Role.GinshoId:
                if (isMaster) motigomaMasterGinsho--;
                else motigomaClientGinsho--;
                break;
            case Role.KinshoId:
                if (isMaster) motigomaMasterKinsho--;
                else motigomaClientKinsho--;
                break;
            case Role.KakugyoId:
                if (isMaster) motigomaMasterKakugyo--;
                else motigomaClientKakugyo--;
                break;
            case Role.HishaId:
                if (isMaster) motigomaMasterHisha--;
                else motigomaClientHisha--;
                break;
            default:
                Debug.Log("NOTHING TO DO");
                break;
        }
    }
}


