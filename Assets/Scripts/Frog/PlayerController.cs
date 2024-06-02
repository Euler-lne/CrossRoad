using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public GameObject dirObject;
    public Sprite upSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;
    public GameObject cancelZone;
    private SpriteRenderer dirctionSpriteRenderer;
    private enum Direction
    {
        Up, Left, Right, NoDir
    }
    private Settings.TerrainType terrainType;
    private Direction direction;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2D;
    private Animator anim;
    public float jumpDistance = 2.35f;
    public float jumpLeftOffsetY = 0.25f;
    private Vector2 originPosition;
    private float originColliderOffsetY;
    private float moveDistance;
    private bool isLongJump;
    private bool isJump;
    private bool canJump;
    private bool isJustLand;
    private bool isDead;
    private bool isOnWood;
    private Vector2 destination;
    private Direction jumpDirction;
    private Vector2 touchPosition;
    private RaycastHit2D[] result = new RaycastHit2D[2];
    private int score;
    private const int RiverTestTimes = 3;
    private int riverTestTimer;

    private float shortPressDuration = 0.2f; // 短按持续时间（秒）
    private float longPressDuration = 0.5f;  // 长按持续时间（秒）
    private float longPressTimer = 0f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        dirctionSpriteRenderer = dirObject.GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        originColliderOffsetY = boxCollider2D.offset.y;
        terrainType = Settings.TerrainType.Grass;
        dirObject.SetActive(false);
        jumpDirction = Direction.NoDir;
        riverTestTimer = 0;
    }

    private void Update()
    {
        if (canJump && !isDead)
        {
            isOnWood = false;
            TriggerJump();
        }
        if (isLongJump)
        {
            if (IsInCancelZone())
            {
                cancelZone.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            }
            else
            {
                cancelZone.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.25f);
            }
        }
        // 调用新的触摸检测函数
        CheckJump();
        CheckLongJump();
        UpdateTouchPosition();
    }

    private void FixedUpdate()
    {
        if (isJump && !isDead)
        {
            rb.position = Vector2.Lerp(rb.position, destination, 0.134f);
            if (jumpDirction != Direction.Up)
            {
                float offsetY = originColliderOffsetY + originPosition.y - rb.position.y;
                boxCollider2D.offset = new Vector2(boxCollider2D.offset.x, offsetY);
            }
        }
        else if (isJustLand && !isDead)
        {
            rb.position = destination;
            boxCollider2D.offset = new Vector2(boxCollider2D.offset.x, originColliderOffsetY);
            isJustLand = false;
        }
    }
    private void OnDestroy()
    {
        if (!isDead)
        {
            isDead = true;
            EventHandler.CallFrogDead();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isJump)
        {
            if (other.CompareTag("Border"))
            {
                isDead = true;
                Debug.Log("Border");
            }
            riverTestTimer = 0;
        }
        else
        {
            if (other.CompareTag("River"))
            {
                Physics2D.RaycastNonAlloc(transform.position, Vector2.one * 0.1f, result, 0.5f);
                foreach (var hit in result)
                {
                    if (hit.collider == null) continue;
                    if (hit.collider.CompareTag("Wood"))
                    {
                        float width = hit.collider.bounds.size.x;
                        if (Mathf.Abs(transform.position.x - hit.collider.transform.position.x) < width / 2 - 0.2f)
                        {
                            transform.parent = hit.collider.transform;
                            isOnWood = true;
                            riverTestTimer = 0;
                            break;
                        }
                    }
                }
                if (!isOnWood)
                {
                    if (riverTestTimer >= RiverTestTimes)
                    {
                        isDead = true;
                        Debug.Log("Water");
                    }
                    else
                    {
                        riverTestTimer++;
                    }
                }
            }
            if (other.CompareTag("Obstacle"))
            {
                isDead = true;
                Debug.Log("Obstacle");
            }
        }
        if (isDead)
        {
            EventHandler.CallFrogDead();
            boxCollider2D.enabled = false;
            anim.speed = 0;
        }
    }
    public bool IsJump() { return isJump; }
    public bool IsFaceUp() { return jumpDirction == Direction.Up; }
    private void UpdateXValude()
    {
        if (terrainType != Settings.TerrainType.River && jumpDirction != Direction.Up)
        {
            float num = transform.position.x / jumpDistance;

            if (!Mathf.Approximately(num, Mathf.Round(num)))
            {
                moveDistance += (Mathf.Round(num) - num) * jumpDistance;
            }
        }
    }
    private void CheckJump()
    {
        if (isJump || isDead) return;

        originPosition = transform.position;

        if (Input.touchCount > 0)
        {
            UnityEngine.Touch touch = Input.GetTouch(0); // 指定 UnityEngine 命名空间
            if (touch.phase == UnityEngine.TouchPhase.Began) // 指定 UnityEngine 命名空间
            {
                longPressTimer = 0f; // 重置长按计时器
            }
            else if (touch.phase == UnityEngine.TouchPhase.Stationary) // 指定 UnityEngine 命名空间
            {
                longPressTimer += Time.deltaTime;
            }
            else if (touch.phase == UnityEngine.TouchPhase.Ended) // 指定 UnityEngine 命名空间
            {
                if (longPressTimer < shortPressDuration)
                {
                    // 短按触发跳跃
                    moveDistance = jumpDistance;
                    UpdateXValude();
                    canJump = true;
                    AudioManager.Instance.SetJumpClip(0);
                    dirObject.SetActive(true);
                    SetDirectionSprite();
                }
            }
        }
    }

    private void CheckLongJump()
    {
        if (isJump || isDead) return;

        originPosition = transform.position;

        if (Input.touchCount > 0)
        {
            UnityEngine.Touch touch = Input.GetTouch(0); // 指定 UnityEngine 命名空间
            if (touch.phase == UnityEngine.TouchPhase.Began) // 指定 UnityEngine 命名空间
            {
                longPressTimer = 0f; // 重置长按计时器
            }
            else if (touch.phase == UnityEngine.TouchPhase.Stationary) // 指定 UnityEngine 命名空间
            {
                longPressTimer += Time.deltaTime;
                if (longPressTimer >= longPressDuration)
                {
                    // 长按触发长跳
                    moveDistance = jumpDistance * 2;
                    UpdateXValude();
                    isLongJump = true;
                    dirObject.SetActive(true);
                    SetDirectionSprite();
                    cancelZone.SetActive(true);
                }
            }
            else if (touch.phase == UnityEngine.TouchPhase.Ended) // 指定 UnityEngine 命名空间
            {
                if (longPressTimer < longPressDuration)
                {
                    // 短按触发跳跃
                    moveDistance = jumpDistance;
                    UpdateXValude();
                    canJump = true;
                    AudioManager.Instance.SetJumpClip(0);
                    dirObject.SetActive(true);
                    SetDirectionSprite();
                }
                else if (isLongJump)
                {
                    // 长按结束
                    if (!IsInCancelZone())
                    {
                        canJump = true;
                        AudioManager.Instance.SetJumpClip(1);
                    }
                    dirObject.SetActive(false);
                    cancelZone.SetActive(false);
                    isLongJump = false;
                }
            }
        }
    }
    private void UpdateTouchPosition()
    {
        if (isDead) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            var offset = touchPosition.x;

            if (!isJump)
            {
                if (Mathf.Abs(offset) / (2.35f * 2) <= 0.5f)
                {
                    direction = Direction.Up;
                }
                else if (offset > 0)
                {
                    direction = Direction.Right;
                }
                else
                {
                    direction = Direction.Left;
                }
                SetDirectionSprite();
                jumpDirction = direction;
            }
            else
            {
                direction = Direction.NoDir;
            }
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isJump || isDead) return;
        originPosition = transform.position;
        if (context.performed)
        {
            moveDistance = jumpDistance;
            UpdateXValude();
            canJump = true;
            AudioManager.Instance.SetJumpClip(0);
            dirObject.SetActive(true);
            SetDirectionSprite();
        }
    }
    public void LongJump(InputAction.CallbackContext context)
    {
        if (isJump || isDead) return;
        originPosition = transform.position;
        if (context.performed)
        {
            // 进入执行
            moveDistance = jumpDistance * 2;
            UpdateXValude();
            isLongJump = true;
            dirObject.SetActive(true);
            SetDirectionSprite();
            cancelZone.SetActive(true);
        }
        if (context.canceled && isLongJump)
        {
            // 退出执行，同时监测到键盘的抬起，键盘抬起就会调用，不会管是否执行了performed
            isLongJump = false;
            if (!IsInCancelZone())
            {
                canJump = true;
                AudioManager.Instance.SetJumpClip(1);
            }
            dirObject.SetActive(false);
            cancelZone.SetActive(false);
        }
    }
    public void GetTouchPosition(InputAction.CallbackContext context)
    {
        // 得到的value是手机的像素点的坐标，也就是相机的坐标，要转换为游戏世界坐标
        if (isDead) return;
        touchPosition = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
        var offset = touchPosition.x;
        if (!isJump)
        {
            if (Mathf.Abs(offset) / (2.35f * 2) <= 0.5f)
            {
                direction = Direction.Up;
            }
            else if (offset > 0)
            {
                direction = Direction.Right;
            }
            else
            {
                direction = Direction.Left;
            }
            SetDirectionSprite();
            jumpDirction = direction;
        }
        else
        {
            direction = Direction.NoDir;
        }
    }
    private bool IsInCancelZone()
    {
        RaycastHit2D[] result = new RaycastHit2D[10];
        Physics2D.RaycastNonAlloc(touchPosition, Vector2.one * 0.1f, result, 0.5f);
        foreach (var hit in result)
        {
            if (hit.collider == null) continue;
            if (hit.collider.CompareTag("CancelJump"))
            {
                return true;
            }
        }
        return false;
    }
    private void SetDirectionSprite()
    {
        // 一定要注意图片的翻转问题！！！
        dirctionSpriteRenderer.sprite = direction switch
        {
            Direction.Left => leftSprite,
            Direction.Right => rightSprite,
            Direction.Up => upSprite,
            _ => null,
        };
        if (isLongJump)
        {
            dirctionSpriteRenderer.color = new Color(1, 1, 1, 1);
        }
        else
        {
            dirctionSpriteRenderer.color = new Color(1, 1, 1, 0.5f);
        }
    }


    /// <summary>
    /// 触发执行跳跃动画
    /// </summary>
    private void TriggerJump()
    {
        transform.parent = null; // 一定要设置回去
        dirObject.SetActive(false);
        switch (jumpDirction)
        {
            case Direction.Up:
                anim.SetBool("IsSide", false);
                destination = new Vector2(originPosition.x, originPosition.y + moveDistance);
                break;
            case Direction.Left:
                transform.localScale = new Vector2(1, 1);
                dirObject.transform.localScale = new Vector2(1, 1);
                anim.SetBool("IsSide", true);
                destination = new Vector2(originPosition.x - moveDistance / 2, originPosition.y + jumpLeftOffsetY);
                break;
            case Direction.Right:
                transform.localScale = new Vector2(-1, 1);
                dirObject.transform.localScale = new Vector2(-1, 1);
                anim.SetBool("IsSide", true);
                destination = new Vector2(originPosition.x + moveDistance / 2, originPosition.y + jumpLeftOffsetY);
                break;
            default:
                break;
        }
        anim.SetTrigger("Jump");
        canJump = false;
    }
    #region 动画事件
    public void JumpAnimationEvent()
    {
        AudioManager.Instance.PlayJumpFx();
        isJump = true;
    }
    public void JumpLeftAnimationEvent()
    {
        switch (jumpDirction)
        {
            case Direction.Left:
                destination = new Vector2(originPosition.x - moveDistance, originPosition.y);
                break;
            case Direction.Right:
                destination = new Vector2(originPosition.x + moveDistance, originPosition.y);
                break;
            default:
                break;
        }
    }
    public void FinishJumpAnimationEvent()
    {
        isJump = false;
        isJustLand = true;
        if (jumpDirction == Direction.Up)
        {
            terrainType = EventHandler.CallFrogJumped(transform.position.y);
            if (!isDead)
            {
                score += isLongJump ? 15 : 10;
                EventHandler.CallFrogJunpSucceed(score);
            }
        }
    }
    #endregion
}
