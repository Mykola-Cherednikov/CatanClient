using TMPro;
using UnityEngine;

public class NumberToken : MonoBehaviour
{
    [SerializeField] public TMP_Text numberText;

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Collider2D collider2d;

    [SerializeField] private Sprite banditSprite;

    [SerializeField] private Sprite numberTokenSprite;

    public bool isBandit;

    public int numberToken;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider2d = GetComponent<Collider2D>();
        HideCollider();
        SetBanditOff();
    }

    public void HideCollider()
    {
        collider2d.enabled = false;
    }

    public void ShowCollider()
    {
        collider2d.enabled = true;
    }

    public void SetBanditOn()
    {
        spriteRenderer.sprite = banditSprite;
        numberText.enabled = false;
    }

    public void SetBanditOff()
    {
        spriteRenderer.sprite = numberTokenSprite;
        numberText.enabled = true;
    }
}
