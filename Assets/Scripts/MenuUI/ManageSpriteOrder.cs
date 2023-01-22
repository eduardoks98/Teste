using UnityEngine;
using UnityEngine.Rendering;

public class ManageSpriteOrder : MonoBehaviour
{
    [SerializeField]
    private int sortingBase = 1000;
    [SerializeField]
    private int offset = 0;
    [SerializeField]
    private SortingGroup _sortGroup;

    [SerializeField] private bool _destroyOnStart = true;



    public SortingGroup SortGroup { get => _sortGroup; set => _sortGroup = value; }
    public bool DestroyOnStart { get => _destroyOnStart; set => _destroyOnStart = value; }

    private void Awake()
    {
        SortGroup = GetComponent<SortingGroup>();
    }

    private void Start()
    {
        CheckSpriteOrder();
        if (DestroyOnStart) Destroy(this);
    }

    private void Update()
    {
        CheckSpriteOrder();

    }

    private void CheckSpriteOrder()
    {
        if (SortGroup != null) SortGroup.sortingOrder = (int)(sortingBase - (transform.position.y * 100) - offset);

    }

}