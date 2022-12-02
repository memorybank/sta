using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionCheckTest : MonoBehaviour
{
    public GameObject[] gobjs;
    Vector3[] pts;

    // Start is called before the first frame update
    void Start()
    {
        pts = new Vector3[gobjs.Length];
        for (int i = 0; i < gobjs.Length; i++)
        {
            pts[i] = gobjs[i].transform.localPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < gobjs.Length; i++)
        {
            if ((gobjs[i].transform.localPosition - pts[i]).magnitude > 0.1)
            {
                Debug.Log("Position Change : " + gobjs[i].name + " Change: " + pts[i] + " => " + gobjs[i].transform.localPosition);
                pts[i] = gobjs[i].transform.localPosition;
            }
        }
    }
}
