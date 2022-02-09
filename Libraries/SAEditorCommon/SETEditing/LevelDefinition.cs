using SharpDX.Direct3D9;
using SAModel.Direct3D;

namespace SAModel.SAEditorCommon.SETEditing
{
	public abstract class LevelDefinition
	{
		public abstract void Init(IniLevelData data, byte act, byte timeofday);
		public abstract void Render(Device dev, EditorCamera cam);
		public virtual void RenderLate(Device dev, EditorCamera cam)
		{
			return;
		}
	}
}
