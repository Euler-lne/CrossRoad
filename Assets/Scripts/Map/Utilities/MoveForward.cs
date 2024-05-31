using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    int dir;
    Vector3 targetPosition;
    public float maxSpeed;
    public float minSpeed;
    float speed;
    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    public void SetDirection(int direction, Vector3 _targetPosition)
    {
        dir = direction;
        transform.localScale = new Vector3(dir, 1, 1);
        targetPosition = _targetPosition;
    }
    void Move()
    {
        if (Vector3.Distance(targetPosition, transform.position) <= 1f)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.position = new Vector3(transform.position.x + speed * Time.deltaTime * dir, transform.position.y, transform.position.z);
        }
    }
}
