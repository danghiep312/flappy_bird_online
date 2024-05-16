using System;
using Riptide;
using Sirenix.OdinInspector;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    public int Id;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Player player;
    public float jumpSpeed;
    [ReadOnly] public float moveSpeed = 2f;
    public bool IsLocal { get; set; }
    public bool IsAlive { get; set; }

    private Transform _cam;
    private void OnValidate()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        player = GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Score"))
        {
            if (ReplayController.Instance.scores.TryGetValue(Id, out PlayerScore scoreValue))
            {
                scoreValue.score++;
            }
        }
        
        if (other.CompareTag("Star"))
        {
            if (ReplayController.Instance.scores.TryGetValue(Id, out PlayerScore scoreValue))
            {
                scoreValue.score += 2;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            IsAlive = false;
        }
    }

    private void Start()
    {
        _cam = Camera.main.transform;
        this.RegisterListener(EventID.EndRecord, OnEndRecord);
        IsAlive = true;
    }

    private void OnDestroy()
    {
        this.RemoveListener(EventID.EndRecord, OnEndRecord);
    }

    private void OnEndRecord(object obj)
    {
        Destroy(gameObject);
    }

    public void Jump()
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
    }

    private void Update()
    {
        if (IsAlive)
            transform.position += Vector3.right * (moveSpeed * Time.deltaTime);
        if (IsLocal)
            _cam.position = Vector3.right * (transform.position.x + 4f) + Vector3.forward * -10f;
    }

    public void Acceleration()
    {
        moveSpeed = moveSpeed < 3f ? 3.5f : 2f;
    }
}