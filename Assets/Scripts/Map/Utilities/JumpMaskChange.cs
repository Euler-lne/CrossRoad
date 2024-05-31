using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpMaskChange : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<PlayerController>().IsJump() && other.GetComponent<PlayerController>().IsFaceUp())
            {
                spriteRenderer.sortingOrder = -1;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && spriteRenderer.sortingOrder == -1)
        {
            if (!(other.GetComponent<PlayerController>().IsJump() && other.GetComponent<PlayerController>().IsFaceUp()))
                spriteRenderer.sortingOrder = 0;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spriteRenderer.sortingOrder = 0;
        }
    }
}
