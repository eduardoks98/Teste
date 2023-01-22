using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VectorForestScenery
{
    public class Scenery : MonoBehaviour
    {
        [SerializeField] private List<SceneryItem> _item;
        public List<SceneryItem> Items { get => _item; private set => _item = value; }

        private void Awake()
        {
            Items = new List<SceneryItem>(GetComponentsInChildren<SceneryItem>());
        }

        public void Add(Vector2 origin, SceneryItem prefab)
        {
            SceneryItem item = Instantiate<SceneryItem>(prefab, origin, Quaternion.identity);
            item.transform.SetParent(transform);
            Items.Add(item);
        }

        public void Clear()
        {
            foreach (var item in Items)
            {
                Destroy(item.gameObject);
            }

            Items.Clear();
        }
    }
}