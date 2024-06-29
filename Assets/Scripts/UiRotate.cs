using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiRotate : MonoBehaviour
{
    // Start is called before the first frame update
    void LateUpdate()
    {
        //　カメラと同じ向きに設定
        transform.rotation = Camera.main.transform.rotation;
    }

}
