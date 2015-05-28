using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;

namespace SonicRetro.SAModel.SAEditorCommon.SETEditing
{
    public abstract class LevelDefinition
    {
        public abstract void Init(IniLevelData data, byte act, Device dev);
        public abstract void Render(Device dev, EditorCamera cam);
    }
}
