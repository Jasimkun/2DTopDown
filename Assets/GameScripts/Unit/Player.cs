using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite downSprite;
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite rightSprite;
    [SerializeField] private float speed = 1f;
    private PlayerController controller;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigid;
    private Animator animator;

    //추가
    private Tilemap groundTilemap;
    private TileBase bossGroundTile;

    void Start()
    {
        controller = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        //bossGroundTile과 groundTilemap 초기화
        MapGeneratorIssac generator = FindObjectOfType<MapGeneratorIssac>();
        if (generator != null)
        {
            groundTilemap = generator.groundTilemap;
            bossGroundTile = generator.bossGroundTile;
        }
    }
    void UpdateSprite(Vector2 direction)
    {
        if (direction.y > 0) // 위로 이동
            spriteRenderer.sprite = upSprite;
        else if (direction.y < 0) // 아래로 이동
            spriteRenderer.sprite = downSprite;
        else if (direction.x > 0) // 오른쪽 이동
            spriteRenderer.sprite = rightSprite;
        else if (direction.x < 0) // 왼쪽 이동
            spriteRenderer.sprite = leftSprite;
    }

    void Update()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 moveVelocity = moveInput.normalized * speed;
        controller.Move(moveVelocity);

        UpdateSprite(moveInput);

        //현재 발 아래 타일 확인
        if (groundTilemap != null && bossGroundTile != null)
        {
            Vector3Int tilePos = groundTilemap.WorldToCell(transform.position);
            TileBase currentTile = groundTilemap.GetTile(tilePos);

            if (currentTile == bossGroundTile)
            {
                SceneManager.LoadScene("Clear");

            }
        }
    }



    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }
}
