using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.Interfaces
{
    interface ICollectable
    {
        public void Collect(string playerID);
    }
}