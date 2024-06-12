using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardInfo : MonoBehaviour
{
    [SerializeField] private bool isMasterTapped;
    public bool IsMasterTapped { get { return isMasterTapped; } set { isMasterTapped = value; } }
    [SerializeField] private bool isClientTapped;
    public bool IsClientTapped { get { return isClientTapped; } set { isClientTapped = value; } }
    [SerializeField] private bool isMovablePos;
    public bool IsMovablePos { get { return isMovablePos; } set { isMovablePos = value; } }
    [SerializeField] private bool isCurrentPos;
    public bool IsCurrentPos { get { return isCurrentPos; } set { isCurrentPos = value; } }
    BoardManager boardManager;

    // Start is called before the first frame update
    void Start()
    {
        boardManager = GameObject.FindGameObjectWithTag("BM").GetComponent<BoardManager>();
    }

    // Update is called once per frame

    public void ColorPos(bool isCurrentPos, bool isMovablePos = false, bool isMasterCharaTapped = false, bool isClientCharaTapped = false)
    {
        // Todo: before online mode, to check the player who toched screen is unnecessary; Instead, to detect somebody touch or not is necessary
        // So, this is fraft until creating online mode
        // isMasterCharaTapped = Input.GetMouseButton(0);

        // Todo: fix when it begin to create online mode
        // isClientTapped = Input.GetMouseButton(0);

        // if (isMasterCharaTapped)
        // {
        if (isCurrentPos)
        {
            if (boardManager.CurrentPos == transform)
            {
                Debug.Log("Color Change Red");
                GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            }
        }
        else
        {
            GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        }
        // }
        // Todo: fix when it begin to create online mode
        // if (isClientCharaTapped)
        // {

        // }
    }

    // below Method is desearved as ChangeGridInfo
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MasterCharacter"))
        {
            Debug.Log($"Add BoardMasterDic as Master - x: {(int)transform.position.x}, y: {(int)transform.position.z}, z: {BoardStatus.MasterCharacterExist}");
            boardManager.AddBoardData((int)transform.position.x, (int)transform.position.z, BoardStatus.MasterCharacterExist);
        }
        if (other.gameObject.CompareTag("ClientCharacter"))
        {
            Debug.Log($"Add BoardMasterDic as Client- x: {(int)transform.position.x}, y: {(int)transform.position.z}, z: {BoardStatus.MasterCharacterExist}");
            boardManager.AddBoardData((int)transform.position.x, (int)transform.position.z, BoardStatus.ClientCHaracterExist);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("MasterCharacter") || other.gameObject.CompareTag("ClientCharacter"))
        {
            Debug.Log($"Remove BoardMasterDicx: {(int)transform.position.x}, y: {(int)transform.position.z}, z: {BoardStatus.MasterCharacterExist}");
            boardManager.RemoveBoardData((int)transform.position.x, (int)transform.position.z);
        }
    }
}
