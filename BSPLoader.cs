using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LibBSP;
using BSPImporter;

namespace Dataram57.Tools
{
    public class BSPLoader : EditorWindow
    {
        static BSPLoader window;
        static string path = @"C:\rintorfer\maps";
        static string mapName = @"e1m1";
        static string meshOutputLocation = @"Assets/mesh/";
        static string materialsLocation = @"Assets/Materials";
        BSP bsp;

        /*
        * 
        * 
        * 
        * 
        * 
        * 
        * 
        * Links:
        * https://github.com/wfowler1/Unity3D-BSP-Importer
        * https://github.com/wfowler1/LibBSP
        */

        [MenuItem("Dataram_57/Tools/BSP Loader")]
        public static void Open()
        {
            window = (BSPLoader)GetWindow(typeof(BSPLoader));
        }

        private void OnGUI()
        {
            GUILayout.Label(@"Maps folder path(C:\BSPWorkspace\maps\):");
            path = GUILayout.TextField(path);
            GUILayout.Label("Map name(e1m1):");
            mapName = GUILayout.TextField(mapName);
            GUILayout.Label("Mesh uutput folder location(Assets/...):");
            meshOutputLocation = GUILayout.TextField(meshOutputLocation);
            GUILayout.Label("Material folder Location(Assets/...):");
            materialsLocation = GUILayout.TextField(materialsLocation);

            if (GUILayout.Button("Load"))
            {
                CorrectPaths();
                LoadBsp();
            }
        }

        private void CorrectPaths()
        {
            path = CorrectDir(path);
            materialsLocation = CorrectDir(materialsLocation);
            meshOutputLocation = CorrectDir(meshOutputLocation);
        }

        public void LoadBsp()
        {
            //setup
            bsp = new BSP(path + mapName + ".bsp");

            //main map model
            Model map = bsp.models[0];

            //faces (each face has texture)
            List<Face> faces = bsp.GetFacesInModel(map);

            // Texture ; Mesh
            Dictionary<string, List<Mesh>> textureMeshMap = new Dictionary<string, List<Mesh>>();

            //later i will combine it
            int i;
            int f;

            Debug.Log("Faces.Count=" + faces.Count);

            //assign faces(represented by Mesh) to the texture group
            for (i = faces.Count - 1; i >= 0; i--)
            {
                Face face = faces[i];

                if (face.NumEdgeIndices <= 0 && face.NumVertices <= 0)
                    continue;

                int textureIndex = bsp.GetTextureIndex(face);       //gets index face (useful for me)

                if (textureIndex >= 0)
                {
                    LibBSP.Texture texture = bsp.textures[textureIndex];
                    string textureName = LibBSP.Texture.SanitizeName(texture.Name, bsp.version);

                    if (!textureMeshMap.ContainsKey(textureName) || textureMeshMap[textureName] == null)
                    {
                        textureMeshMap[textureName] = new List<Mesh>();
                    }

                    //returns mesh
                    //later it registers it into textureMeshMap
                    textureMeshMap[textureName].Add(CreateFaceMesh(face));

                }
            }

            if (bsp.lodTerrains != null)
            {
                foreach (LODTerrain lodTerrain in bsp.lodTerrains)
                {
                    if (lodTerrain.TextureIndex >= 0)
                    {
                        LibBSP.Texture texture = bsp.textures[lodTerrain.TextureIndex];
                        string textureName = texture.Name;

                        if (!textureMeshMap.ContainsKey(textureName) || textureMeshMap[textureName] == null)
                        {
                            textureMeshMap[textureName] = new List<Mesh>();
                        }

                        textureMeshMap[textureName].Add(CreateLoDTerrainMesh(lodTerrain));
                    }
                }
            }

            Mesh[] textureMeshes = new Mesh[textureMeshMap.Count];
            Material[] materials = new Material[textureMeshes.Length];
            i = 0;
            f = 0;

            CombineInstance[] mapCombine = new CombineInstance[textureMeshMap.Count];

            foreach (KeyValuePair<string, List<Mesh>> pair in textureMeshMap)
            {
                Mesh submesh = new Mesh();
                f = 0;
                CombineInstance[] combine = new CombineInstance[pair.Value.Count];
                foreach (Mesh mesh in pair.Value)
                {
                    if (mesh.normals.Length == 0 || mesh.normals[0] == Vector3.zero)
                    {
                        mesh.RecalculateNormals();
                    }
                    combine[f].mesh = mesh;
                    combine[f].transform = Matrix4x4.identity;
                    //mesh.AddMeshToGameObject(new Material[] { }, new GameObject("Map " + mapName));
                    f++;
                }

                submesh.CombineMeshes(combine, true);
                mapCombine[i].mesh = submesh;
                mapCombine[i].transform = Matrix4x4.identity;

                Material mat = (Material)AssetDatabase.LoadAssetAtPath(materialsLocation + pair.Key + ".mat", typeof(Material));
                if (mat != null)
                    materials[i] = mat;
                else
                    Debug.LogWarning("No material of path ___ has been found: " + materialsLocation + pair.Key + ".mat");

                i++;
            }

            Mesh mapMesh = new Mesh();
            mapMesh.CombineMeshes(mapCombine, false);

            //optimize
            mapMesh.Optimize();
            //I will try... (i tried and i can tell that i will have to consider uv mapping optimalisation)

            mapMesh.AddMeshToGameObject(materials, new GameObject("Map " + mapName));
            AssetDatabase.CreateAsset(mapMesh, meshOutputLocation + mapName + ".mesh");


        }


        protected string CorrectDir(string dir)
        {
            dir = dir.Trim();
            if (dir.IndexOf("/") != -1)     //dir is Asset/...
            {
                if (dir.LastIndexOf("/") != dir.Length - 1) dir += "/";
            }
            else
                if (dir.LastIndexOf("\\") != dir.Length - 1) dir += "\\";
            return dir;
        }

        protected Mesh CreateFaceMesh(Face face)
        {
            //dims are made for uvs complete it cause it is quite dangerous for the performce.
            Vector2 dims;
            dims = new Vector2(128, 128);

            //mesh generation
            Mesh mesh;
            if (face.DisplacementIndex >= 0)
            {
                mesh = MeshUtils.CreateDisplacementMesh(bsp, face, dims);
            }
            else
            {
                mesh = MeshUtils.CreateFaceMesh(bsp, face, dims, 0);
            }

            return mesh;
        }

        protected Mesh CreateLoDTerrainMesh(LODTerrain lodTerrain)
        {
            return MeshUtils.CreateMoHAATerrainMesh(bsp, lodTerrain);
        }
    }
}