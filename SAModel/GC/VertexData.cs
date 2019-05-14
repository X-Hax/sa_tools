using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SonicRetro.SAModel;

namespace SonicRetro.SAModel.GC
{
    public class VertexData
    {
        private List<GXVertexAttribute> m_Attributes;

        public List<Vector3> Positions { get; private set; }
        public List<Vector3> Normals { get; private set; }
        public List<Color> Color_0 { get; private set; }
        public List<Color> Color_1 { get; private set; }
        public List<Vector2> TexCoord_0 { get; private set; }
        public List<Vector2> TexCoord_1 { get; private set; }
        public List<Vector2> TexCoord_2 { get; private set; }
        public List<Vector2> TexCoord_3 { get; private set; }
        public List<Vector2> TexCoord_4 { get; private set; }
        public List<Vector2> TexCoord_5 { get; private set; }
        public List<Vector2> TexCoord_6 { get; private set; }
        public List<Vector2> TexCoord_7 { get; private set; }

        public VertexData()
        {
            m_Attributes = new List<GXVertexAttribute>();
            Positions = new List<Vector3>();
            Normals = new List<Vector3>();
            Color_0 = new List<Color>();
            Color_1 = new List<Color>();
            TexCoord_0 = new List<Vector2>();
            TexCoord_1 = new List<Vector2>();
            TexCoord_2 = new List<Vector2>();
            TexCoord_3 = new List<Vector2>();
            TexCoord_4 = new List<Vector2>();
            TexCoord_5 = new List<Vector2>();
            TexCoord_6 = new List<Vector2>();
            TexCoord_7 = new List<Vector2>();
        }

        public bool CheckAttribute(GXVertexAttribute attribute)
        {
            if (m_Attributes.Contains(attribute))
                return true;
            else
                return false;
        }

        public object GetAttributeData(GXVertexAttribute attribute)
        {
            if (!CheckAttribute(attribute))
                return null;

            switch (attribute)
            {
                case GXVertexAttribute.Position:
                    return Positions;
                case GXVertexAttribute.Normal:
                    return Normals;
                case GXVertexAttribute.Color0:
                    return Color_0;
                case GXVertexAttribute.Color1:
                    return Color_1;
                case GXVertexAttribute.Tex0:
                    return TexCoord_0;
                case GXVertexAttribute.Tex1:
                    return TexCoord_1;
                case GXVertexAttribute.Tex2:
                    return TexCoord_2;
                case GXVertexAttribute.Tex3:
                    return TexCoord_3;
                case GXVertexAttribute.Tex4:
                    return TexCoord_4;
                case GXVertexAttribute.Tex5:
                    return TexCoord_5;
                case GXVertexAttribute.Tex6:
                    return TexCoord_6;
                case GXVertexAttribute.Tex7:
                    return TexCoord_7;
                default:
                    throw new ArgumentException("attribute");
            }
        }

        public void SetAttributeData(GXVertexAttribute attribute, object data)
        {
            if (!CheckAttribute(attribute))
                m_Attributes.Add(attribute);

            switch (attribute)
            {
                case GXVertexAttribute.Position:
                    if (data.GetType() != typeof(List<Vector3>))
                        throw new ArgumentException("position data");
                    else
                        Positions = (List<Vector3>)data;
                    break;
                case GXVertexAttribute.Normal:
                    if (data.GetType() != typeof(List<Vector3>))
                        throw new ArgumentException("normal data");
                    else
                        Normals = (List<Vector3>)data;
                    break;
                case GXVertexAttribute.Color0:
                    if (data.GetType() != typeof(List<Color>))
                        throw new ArgumentException("color0 data");
                    else
                        Color_0 = (List<Color>)data;
                    break;
                case GXVertexAttribute.Color1:
                    if (data.GetType() != typeof(List<Color>))
                        throw new ArgumentException("color1 data");
                    else
                        Color_1 = (List<Color>)data;
                    break;
                case GXVertexAttribute.Tex0:
                    if (data.GetType() != typeof(List<Vector2>))
                        throw new ArgumentException("texcoord0 data");
                    else
                        TexCoord_0 = (List<Vector2>)data;
                    break;
                case GXVertexAttribute.Tex1:
                    if (data.GetType() != typeof(List<Vector2>))
                        throw new ArgumentException("texcoord1 data");
                    else
                        TexCoord_1 = (List<Vector2>)data;
                    break;
                case GXVertexAttribute.Tex2:
                    if (data.GetType() != typeof(List<Vector2>))
                        throw new ArgumentException("texcoord2 data");
                    else
                        TexCoord_2 = (List<Vector2>)data;
                    break;
                case GXVertexAttribute.Tex3:
                    if (data.GetType() != typeof(List<Vector2>))
                        throw new ArgumentException("texcoord3 data");
                    else
                        TexCoord_3 = (List<Vector2>)data;
                    break;
                case GXVertexAttribute.Tex4:
                    if (data.GetType() != typeof(List<Vector2>))
                        throw new ArgumentException("texcoord4 data");
                    else
                        TexCoord_4 = (List<Vector2>)data;
                    break;
                case GXVertexAttribute.Tex5:
                    if (data.GetType() != typeof(List<Vector2>))
                        throw new ArgumentException("texcoord5 data");
                    else
                        TexCoord_5 = (List<Vector2>)data;
                    break;
                case GXVertexAttribute.Tex6:
                    if (data.GetType() != typeof(List<Vector2>))
                        throw new ArgumentException("texcoord6 data");
                    else
                        TexCoord_6 = (List<Vector2>)data;
                    break;
                case GXVertexAttribute.Tex7:
                    if (data.GetType() != typeof(List<Vector2>))
                        throw new ArgumentException("texcoord7 data");
                    else
                        TexCoord_7 = (List<Vector2>)data;
                    break;
            }
        }

        public void SetAttributesFromList(List<GXVertexAttribute> attributes)
        {
            m_Attributes = new List<GXVertexAttribute>(attributes);
        }
    }
}
