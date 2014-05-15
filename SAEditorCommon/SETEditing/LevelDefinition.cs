using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using SonicRetro.SAModel.Direct3D;

namespace SonicRetro.SAModel.SAEditorCommon.SETEditing
{
    public abstract class LevelDefinition
    {
        public abstract void Init(Dictionary<string, string> data, byte act, Device dev);
        public abstract void Render(Device dev, Camera cam);
    }
}
