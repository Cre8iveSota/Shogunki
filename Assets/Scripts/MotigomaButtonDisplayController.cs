using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MotigomaButtonDisplayController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Button[] masterMotigomaButton = new Button[7];
    [SerializeField] Button[] clientMotigomaButton = new Button[7];
    MotigomaManager motigomaManager;
    void Start()
    {
        motigomaManager = GameObject.FindGameObjectWithTag("MM").GetComponent<MotigomaManager>();
    }

    // Update is called once per frame
    public void ComtrolButtonDisplay()
    {
        if (motigomaManager.motigomaMasterHohei == 0) { masterMotigomaButton[0].interactable = false; }
        else { masterMotigomaButton[0].interactable = true; }
        if (motigomaManager.motigomaMasterKyosha == 0) { masterMotigomaButton[1].interactable = false; }
        else { masterMotigomaButton[1].interactable = true; }
        if (motigomaManager.motigomaMasterKeuma == 0) { masterMotigomaButton[2].interactable = false; }
        else { masterMotigomaButton[2].interactable = true; }
        if (motigomaManager.motigomaMasterGinsho == 0) { masterMotigomaButton[3].interactable = false; }
        else { masterMotigomaButton[3].interactable = true; }
        if (motigomaManager.motigomaMasterKinsho == 0) { masterMotigomaButton[4].interactable = false; }
        else { masterMotigomaButton[4].interactable = true; }
        if (motigomaManager.motigomaMasterKakugyo == 0) { masterMotigomaButton[5].interactable = false; }
        else { masterMotigomaButton[5].interactable = true; }
        if (motigomaManager.motigomaMasterHisha == 0) { masterMotigomaButton[6].interactable = false; }
        else { masterMotigomaButton[6].interactable = true; }

        if (motigomaManager.motigomaClientHohei == 0) { clientMotigomaButton[0].interactable = false; }
        else { clientMotigomaButton[0].interactable = true; }
        if (motigomaManager.motigomaClientKyosha == 0) { clientMotigomaButton[1].interactable = false; }
        else { clientMotigomaButton[1].interactable = true; }
        if (motigomaManager.motigomaClientKeuma == 0) { clientMotigomaButton[2].interactable = false; }
        else { clientMotigomaButton[2].interactable = true; }
        if (motigomaManager.motigomaClientGinsho == 0) { clientMotigomaButton[3].interactable = false; }
        else { clientMotigomaButton[3].interactable = true; }
        if (motigomaManager.motigomaClientKinsho == 0) { clientMotigomaButton[4].interactable = false; }
        else { clientMotigomaButton[4].interactable = true; }
        if (motigomaManager.motigomaClientKakugyo == 0) { clientMotigomaButton[5].interactable = false; }
        else { clientMotigomaButton[5].interactable = true; }
        if (motigomaManager.motigomaClientHisha == 0) { clientMotigomaButton[6].interactable = false; }
        else { clientMotigomaButton[6].interactable = true; }
    }
}
