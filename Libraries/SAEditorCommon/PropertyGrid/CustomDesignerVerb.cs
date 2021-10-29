using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using System;
using System.ComponentModel.Design;
using System.Reflection;

namespace SAModel.SAEditorCommon.PropertyGrid
{
	public class CustomDesignerVerb : DesignerVerb
	{
		public bool IsVerbSpec { get; private set; }
		public MethodInfo Method { get; private set; }
		public Action<SETItem> Action { get; private set; }

		public CustomDesignerVerb(string text, EventHandler handler, MethodInfo method)
			: base(text, handler)
		{
			Method = method;
			IsVerbSpec = false;
		}

		public CustomDesignerVerb(VerbSpec spec, EventHandler handler)
			: base(spec.Name, handler)
		{
			Action = spec.DoVerb;
			IsVerbSpec = true;
		}
	}
}
