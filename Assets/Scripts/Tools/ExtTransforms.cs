using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Transforms
{
    public static void DestryChildren(this Transform t, bool destroyImmediately = false)
    {
        foreach (Transform child in t) {
            if (destroyImmediately)
            {
                MonoBehaviour.DestroyImmediate(child.gameObject);
            }
            else {
                MonoBehaviour.Destroy(child.gameObject);
            }
        }
    }
}
