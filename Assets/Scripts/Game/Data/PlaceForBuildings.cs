using UnityEngine;

public class PlaceForBuildings : MonoBehaviour
{
    public int id;

    public User user;

    protected SpriteRenderer sprite;

    protected Collider2D collider2d;

    protected virtual void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        collider2d = GetComponent<Collider2D>();
        user = null;
        HideSpriteAndCollider();
    }

    public void ShowSpriteAndCollider()
    {
        sprite.enabled = true;
        collider2d.enabled = true;
    }

    public void HideSpriteAndCollider()
    {
        sprite.enabled = false;
        collider2d.enabled = false;
    }
}

