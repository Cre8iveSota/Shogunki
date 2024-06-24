using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MotigomaManager : MonoBehaviour
{
    private List<CharacterModel> masterMotigoma = new List<CharacterModel>();
    public List<CharacterModel> MasterMotigoma { get { return masterMotigoma; } set { masterMotigoma = value; } }
    private List<CharacterModel> clientMotigoma = new List<CharacterModel>();
    public List<CharacterModel> ClientMotigoma { get { return clientMotigoma; } set { clientMotigoma = value; } }
    [SerializeField] private TMP_Text motigomaMasterHoheiText, motigomaMasterKyoshaText, motigomaMasterKeumaText, motigomaMasterGinshoText, motigomaMasterKinshoText, motigomaMasterKakugyoText, motigomaMasterHishaText;
    private int motigomaMasterHohei, motigomaMasterKyosha, motigomaMasterKeuma, motigomaMasterGinsho, motigomaMasterKinsho, motigomaMasterKakugyo, motigomaMasterHisha;
    [SerializeField] private TMP_Text motigomaClientHoheiText, motigomaClientKyoshaText, motigomaClientKeumaText, motigomaClientGinshoText, motigomaClientKinshoText, motigomaClientKakugyoText, motigomaClientHishaText;
    private int motigomaClientHohei, motigomaClientKyosha, motigomaClientKeuma, motigomaClientGinsho, motigomaClientKinsho, motigomaClientKakugyo, motigomaClientHisha;
    [SerializeField] GameObject senteMotigomaList, goteteMotigomaList;
    private Role? updateMotigomaTargetRole = null;
    public Role? UpdateMotigomaTargetRole { get { return updateMotigomaTargetRole; } set { updateMotigomaTargetRole = value; } }
    GameManager gameManager;
    BoardManager boardManager;
    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        boardManager = GameObject.FindGameObjectWithTag("BM").GetComponent<BoardManager>();

        // Initialize each motigoma list, master and client
        UpdateMotigoma(true);
        UpdateMotigoma(false);
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
                if (isMaster) motigomaMasterHohei++;
                else motigomaClientHohei++;
                break;
            case Role.KyoshaId:
                if (isMaster) motigomaMasterKyosha++;
                else motigomaClientKyosha++;
                break;
            case Role.KeumaId:
                if (isMaster) motigomaMasterKeuma++;
                else motigomaClientKeuma++;
                break;
            case Role.GinshoId:
                if (isMaster) motigomaMasterGinsho++;
                else motigomaClientGinsho++;
                break;
            case Role.KiinshoId:
                if (isMaster) motigomaMasterKinsho++;
                else motigomaClientKinsho++;
                break;
            case Role.KakugyoId:
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

        // Assign NULL to prevent unexpected update whenever this method is called
        UpdateMotigomaTargetRole = null;
    }

    // public void UseMotigoma(bool isMaster, int roleNum)
    // {
    //     switch (roleNum)
    //     {
    //         case (int)Role.HoheiId:
    //             if (isMaster && gameManager.IsMasterTurn)

    //                 Debug.Log("");
    //             break;
    //     }
    // }
}


