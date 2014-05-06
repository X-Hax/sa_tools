using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;

namespace SonicRetro.SAModel.SAEditorCommon.PropertyGrid
{
    public class CustomDesignerVerb : DesignerVerb
    {
        public MethodInfo Method { get; set; }

        public CustomDesignerVerb(string text, EventHandler handler, MethodInfo method)
            : base(text, handler)
        {
            Method = method;
        }
    }
}
