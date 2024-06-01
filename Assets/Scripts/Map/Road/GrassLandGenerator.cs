using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassLandGenerator : TerrainGenerator
{
    public GameObject[] BuildingList;
    public GameObject[] ObstacleList;
    public GameObject[] GrassList;
    public GameObject Bush;
    public GameObject Tree;
    public GameObject GrassShadow;
    public Transform BuildingParent;
    public Transform ObstacleParent;
    public Transform PlantParent;
    private int[] position = new int[10];
    private int[] distence = new int[10];
    private int[,] grassMap = new int[10, 5];
    private float width = 2.35f;
    // Start is called before the first frame update
    void Start()
    {
        InitGrassMap();
        CheckGrassMap();
        GenerateGrassMap();
        landHeight = width * 11;
    }
    void InitGrassMap()
    {
        int prePos = -1;
        for (int i = 0; i < 10; i++)
        {
            int passPos = Random.Range(0, 5);
            position[i] = passPos;
            grassMap[i, passPos] = Random.Range(0f, 1f) >= 0.25f ? 1 : 2;
            for (int j = 0; j < 5; j++)
            {
                bool isLongJump = i != 0 && grassMap[i - 1, j] == 2;
                if (j == passPos)
                {
                    // 可以通过的位置
                    grassMap[i, j] = isLongJump ? 1 : 2;
                    distence[i] = grassMap[i, j];
                }
                else
                {
                    if (!isLongJump && prePos != j && Random.Range(0f, 1f) > 0.5f)
                    {
                        // 障碍物
                        grassMap[i, j] = -1;
                    }
                    else if (!isLRHasObstacle(j, i) && Random.Range(0f, 1f) > 0.5f)
                    {
                        // 可以跳跃
                        grassMap[i, j] = isLongJump || Random.Range(0f, 1f) >= 0.25f ? 1 : 2;
                    }
                    else
                    {
                        // 空格
                        grassMap[i, j] = 0;
                    }
                }
            }
            prePos = passPos;
        }
        // string outPut = "";
        // for (int i = 0; i < 10; i++)
        // {
        //     for (int j = 0; j < 5; j++)
        //     {
        //         outPut += grassMap[i, j] + " ";
        //     }
        //     outPut += "\n";
        // }
        // Debug.LogFormat(outPut);
    }

    void CheckGrassMap()
    {
        for (int i = 0; i < 10; i++)
        {
            if (distence[i] == 1)
            {
                if (i + 1 >= 10) continue;
                if (position[i] == position[i + 1]) continue;
                int start = position[i] < position[i + 1] ? position[i] : position[i + 1];
                int end = position[i] > position[i + 1] ? position[i] : position[i + 1];
                for (int j = start; j <= end; j++)
                {
                    if (grassMap[i, j] == 2)
                    {
                        grassMap[i, j] = Random.Range(0f, 1f) >= 0.5f ? 0 : 1;
                    }
                }
            }
            else
            {
                i++;
            }
        }
    }

    void GenerateGrassMap()
    {
        for (int i = 0; i < 10; i++)
        {
            float offsetY = i * width + transform.position.y; // 一定要加上
            bool isBuilding = false;
            for (int j = 0; j < 5; j++)
            {
                float offsetX = j * width - 2f * width + transform.position.x;
                if (grassMap[i, j] == 1)
                {
                    InstantiateObstacle(offsetX, offsetY, false);
                }
                else if (grassMap[i, j] == 2)
                {
                    InstantiateObstacle(offsetX, offsetY, true);
                }
                else if (grassMap[i, j] == -1)
                {
                    if (j != 4 && grassMap[i, j + 1] == -1 && !isBuilding)
                    {
                        // 生成房子
                        InstantiateBuilding(offsetX, offsetY);
                        isBuilding = true;
                        j++;
                    }
                    else
                    {
                        // 生成树
                        InstantiateTree(offsetX, offsetY);
                        if (Random.Range(0f, 1f) >= 0.7f)
                            InstantiateGrass(offsetX, offsetY);
                    }
                }
                else
                {
                    // 生成草地
                    InstantiateGrass(offsetX, offsetY);
                }
            }
        }
    }
    void InstantiateObstacle(float offsetX, float offsetY, bool isLongJump)
    {
        float rate = Random.Range(0.25f, 0.75f);
        float y = offsetY + (isLongJump ? width : rate * width);
        rate = Random.Range(0f, 0.25f);
        float x = offsetX + rate * width;
        Vector3 position = new Vector3(x, y, 0);
        GameObject instantiated = Random.Range(0f, 1f) > 0.5f ? ObstacleList[0] : ObstacleList[1];
        Instantiate(instantiated, position, Quaternion.identity, ObstacleParent);
    }
    void InstantiateBuilding(float offsetX, float offsetY)
    {
        float rate = Random.Range(0.25f, 0.5f);
        float y = offsetY + rate * width;
        rate = Random.Range(0.25f, 0.75f);
        float x = offsetX + rate * width;
        Vector3 position = new Vector3(x, y, 0);
        GameObject instantiated = Random.Range(0f, 1f) > 0.5f ? BuildingList[0] : BuildingList[1];
        Instantiate(instantiated, position, Quaternion.identity, ObstacleParent);
    }
    void InstantiateTree(float offsetX, float offsetY)
    {
        float rate = Random.Range(0.25f, 0.75f);
        float y = offsetY + rate * width;
        rate = Random.Range(0f, 0.5f);
        float x = offsetX + rate * width;
        Vector3 position = new Vector3(x, y, 0);
        Instantiate(Tree, position, Quaternion.identity, ObstacleParent);
    }
    void InstantiateGrass(float offsetX, float offsetY)
    {
        int num = Random.Range(2, 6);
        for (int i = 0; i < num; i++)
        {
            float rate = Random.Range(0.0f, 1f);
            float y = offsetY + rate * width;
            rate = Random.Range(0f, 1f);
            float x = offsetX + rate * width;
            Vector3 position = new Vector3(x, y, 0);
            GameObject instantiated = Random.Range(0f, 1f) > 0.5f ? GrassList[0] : GrassList[1];
            Instantiate(instantiated, position, Quaternion.identity, ObstacleParent);
        }
        if (Random.Range(0f, 1f) >= 0.8f)
        {
            InstantiateItem(Bush, offsetX, offsetY);
        }
        if (Random.Range(0f, 1f) >= 0.5f)
        {
            InstantiateItem(GrassShadow, offsetX, offsetY);
        }
    }
    void InstantiateItem(GameObject item, float offsetX, float offsetY)
    {
        float rate = Random.Range(0.0f, 1f);
        float y = offsetY + rate * width;
        rate = Random.Range(0f, 1f);
        float x = offsetX + rate * width;
        Vector3 position = new Vector3(x, y, 0);
        Instantiate(item, position, Quaternion.identity, ObstacleParent);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="i">列</param>
    /// <param name="j">行</param>
    /// <returns></returns>
    bool isLRHasObstacle(int i, int j)
    {
        if (i != 0 && i != 4)
        {
            return grassMap[j, i - 1] == 1 || grassMap[j, i + 1] == 1 || grassMap[j, i + 1] == 2 || grassMap[j, i - 1] == 2;
        }
        else if (i != 0)
        {
            return grassMap[j, i - 1] == 1 || grassMap[j, i - 1] == 2;
        }
        else
        {
            return grassMap[j, i + 1] == 1 || grassMap[j, i + 1] == 2;
        }
    }
}


