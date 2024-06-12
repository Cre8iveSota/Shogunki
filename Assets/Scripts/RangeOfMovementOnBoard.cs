using System.Collections;
using System.Collections.Generic;
using UnityEditor.MPE;
using UnityEngine;

public class RangeOfMovementOnBoard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    (int x, int y)[] IndicateMovablePosition(int[,] currentPos, Role role, bool isMasterPlayer)
    {
        if (role == Role.HoheiId)
        {
            // Todo: Bordinfo for return the info of grid data : isMaster for judgge controller ,null for indigation of out of field
        }
        return new (int, int)[] { (1, 1), (1, 2) };
    }
}
