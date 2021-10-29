using SharpDX.Direct3D9;
using SAModel.Direct3D;

namespace SAModel.SAEditorCommon.SETEditing
{
	public abstract class LevelDefinition
	{
		public abstract void Init(IniLevelData data, byte act);
		public abstract void Render(Device dev, EditorCamera cam);
	}
}
