using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardInfo : MonoBehaviour, IPointerClickHandler
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
    GameManager gameManager;
    MotigomaManager motigomaManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GM")?.GetComponent<GameManager>();
        boardManager = GameObject.FindGameObjectWithTag("BM")?.GetComponent<BoardManager>();
        motigomaManager = GameObject.FindGameObjectWithTag("MM")?.GetComponent<MotigomaManager>();

        // boardManager.AddBoardData((int)transform.position.x, (int)transform.position.z, BoardStatus.Empty, false);
        RegisterIntoBoardDataBank(gameManager.IsMasterTurn);
    }

    public void RegisterIntoBoardDataBank(bool isMasterTurn)
    {
        // Once All grid is registered as empty
        Debug.Log($"RegisterIntoBoardDataBank is working; Add BoardMasterDic as Client- x: {(int)transform.position.x}, z: {(int)transform.position.z}, BoardStatus: {BoardStatus.ClientCharacterExist}, isMovablePos : {null}");
        boardManager.AddBoardData((int)transform.position.x, (int)transform.position.z, BoardStatus.Empty, null);
        Collider[] colliders = Physics.OverlapBox(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.localScale / 2, Quaternion.identity);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.CompareTag("MasterCharacter"))
            {
                Debug.Log($"RegisterIntoBoardDataBank is working; Add BoardMasterDic as Master  - x: {(int)transform.position.x}, z: {(int)transform.position.z}, BoardStatus: {BoardStatus.MasterCharacterExist}, isMovablePos :{!isMasterTurn}");
                boardManager.AddBoardData((int)transform.position.x, (int)transform.position.z, BoardStatus.MasterCharacterExist, !isMasterTurn);
            }
            else if (collider.gameObject.CompareTag("ClientCharacter"))
            {
                Debug.Log($"RegisterIntoBoardDataBank is working; Add BoardMasterDic as Client- x: {(int)transform.position.x}, z: {(int)transform.position.z}, BoardStatus: {BoardStatus.ClientCharacterExist}, isMovablePos : {isMasterTurn}");
                boardManager.AddBoardData((int)transform.position.x, (int)transform.position.z, BoardStatus.ClientCharacterExist, isMasterTurn);
            }
        }
    }

    // Update is called once per frame

    public void ColoringGrid(Color color)
    {
        GetComponent<Renderer>().material.SetColor("_Color", color);
    }

    public void ColorPos(bool isCurrentPos, bool isMovablePos, bool isMasterCharaTapped = false, bool isClientCharaTapped = false)
    {
        this.isCurrentPos = isCurrentPos;
        this.isMovablePos = isMovablePos;
        // Todo: before online mode, to check the player who toched screen is unnecessary; Instead, to detect somebody touch or not is necessary
        // So, this is fraft until creating online mode
        // isMasterCharaTapped = Input.GetMouseButton(0);

        // Todo: fix when it begin to create online mode
        // isClientTapped = Input.GetMouseButton(0);

        // if (isMasterCharaTapped)
        // {
        if (isCurrentPos)
        {
            if (boardManager.CurrentPos == transform.position)
            {
                Debug.Log("Color Change Red");
                ColoringGrid(Color.red);
            }
        }
        else if (isMovablePos)
        {
            Debug.Log($"boardManager.MovablePos: {boardManager.MovablePos} transform: {transform.position} ");

            if (boardManager.MovablePos != null && boardManager.MovablePos.Count > 0)
            {
                Debug.Log("Color Change yellow");
                ColoringGrid(Color.yellow);
            }
            else
            {
                Debug.Log("The dictionary is empty.");
            }
        }
        else
        {
            ColoringGrid(Color.white);
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
            Debug.Log($"Add BoardMasterDic as Master - x: {(int)transform.position.x}, z: {(int)transform.position.z}, BoardStatus: {BoardStatus.MasterCharacterExist}, isMovablePos :{false}");
            boardManager.AddBoardData((int)transform.position.x, (int)transform.position.z, BoardStatus.MasterCharacterExist, false);
        }
        if (other.gameObject.CompareTag("ClientCharacter"))
        {
            Debug.Log($"Add BoardMasterDic as Client- x: {(int)transform.position.x}, z: {(int)transform.position.z}, BoardStatus: {BoardStatus.MasterCharacterExist}, isMovablePos : {false}");
            boardManager.AddBoardData((int)transform.position.x, (int)transform.position.z, BoardStatus.ClientCharacterExist, false);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("MasterCharacter") || other.gameObject.CompareTag("ClientCharacter"))
        {
            Debug.Log($"Remove BoardMasterDic x: {(int)transform.position.x}, y: {(int)transform.position.z}, z: {BoardStatus.MasterCharacterExist}");
            boardManager.RemoveBoardData((int)transform.position.x, (int)transform.position.z);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        print($"オブジェクト{name} ({eventData.pointerPress}) がクリックされたよ！");
        if (motigomaManager.IsUsingMotigoma)
        {
            Renderer renderer = GetComponent<Renderer>();
            Color color = renderer.material.GetColor("_Color");
            if (color == Color.green)
            {
                motigomaManager.PutMotigoma(gameManager.IsMasterTurn, (int)this.transform.position.x, (int)this.transform.position.z);
                gameManager.TurnChange(gameManager.IsMasterTurn);
            }
        }
        else if (isMovablePos && boardManager != null && boardManager.TouchedChara != null &&
        (boardManager.TouchedChara.GetComponent<CharacterModel>().HasMasterOwnership == gameManager.IsMasterTurn)
        || (!boardManager.TouchedChara.GetComponent<CharacterModel>().HasMasterOwnership == !gameManager.IsMasterTurn))
        {
            GameObject obj = eventData.pointerCurrentRaycast.gameObject;
            if (obj.GetComponent<BoardInfo>() != null)
            {
                Vector3 clickedCharaPos = obj.GetComponent<BoardInfo>().transform.position;
                if (gameManager.IsMasterTurn)
                {
                    boardManager.AddBoardData((int)clickedCharaPos.x, (int)clickedCharaPos.z, BoardStatus.MasterCharacterExist, false);
                }
                else
                {
                    boardManager.AddBoardData((int)clickedCharaPos.x, (int)clickedCharaPos.z, BoardStatus.ClientCharacterExist, false);
                }
                boardManager.ExpectedMovingPos = ((int)clickedCharaPos.x, (int)clickedCharaPos.z);
            }
        }
    }
}
