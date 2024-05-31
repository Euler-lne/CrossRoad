using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarGenerator : MonoBehaviour
{
    public Transform[] upPosition;
    public Transform[] downPosition;
    public GameObject[] cars;
    public float maxTimeSlot;
    public float minTimeSlot;
    float upTimer = 0f;
    float upTimeSlot = 8f;
    float downTimer = 0f;
    float downTimeSlot = 10f;
    bool upIsLeft;
    bool downIsLeft;

    void Start()
    {
        upIsLeft = Random.Range(0f, 1f) <= 0.5;
        GenerateCar(upPosition, upIsLeft);
        downIsLeft = Random.Range(0f, 1f) <= 0.5;
        GenerateCar(downPosition, downIsLeft);
    }
    private void Update()
    {
        if (upTimer >= upTimeSlot)
        {
            GenerateCar(upPosition, upIsLeft);
            upTimer = 0f;
            upTimeSlot = Random.Range(minTimeSlot, maxTimeSlot);
        }
        else if (downTimer >= downTimeSlot)
        {
            GenerateCar(downPosition, downIsLeft);
            downTimer = 0f;
            downTimeSlot = Random.Range(minTimeSlot, maxTimeSlot);
        }
        else
        {
            upTimer += Time.deltaTime;
            downTimer += Time.deltaTime;
        }
    }
    void GenerateCar(Transform[] position, bool isLeft)
    {
        int index = Random.Range(0, cars.Length);
        GameObject carPrefab = cars[index];
        Vector3 carPosition = position[1].position;
        Vector3 targetPosition = position[0].position;
        if (isLeft)
        {
            carPosition = position[0].position;
            targetPosition = position[1].position;
        }
        GameObject car = Instantiate(carPrefab, carPosition, Quaternion.identity, transform);
        int dir = isLeft ? 1 : -1;
        car.GetComponent<MoveForward>().SetDirection(dir, targetPosition);
    }
}
