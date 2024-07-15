using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Queue;

public class CharaViewRange : MonoBehaviour
{
    Role role;
    bool hasMasterOwnership;
    GameManager gameManager;
    CharacterModel parentModel;
    BoardManager boardManager;

    private void Awake()
    {
        parentModel = transform.parent.GetComponent<CharacterModel>();
        role = transform.parent.GetComponent<CharacterModel>().Role;
        hasMasterOwnership = transform.parent.GetComponent<CharacterModel>().HasMasterOwnership;
        gameManager = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        boardManager = GameObject.FindGameObjectWithTag("BM").GetComponent<BoardManager>();
    }

    private void OnTriggerStay(Collider other)
    {

    }
}


