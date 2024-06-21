using Unity.VisualScripting;
using UnityEngine;

public class Place : MonoBehaviour
{
    public int id;

    [SerializeReference] public User user;

    public SpriteRenderer spriteRenderer;

    public Collider2D collider2d;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider2d = GetComponent<Collider2D>();
        user = null;
        HideSpriteAndCollider();
    }

    public void ShowSpriteAndCollider()
    {
        spriteRenderer.enabled = true;
        collider2d.enabled = true;
    }

    public void HideSpriteAndCollider()
    {
        spriteRenderer.enabled = false;
        collider2d.enabled = false;
    }
}

