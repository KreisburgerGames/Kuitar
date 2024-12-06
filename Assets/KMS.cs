using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KMS : MonoBehaviour
{
    Color epilepsy;

    void Update()
    {
        epilepsy = new Color(Random.Range(0.00f, 1.00f), Random.Range(0.00f, 1.00f), Random.Range(0.00f, 1.00f));
        GetComponent<Camera>().backgroundColor = epilepsy;
    }
}
