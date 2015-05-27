using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SonicRetro.SAModel.Direct3D
{
    public class HitResult
    {
        public NJS_OBJECT Model { get; private set; }
        public float Distance { get; private set; }
        public bool IsHit { get; private set; }

        private HitResult() { }

        public HitResult(NJS_OBJECT model, float distance)
        {
            Model = model;
            Distance = distance;
            IsHit = true;
        }

        public static readonly HitResult NoHit = new HitResult();

        public static HitResult Min(HitResult a, HitResult b)
        {
            if (!a.IsHit) return b;
            if (!b.IsHit) return a;
            if (a.Distance < b.Distance) return a;
            else return b;
        }
    }
}