using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Scriptables;
using Assets.Scripts.Interfaces;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
public class Item : MonoBehaviour, ICollectable
{

    public static Action<ItemData, string> OnItemCollected;
    public ItemData itemData;

    [SerializeField] private CircleCollider2D _circleCollider2D;
    [SerializeField] private SpriteRenderer _spriteRenderer;


    public SpriteRenderer SpriteRenderer { get => _spriteRenderer; set => _spriteRenderer = value; }
    public CircleCollider2D CircleCollider2D { get => _circleCollider2D; set => _circleCollider2D = value; }


    public void Collect(string playerID)
    {
        Destroy(gameObject);
        OnItemCollected?.Invoke(itemData, playerID);
    }

    private void OnValidate()
    {
        SetupRequireds();
    }
    private void OnEnable()
    {
        SetupRequireds();
    }

    private void SetupRequireds()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        if (SpriteRenderer != null) SpriteRenderer.sprite = itemData.sprite;

        CircleCollider2D = GetComponent<CircleCollider2D>();
        if (CircleCollider2D != null)
        {
            CircleCollider2D.isTrigger = true;
            CircleCollider2D.radius = 1;
        }
    }

    



}
