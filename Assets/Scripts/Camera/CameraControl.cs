using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform frog;
    public float offsetY;
    public float zoomBase;
    private float ratio;
    private void Awake()
    {
        // 通过横纵比得到适当的比例
        ratio = (float)Screen.height / (float)Screen.width;
        Camera.main.orthographicSize = zoomBase * ratio * 0.5f;
        transform.position = new Vector3(transform.position.x, offsetY * ratio, transform.position.z);
    }
    private void LateUpdate()
    {
        if (frog && frog.GetComponent<PlayerController>().IsFaceUp())
            transform.position = new Vector3(transform.position.x, frog.position.y + offsetY * ratio, transform.position.z);
    }
}
