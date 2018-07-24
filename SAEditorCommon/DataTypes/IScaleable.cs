using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SA_Tools;

namespace SonicRetro.SAModel.SAEditorCommon.DataTypes
{
    public interface IScaleable
    {
        Vertex GetScale();
        void SetScale(Vertex scale);        
    }
}
