using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandyMethods : MonoBehaviour
{
    public GameObject FindChildWithTag(Transform parent, string tag)
    {
        // 親オブジェクトの全ての子オブジェクトをループ
        foreach (Transform child in parent)
        {
            if (child.gameObject.tag == tag)
            {
                // タグが一致する子オブジェクトが見つかった場合、そのGameObjectを返す
                return child.gameObject;
            }
        }

        // タグが一致する子オブジェクトが見つからなかった場合、nullを返す
        return null;
    }
}
