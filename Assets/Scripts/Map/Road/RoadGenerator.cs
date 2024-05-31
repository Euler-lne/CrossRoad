using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class RoadGenerator : TerrainGenerator
{
    public GameObject border;
    public GameObject road;
    public float offset;
    public float offsetBottom;
    public float relativePosition;
    public bool isRiver;
    int num;
    void Start()
    {
        num = Random.Range(1, 4);
        float borderH = border.GetComponent<SpriteRenderer>().size.y;
        float roadH = road.GetComponent<SpriteRenderer>().size.y;
        GameObject borderObject = Instantiate(border, transform.position, Quaternion.identity, transform);
        Vector3 offsetY = Vector3.up * borderH / 2 + Vector3.up * roadH / 2;
        if (isRiver)
        {
            borderObject.transform.localScale = new Vector3(1, -1, 1);
            offsetY = Vector3.up * roadH / 2;
        }
        for (int i = 0; i < num; i++)
        {
            Instantiate(road, transform.position + Vector3.up * offset * i + offsetY, Quaternion.identity, transform);
        }
        offsetY = Vector3.up * borderH;
        if (isRiver)
        {
            offsetY = Vector3.zero;
        }
        Instantiate(border, transform.position + Vector3.up * offset * num + offsetY, Quaternion.identity, transform);
        landHeight = roadH * num + borderH * 2;
    }

}
