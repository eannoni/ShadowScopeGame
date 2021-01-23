using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    public int id;
    [SerializeField] bool active;
    public float inactiveTime;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetActive(false);
    }

    public void SetActive(bool setActive)
    {
        if (setActive)
        {
            active = true;
            spriteRenderer.gameObject.SetActive(true);
        }
        else
        {
            active = false;
            spriteRenderer.gameObject.SetActive(false);
            inactiveTime = 0f;
        }
    }

    public bool IsActive()
    {
        return active;
    }
}
