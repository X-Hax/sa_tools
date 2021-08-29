using SAModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelTool
{
	partial class Program
	{
        // From https://stackoverflow.com/a/8809437
        public static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static string MakeLabel(string label_object, string label_destination)
        {
            return ReplaceFirst(label_object, "object", label_destination);
        }

        public static string MakeLabelWithID(string label_object, string label_destination, int index)
        {
            return ReplaceFirst(label_object, "object", label_destination + "_" + index.ToString());
        }

        static void GenerateLabels(NJS_OBJECT obj, string label)
        {
            Console.WriteLine("Object: {0}", label);
            obj.Name = label;
            Console.WriteLine("Object: {0}", label);
            if (obj.Attach != null)
            {
                Console.WriteLine("Attach: {0}", MakeLabel(label, "model"));
                obj.Attach.Name = MakeLabel(label, "model");
                if (obj.Attach is BasicAttach batt)
                {
                    batt.NormalName = MakeLabel(label, "normal");
                    batt.VertexName = MakeLabel(label, "point");
                    batt.MeshName=MakeLabel(label, "meshset");
                    batt.MaterialName= MakeLabel(label, "material");
                    if (batt.Mesh != null && batt.Mesh.Count > 0)
                    {
                        foreach (NJS_MESHSET mesh in batt.Mesh)
						{
							Console.WriteLine("Mesh {0}", batt.Mesh.IndexOf(mesh));
							if (mesh.Poly != null)
                                mesh.PolyName = MakeLabelWithID(label, "pgS", mesh.MaterialID);
                            if (mesh.UV != null)
                                mesh.UVName = MakeLabelWithID(label, "vuvS", mesh.MaterialID);
                            if (mesh.VColor != null)
                                mesh.VColorName = MakeLabelWithID(label, "vcS", mesh.MaterialID);
                            if (mesh.PolyNormal != null)
                                mesh.PolyNormalName = MakeLabelWithID(label, "pnS", mesh.MaterialID);
                        }
                    }
                }
            }
        }

        static void GenerateLabels(NJS_MOTION mot, string motname, NJS_OBJECT obj)
        {
            Console.WriteLine("Motion for object: {0}", obj.Name);
            string genname = obj.Name;
            mot.Name = MakeLabel(genname, "motion_" + motname);
            mot.MdataName = MakeLabel(genname, "mdata_" + motname);
            Console.WriteLine("Motion: {0}", mot.Name);
            Console.WriteLine("Mdata: {0}", mot.MdataName);
            int nodeid = 0;
            foreach (KeyValuePair<int, AnimModelData> model in mot.Models)
            {
                string nodename = obj.GetObjectsAnimated()[model.Key].Name;
                if (model.Value.Position.Count > 0)
                {
                    model.Value.PositionName = MakeLabel(nodename, "pos_" + motname);
                    Console.WriteLine("Pos: {0}", model.Value.PositionName);
                }
                if (model.Value.Rotation.Count > 0)
                {
                    model.Value.RotationName = MakeLabel(nodename, "rot_" + motname);
                    Console.WriteLine("Rot: {0}", model.Value.RotationName);
                }
                if (model.Value.Scale.Count > 0)
                {
                    model.Value.ScaleName = MakeLabel(nodename, "scl_" + motname);
                    Console.WriteLine("Scl: {0}", model.Value.ScaleName);
                }
                if (model.Value.Vector.Count > 0)
                {
                    model.Value.VectorName = MakeLabel(nodename, "vct_" + motname);
                    Console.WriteLine("Vect: {0}", model.Value.VectorName);
                }
                /*
                if (model.Value.Vertex.Count > 0)
                {
                    model.Value.VertexName = MakeLabel(nodename, "vlist_" + motname);
                    for (int frame = 0; frame < model.Value.Vertex.Count; frame++)
                    {
                        int[] keyframes = model.Value.Vertex.Keys.ToArray();
                        model.Value.VertexItemName[frame] = MakeLabel(nodename, "vert" + keyframes[frame].ToString() + "_" + motname);
                    }
                }
                if (model.Value.Normal.Count > 0)
                {
                    model.Value.ScaleName = MakeLabel(nodename, "vert" + model.Key.ToString() + "_" + motname);
                }
                */
                Console.WriteLine();
            }

        }

            static void LabelGen_Main(string[] args)
        {
            NJS_OBJECT oldobj = new ModelFile(args[0]).Model;
            NJS_MOTION newmot=NJS_MOTION.Load(args[1]);
            GenerateLabels(newmot, "run", oldobj);
            newmot.Save("C:\\Users\\Pkr\\desktop\\ass.saanim");
           ModelFile.CreateFile("C:\\Users\\Pkr\\desktop\\ass.sa1mdl", oldobj, null, null, null, null, oldobj.GetModelFormat());
            //GenerateLabels(oldobj, args[1]);

        }
	}
}
