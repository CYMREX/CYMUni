using CYM.AI;
using CYM.Sense;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CYM
{
    public partial class BaseUnit : BaseCoreMono
    {
        public List<ISenseMgr> SenseMgrs { get; protected set; } = new List<ISenseMgr>();
        public ISenseMgr SenseMgr { get; protected set; }
        public BaseDetectionMgr DetectionMgr { get; protected set; }
    }
}