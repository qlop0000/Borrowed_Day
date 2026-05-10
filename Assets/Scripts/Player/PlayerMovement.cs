using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // 이동 속도
    public bool canMove = true; // 대화 체크

    private Rigidbody2D rb;
    private Vector2 movement;

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }
    void Awake()
    {
        // 오브젝트에 붙어있는 Rigidbody2D 컴포넌트 가져오기
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!canMove)
        {
            movement = Vector2.zero; // 움직임 멈춤
            anim.SetFloat("Speed", 0); // 대화 중에 애니메이션 멈추기
            return;
        }
        // 입력
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        
        anim.SetFloat("Speed", movement.magnitude);

        float hInput = Input.GetAxisRaw("Horizontal");
        // 좌우 반전 (SpriteRenderer의 FlipX 사용)
        if (hInput != 0)
        {
            GetComponent<SpriteRenderer>().flipX = (hInput < 0);
        }
    }

    void FixedUpdate()
    {
        // 물리 이동 처리 (프레임에 상관없이 일정한 속도로 이동)
        // .normalized = 대각선 이동 시 속도가 빨라는 것을 방지
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}