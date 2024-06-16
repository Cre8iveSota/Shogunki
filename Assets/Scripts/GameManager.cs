using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool isMasterTurn;
    public bool IsMasterTurn { get { return isMasterTurn; } set { isMasterTurn = value; } }
    private float timeCnt = 0;
    private int entireTime;
    public int EntireTime { get { return entireTime; } }
    // Start is called before the first frame update
    void Start()
    {
        isMasterTurn = true;
    }

    // Update is called once per frame
    void Update()
    {
        timeCnt += Time.deltaTime;
        entireTime = (int)timeCnt;
    }
}
