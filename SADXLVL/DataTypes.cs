using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;
using System.ComponentModel;

namespace SonicRetro.SAModel.SADXLVL2
{
    public class LevelItem : Item
    {
        [Browsable(false)]
        private COL COL { get; set; }
        [Browsable(false)]
        private Microsoft.DirectX.Direct3D.Mesh Mesh { get; set; }

        public override Vertex Position
        {
            get
            {
                return COL.Model.Position;
            }
            set
            {
                COL.Model.Position = value;
                COL.CalculateBounds();
            }
        }

        public override Rotation Rotation
        {
            get
            {
                return COL.Model.Rotation;
            }
            set
            {
                COL.Model.Rotation = value;
                COL.CalculateBounds();
            }
        }

        public override float CheckHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View)
        {
            Matrix transform = Matrix.Identity;
            transform.Multiply(Matrix.RotationYawPitchRoll(LevelData.BAMSToRad(Rotation.Y), LevelData.BAMSToRad(Rotation.X), LevelData.BAMSToRad(Rotation.Z)));
            transform.Multiply(Matrix.Translation(Position.ToVector3()));
            Vector3 pos = Vector3.Unproject(Near, Viewport, Projection, View, transform);
            Vector3 dir = Vector3.Subtract(pos, Vector3.Unproject(Far, Viewport, Projection, View, transform));
            IntersectInformation info;
            if (!Mesh.Intersect(pos, dir, out info)) return -1;
            return info.Dist;
        }

        [Browsable(true)]
        public void ImportModel()
        {

        }

        [Browsable(true)]
        public void ExportModel()
        {

        }
    }
}
