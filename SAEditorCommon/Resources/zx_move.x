xof 0303txt 0032
template ColorRGBA {
 <35ff44e0-6c7c-11cf-8f52-0040333594a3>
 FLOAT red;
 FLOAT green;
 FLOAT blue;
 FLOAT alpha;
}

template ColorRGB {
 <d3e16e81-7835-11cf-8f52-0040333594a3>
 FLOAT red;
 FLOAT green;
 FLOAT blue;
}

template Material {
 <3d82ab4d-62da-11cf-ab39-0020af71e433>
 ColorRGBA faceColor;
 FLOAT power;
 ColorRGB specularColor;
 ColorRGB emissiveColor;
 [...]
}

template Frame {
 <3d82ab46-62da-11cf-ab39-0020af71e433>
 [...]
}

template Matrix4x4 {
 <f6f23f45-7686-11cf-8f52-0040333594a3>
 array FLOAT matrix[16];
}

template FrameTransformMatrix {
 <f6f23f41-7686-11cf-8f52-0040333594a3>
 Matrix4x4 frameMatrix;
}

template Vector {
 <3d82ab5e-62da-11cf-ab39-0020af71e433>
 FLOAT x;
 FLOAT y;
 FLOAT z;
}

template MeshFace {
 <3d82ab5f-62da-11cf-ab39-0020af71e433>
 DWORD nFaceVertexIndices;
 array DWORD faceVertexIndices[nFaceVertexIndices];
}

template Mesh {
 <3d82ab44-62da-11cf-ab39-0020af71e433>
 DWORD nVertices;
 array Vector vertices[nVertices];
 DWORD nFaces;
 array MeshFace faces[nFaces];
 [...]
}

template MeshMaterialList {
 <f6f23f42-7686-11cf-8f52-0040333594a3>
 DWORD nMaterials;
 DWORD nFaceIndexes;
 array DWORD faceIndexes[nFaceIndexes];
 [Material <3d82ab4d-62da-11cf-ab39-0020af71e433>]
}

template VertexElement {
 <f752461c-1e23-48f6-b9f8-8350850f336f>
 DWORD Type;
 DWORD Method;
 DWORD Usage;
 DWORD UsageIndex;
}

template DeclData {
 <bf22e553-292c-4781-9fea-62bd554bdd93>
 DWORD nElements;
 array VertexElement Elements[nElements];
 DWORD nDWords;
 array DWORD data[nDWords];
}


Material doubleAxisMaterial {
 0.588235;0.588235;0.588235;1.000000;;
 9.999999;
 0.000000;0.000000;0.000000;;
 0.000000;0.000000;0.000000;;
}

Frame xz_component {
 

 FrameTransformMatrix {
  1.000000,0.000000,0.000000,0.000000,0.000000,1.000000,0.000000,0.000000,0.000000,0.000000,1.000000,0.000000,0.000000,0.000000,0.000000,1.000000;;
 }

 Mesh xz_component {
  24;
  0.027883;0.030947;0.027398;,
  0.027883;0.030947;0.483889;,
  0.484374;0.030947;0.483889;,
  0.484374;0.030947;0.027398;,
  0.027883;-0.034893;0.027398;,
  0.484374;-0.034893;0.027398;,
  0.484374;-0.034893;0.483889;,
  0.027883;-0.034893;0.483889;,
  0.027883;0.030947;0.027398;,
  0.484374;0.030947;0.027398;,
  0.484374;-0.034893;0.027398;,
  0.027883;-0.034893;0.027398;,
  0.484374;0.030947;0.027398;,
  0.484374;0.030947;0.483889;,
  0.484374;-0.034893;0.483889;,
  0.484374;-0.034893;0.027398;,
  0.484374;0.030947;0.483889;,
  0.027883;0.030947;0.483889;,
  0.027883;-0.034893;0.483889;,
  0.484374;-0.034893;0.483889;,
  0.027883;0.030947;0.483889;,
  0.027883;0.030947;0.027398;,
  0.027883;-0.034893;0.027398;,
  0.027883;-0.034893;0.483889;;
  12;
  3;0,1,2;,
  3;2,3,0;,
  3;4,5,6;,
  3;6,7,4;,
  3;8,9,10;,
  3;10,11,8;,
  3;12,13,14;,
  3;14,15,12;,
  3;16,17,18;,
  3;18,19,16;,
  3;20,21,22;,
  3;22,23,20;;

  MeshMaterialList {
   1;
   12;
   0,
   0,
   0,
   0,
   0,
   0,
   0,
   0,
   0,
   0,
   0,
   0;
   { doubleAxisMaterial }
  }

  DeclData {
   1;
   2;0;3;0;;
   72;
   0,
   1065353216,
   859553070,
   0,
   1065353216,
   859553070,
   0,
   1065353216,
   859553070,
   0,
   1065353216,
   859553070,
   0,
   3212836864,
   3007036718,
   0,
   3212836864,
   3007036718,
   0,
   3212836864,
   3007036718,
   0,
   3212836864,
   3007036718,
   0,
   859553070,
   3212836864,
   0,
   859553070,
   3212836864,
   0,
   859553070,
   3212836864,
   0,
   859553070,
   3212836864,
   1065353216,
   0,
   0,
   1065353216,
   0,
   0,
   1065353216,
   0,
   0,
   1065353216,
   0,
   0,
   0,
   3007036718,
   1065353216,
   0,
   3007036718,
   1065353216,
   0,
   3007036718,
   1065353216,
   0,
   3007036718,
   1065353216,
   3212836864,
   0,
   0,
   3212836864,
   0,
   0,
   3212836864,
   0,
   0,
   3212836864,
   0,
   0;
  }
 }
}