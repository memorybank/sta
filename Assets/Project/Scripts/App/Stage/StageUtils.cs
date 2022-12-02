using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Playa.App.Stage;
using Playa.Item;

namespace Playa.App
{

    public class StageUtils
    {
        public BaseItem _Item;
        public StageApi _StageManager => _Item._BaseApp._StageManager;

        public StageUtils(BaseItem item)
        {
            _Item = item;
        }

        public void ExecuteCmd(StageCmd cmd)
        {
            if (cmd.GetType() == typeof(InstantiateObjectCmd))
            {
                var tcmd = (InstantiateObjectCmd)cmd;
                GameObject obj;
                if (tcmd.refObj != null)
                {
                    obj = _StageManager.InstantiateObject(_Item.ItemId, tcmd.assetPath, tcmd.refObj, tcmd.position, tcmd.rotation, tcmd.strategy);
                }
                else
                {
                    obj = _StageManager.InstantiateObject(_Item.ItemId, tcmd.assetPath, tcmd.position, tcmd.rotation, tcmd.strategy, tcmd.transformNames);
                }
                obj.transform.SetParent(_Item._BaseApp.transform);
                _Item._Objects.Add(tcmd.name, obj);
            }
        }

    }

}