using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject[] initMap;
    public GameObject grassLand;
    public GameObject street;
    public GameObject river;
    public float offset;
    private float height;
    public float originHeight = 18.8f;
    private Vector3 generatePos;
    private float originCameraOffsetY;
    private readonly List<MapListItem> mapList = new();
    private void Start()
    {
        originCameraOffsetY = Camera.main.transform.position.y;
        OriginTerrain();
    }
    private void OnEnable()
    {
        EventHandler.FrogJumped += OnFrogJumped;
    }
    private void OnDisable()
    {
        EventHandler.FrogJumped -= OnFrogJumped;

    }

    private void OriginTerrain()
    {
        height = originHeight;
        int index = Random.Range(0, initMap.Length);
        GameObject instantiated = Instantiate(initMap[index], transform.position, Quaternion.identity, transform);
        MapListItem item = new(instantiated, height, Settings.TerrainType.Grass);
        mapList.Add(item);
        SpawnTerrain();
    }

    private Settings.TerrainType OnFrogJumped(float frogY)
    {
        CheckPoint();
        for (int i = 0; i < mapList.Count(); i++)
        {
            if (mapList[i].height <= frogY && mapList[i].GetTerrainHeight() >= frogY)
            {
                return mapList[i].type;
            }
        }
        return Settings.TerrainType.Nothing;
    }

    private void CheckPoint()
    {
        if (Camera.main.transform.position.y > height - offset)
        {
            UpdateHeight();
            SpawnTerrain();
        }
    }
    private void SpawnTerrain()
    {
        var type = Random.Range(0, 3);
        GameObject prefab = type switch
        {
            0 => grassLand,
            1 => street,
            _ => river,
        };
        if (type != 0)
        {
            height += prefab.GetComponent<RoadGenerator>().offsetBottom;
        }
        generatePos = new Vector3(transform.position.x, height, 0);
        // 要添加实例化后的
        GameObject instantiated = Instantiate(prefab, generatePos, Quaternion.identity, transform);
        Settings.TerrainType terrainType = type switch
        {
            0 => Settings.TerrainType.Grass,
            1 => Settings.TerrainType.Road,
            _ => Settings.TerrainType.River,
        };
        MapListItem item = new(instantiated, height, terrainType);
        mapList.Add(item);
    }
    void UpdateHeight()
    {
        if (mapList.Count > 0)
        {
            for (int i = mapList.Count() - 1; i >= 0; i--)
            {
                // 使用倒序才能删除
                if (Camera.main.transform.position.y > mapList[i].GetTerrainHeight() + originCameraOffsetY + 5f)
                {
                    Destroy(mapList[i].item);
                    mapList.RemoveAt(i);
                }
            }
        }
        height += mapList.Last().item.GetComponent<TerrainGenerator>().GetLandHeight();
        float num = height / 2.35f;
        if (!Mathf.Approximately(num, Mathf.Floor(num)))
        {
            // 不相等需要进行位移操作
            height = Mathf.Ceil(num) * 2.35f;
        }
    }
}
public class MapListItem
{
    public GameObject item;
    public float height; // 生成的位置的高度
    public Settings.TerrainType type;
    public MapListItem(GameObject _item, float _height, Settings.TerrainType _type)
    {
        item = _item;
        height = _height;
        type = _type;
    }
    public float GetTerrainHeight()
    {
        // 获取加上总高度的位置
        if (item.GetComponent<TerrainGenerator>())
            return item.GetComponent<TerrainGenerator>().GetLandHeight() + height;
        // 初始场景
        return height;
    }
}
