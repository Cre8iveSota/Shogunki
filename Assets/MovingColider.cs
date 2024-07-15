using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingColider : MonoBehaviour
{
    // Start is called before the first frame update
    BoxCollider boxCollider;
    public CharacterModel parentCharacterModel;
    bool hashit;
    [SerializeField] bool enableSlowMode;
    [SerializeField] private bool allowColliderFire;
    public bool AllowColliderFire
    {
        get { return allowColliderFire; }
        set
        {
            allowColliderFire = value;
            boxCollider.center = firstColPos;
        }
    }
    [SerializeField] Vector3 firstColPos = new Vector3(0, 1, 3);
    // hohei, kyosya 0,1,3
    private int cnt = 0;
    private GameManager gameManager;
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        parentCharacterModel = this.transform.parent.transform.parent.GetComponent<CharacterModel>();
        gameManager = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!parentCharacterModel.HasMasterOwnership && gameManager.IsMasterTurn)
        {
            AllowColliderFire = false;
        }
        else if (parentCharacterModel.HasMasterOwnership && !gameManager.IsMasterTurn)
        {
            AllowColliderFire = false;
        }
        if (allowColliderFire) { MoveColider(); cnt++; }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("had hit" + other.gameObject.tag);
        if (other.CompareTag("ClientCharacter") || other.CompareTag("MasterCharacter"))
        {
            AllowColliderFire = false;
            hashit = true;
            other.gameObject.GetComponent<CharacterModel>().visible.SetActive(true);
            Debug.Log($"Show up attacked chara is {other.gameObject.GetComponent<CharacterModel>().Id} and role is {other.gameObject.GetComponent<CharacterModel>().Role} from {parentCharacterModel.Id} and the role is {parentCharacterModel.Role}");
            boxCollider.center = firstColPos;
            cnt = 0;
            hashit = false;
        }
    }
    void MoveColider()
    {
        Vector3 movingSpeedStraigt;
        Vector3 movingSpeedDiagonal;
        if (enableSlowMode)
        {
            movingSpeedStraigt = new Vector3(0, 0, .005f);
            movingSpeedDiagonal = new Vector3(.005f, 0, .005f);
        }
        else
        {
            movingSpeedStraigt = new Vector3(0, 0, .05f);
            movingSpeedDiagonal = new Vector3(.05f, 0, .05f);
        }
        int cntThreshould = enableSlowMode ? 30000 : 10000;

        if (hashit || !allowColliderFire) return;
        else if (cnt > cntThreshould)
        {
            AllowColliderFire = false;
            cnt = 0;
        }
        else if (parentCharacterModel.Role == Role.HoheiId || parentCharacterModel.Role == Role.TokinId)
        {
            boxCollider.center += movingSpeedStraigt;
            if (boxCollider.center.z > 5.5f) { AllowColliderFire = false; }
        }
        else if (parentCharacterModel.Role == Role.KyoshaId || parentCharacterModel.Role == Role.NariKyoId)
        {
            boxCollider.center += movingSpeedStraigt;
        }
        else if (parentCharacterModel.Role == Role.GinshoId || parentCharacterModel.Role == Role.NariGinId)
        {
            if (CompareTag("VisibleFS"))
            {
                boxCollider.center += movingSpeedStraigt;
                if (boxCollider.center.z > 5.5f) { AllowColliderFire = false; }
            }
            else if (CompareTag("VisibleFR"))
            {
                boxCollider.center += new Vector3(movingSpeedDiagonal.x, 0, movingSpeedDiagonal.z);
                if (boxCollider.center.z > 5.5f) { AllowColliderFire = false; }
            }
            else if (CompareTag("VisibleFL"))
            {
                boxCollider.center += new Vector3(movingSpeedDiagonal.x * -1, 0, movingSpeedDiagonal.z);
                if (boxCollider.center.z > 5.5f) { AllowColliderFire = false; }
            }
            else if (CompareTag("VisibleBL"))
            {
                boxCollider.center += new Vector3(movingSpeedDiagonal.x * -1, 0, movingSpeedDiagonal.z * -1);
                if (boxCollider.center.z < -5.5f) { AllowColliderFire = false; }
            }
            else if (CompareTag("VisibleBR"))
            {
                boxCollider.center += new Vector3(movingSpeedDiagonal.x, 0, movingSpeedDiagonal.z * -1);
                if (boxCollider.center.z < -5.5f) { AllowColliderFire = false; }
            }
        }
        else if (parentCharacterModel.Role == Role.KinshoId)
        {
            if (CompareTag("VisibleFS"))
            {
                boxCollider.center += movingSpeedStraigt;
                if (boxCollider.center.z > 5.5f) { AllowColliderFire = false; }
            }
            else if (CompareTag("VisibleFR"))
            {
                boxCollider.center += new Vector3(movingSpeedDiagonal.x, 0, movingSpeedDiagonal.z);
                if (boxCollider.center.z > 5.5f) { AllowColliderFire = false; }
            }
            else if (CompareTag("VisibleFL"))
            {
                boxCollider.center += new Vector3(movingSpeedDiagonal.x * -1, 0, movingSpeedDiagonal.z);
                if (boxCollider.center.z > 5.5f) { AllowColliderFire = false; }
            }
            else if (CompareTag("VisibleBS"))
            {
                boxCollider.center += new Vector3(0, 0, movingSpeedDiagonal.z * -1);
                if (boxCollider.center.z < -5.5f) { AllowColliderFire = false; }
            }
            else if (CompareTag("VisibleR"))
            {
                boxCollider.center += new Vector3(movingSpeedDiagonal.x, 0, 0);
                if (boxCollider.center.x > 5.5f) { AllowColliderFire = false; }
            }
            else if (CompareTag("VisibleL"))
            {
                boxCollider.center += new Vector3(movingSpeedDiagonal.x * -1, 0, 0);
                if (boxCollider.center.x < -5.5f) { AllowColliderFire = false; }
            }
        }
        else if (parentCharacterModel.Role == Role.KakugyoId || parentCharacterModel.Role == Role.NariKakuId)
        {
            if (CompareTag("VisibleFR"))
            {
                boxCollider.center += new Vector3(movingSpeedDiagonal.x, 0, movingSpeedDiagonal.z);
                if (boxCollider.center.z > 30f) { AllowColliderFire = false; }
            }
            else if (CompareTag("VisibleFL"))
            {
                boxCollider.center += new Vector3(movingSpeedDiagonal.x * -1, 0, movingSpeedDiagonal.z);
                if (boxCollider.center.z > 30f) { AllowColliderFire = false; }
            }
            else if (CompareTag("VisibleBL"))
            {
                boxCollider.center += new Vector3(movingSpeedDiagonal.x * -1, 0, movingSpeedDiagonal.z * -1);
                if (boxCollider.center.z < -30f) { AllowColliderFire = false; }
            }
            else if (CompareTag("VisibleBR"))
            {
                boxCollider.center += new Vector3(movingSpeedDiagonal.x, 0, movingSpeedDiagonal.z * -1);
                if (boxCollider.center.z < -30f) { AllowColliderFire = false; }
            }
        }
        else if (parentCharacterModel.Role == Role.HishaId || parentCharacterModel.Role == Role.NariHishaId)
        {
            if (CompareTag("VisibleFS"))
            {
                boxCollider.center += movingSpeedStraigt;
                if (boxCollider.center.z > 30f) { AllowColliderFire = false; }
            }
            else if (CompareTag("VisibleBS"))
            {
                boxCollider.center += new Vector3(0, 0, movingSpeedDiagonal.z * -1);
                if (boxCollider.center.z < -30f) { AllowColliderFire = false; }
            }
            else if (CompareTag("VisibleR"))
            {
                boxCollider.center += new Vector3(movingSpeedDiagonal.x, 0, 0);
                if (boxCollider.center.x > 30f) { AllowColliderFire = false; }
            }
            else if (CompareTag("VisibleL"))
            {
                boxCollider.center += new Vector3(movingSpeedDiagonal.x * -1, 0, 0);
                if (boxCollider.center.x < -30f) { AllowColliderFire = false; }
            }
        }
        else if (parentCharacterModel.Role == Role.OhId)
        {
            if (CompareTag("VisibleFS"))
            {
                boxCollider.center += movingSpeedStraigt;
                if (boxCollider.center.z > 5.5f) { AllowColliderFire = false; }
            }
            else if (CompareTag("VisibleFR"))
            {
                boxCollider.center += new Vector3(movingSpeedDiagonal.x, 0, movingSpeedDiagonal.z);
                if (boxCollider.center.z > 5.5f) { AllowColliderFire = false; }
            }
            else if (CompareTag("VisibleFL"))
            {
                boxCollider.center += new Vector3(movingSpeedDiagonal.x * -1, 0, movingSpeedDiagonal.z);
                if (boxCollider.center.z > 5.5f) { AllowColliderFire = false; }
            }
            else if (CompareTag("VisibleBS"))
            {
                boxCollider.center += new Vector3(0, 0, movingSpeedDiagonal.z * -1);
                if (boxCollider.center.z < -5.5f) { AllowColliderFire = false; }
            }
            else if (CompareTag("VisibleR"))
            {
                boxCollider.center += new Vector3(movingSpeedDiagonal.x, 0, 0);
                if (boxCollider.center.x > 5.5f) { AllowColliderFire = false; }
            }
            else if (CompareTag("VisibleL"))
            {
                boxCollider.center += new Vector3(movingSpeedDiagonal.x * -1, 0, 0);
                if (boxCollider.center.x < -5.5f) { AllowColliderFire = false; }
            }
            else if (CompareTag("VisibleBL"))
            {
                boxCollider.center += new Vector3(movingSpeedDiagonal.x * -1, 0, movingSpeedDiagonal.z * -1);
                if (boxCollider.center.z < -5.5f) { AllowColliderFire = false; }
            }
            else if (CompareTag("VisibleBR"))
            {
                boxCollider.center += new Vector3(movingSpeedDiagonal.x, 0, movingSpeedDiagonal.z * -1);
                if (boxCollider.center.z < -5.5f) { AllowColliderFire = false; }
            }
        }
    }
}
