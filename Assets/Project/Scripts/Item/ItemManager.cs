using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;
using Playa.Avatars;

namespace Playa.Item
{
    using ItemId = UInt64;

    public class ItemManager : MonoBehaviour
    {
        public SortedDictionary<ItemId, BaseItem> _Items;
        public Dictionary<string, List<ItemId>> _ItemIndicesByName;
        public Dictionary<string, List<ItemId>> _ItemIndicesByTag;

        // Note that _CurrentId is monotonically increasing
        private static ItemId _CurrentId = 0;
        private ItemId _StageItemId = 0;
        // Composition
        [SerializeField] private ItemEventManager _ItemEventManager;

        public ItemFactory ItemFactory;

        void Awake()
        {
            _Items = new SortedDictionary<ItemId, BaseItem>();
            _ItemIndicesByName = new Dictionary<string, List<ItemId>>();
            _ItemIndicesByTag = new Dictionary<string, List<ItemId>>();
        }

        public ItemId Register(BaseItem item, AvatarUser u)
        {

            //comment: u param is used for not fix affectavataruser
            ItemId id = _CurrentId++;
            if (item.ItemProperties.EffectArea == ItemEffectArea.Stage)
            {
                ChangeStageItem(id);
            }
            _Items.Add(id, item);
            if (!_ItemIndicesByName.ContainsKey(item.ItemProperties.Name))
            {
                _ItemIndicesByName.Add(item.ItemProperties.Name, new List<ItemId>());
            }
            _ItemIndicesByName[item.ItemProperties.Name].Add(id);
            foreach (var tag in item.ItemProperties.Tags)
            {
                if (!_ItemIndicesByTag.ContainsKey(tag))
                {
                    _ItemIndicesByTag.Add(tag, new List<ItemId>());
                }
                _ItemIndicesByTag[tag].Add(id);
            }
            
            return id;
        }

        public void ChangeStageItem(ItemId id)
        {
            DeactivateAll();
            _StageItemId = id;
        }

        public BaseItem FindStageItem()
        {
            return _Items[_StageItemId];
        }

        public BaseItem FindItemById(ItemId id)
        {
            if (_Items.ContainsKey(id))
            {
                return _Items[id];
            }
            return null;
        }

        public void DeactivateItemName(string name)
        {
            if (_ItemIndicesByName.ContainsKey(name))
            {
                _ItemIndicesByName[name] = new List<ItemId>();
                _ItemIndicesByName.Remove(name);
            }
        }

        public BaseItem FindItemByName(string name)
        {
            if (_ItemIndicesByName.ContainsKey(name))
            {
                return _Items[_ItemIndicesByName[name][0]];
            }
            return null;
        }

        public BaseItem FindItemByTag(string tag)
        {
            if (_ItemIndicesByTag.ContainsKey(tag))
            {
                return _Items[_ItemIndicesByTag[tag][0]];
            }
            return null;
        }

        public void DeactivateAll()
        {
            // Always deactivate in reverse order
            foreach (var item in _Items.Reverse())
            {
                item.Value.Deactivate();
            }
            _Items.Clear();
            _ItemIndicesByName.Clear();
            _ItemIndicesByTag.Clear();
        }

        public void DeactivateAllExcept(ItemId id)
        {
            var theOne = _Items[id];

            // Always deactivate in reverse order
            foreach (var item in _Items.Reverse())
            {
                if (item.Value.ItemId == id)
                {
                    Debug.Log("Deactivate all skipping " + item.Value.ItemProperties.Name);
                    continue;
                }
                item.Value.Deactivate();
            }
            _Items.Clear();
            _ItemIndicesByName.Clear();
            _ItemIndicesByTag.Clear();

            _Items[id] = theOne;
            _ItemIndicesByName[theOne.ItemProperties.Name] = new List<ItemId> { theOne.ItemId };
            foreach (var tag in theOne.ItemProperties.Tags)
            {
                _ItemIndicesByTag[tag] = new List<ItemId> { theOne.ItemId };
            }
        }
    }
}