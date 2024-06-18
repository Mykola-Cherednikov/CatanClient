using TMPro;
using UnityEngine;

public class NumberToken : Place
{
    [SerializeField] public TMP_Text numberText;

    [SerializeField] private Sprite banditSprite;
    [SerializeField] private Sprite numberTokenSprite;

    public bool isBandit;

    public int numberToken;

    protected override void Awake()
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
        isBandit = true;
        numberText.enabled = false;
    }

    public void SetBanditOff()
    {
        spriteRenderer.sprite = numberTokenSprite;
        isBandit = false;
        numberText.enabled = true;
    }
}
