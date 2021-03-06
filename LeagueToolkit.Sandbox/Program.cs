﻿using LeagueToolkit.Converters;
using LeagueToolkit.Helpers.Structures.BucketGrid;
using LeagueToolkit.IO.AnimationFile;
using LeagueToolkit.IO.PropertyBin;
using LeagueToolkit.IO.MapGeometry;
using LeagueToolkit.IO.NavigationGridOverlay;
using LeagueToolkit.IO.NVR;
using LeagueToolkit.IO.OBJ;
using LeagueToolkit.IO.ReleaseManifestFile;
using LeagueToolkit.IO.SimpleSkinFile;
using LeagueToolkit.IO.SkeletonFile;
using LeagueToolkit.IO.StaticObjectFile;
using LeagueToolkit.IO.WadFile;
using LeagueToolkit.IO.WGT;
using LeagueToolkit.IO.WorldGeometry;
using ImageMagick;
using Newtonsoft.Json;
using SharpGLTF.Schema2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LeagueAnimation = LeagueToolkit.IO.AnimationFile.Animation;
using LeagueToolkit.Meta;
using LeagueToolkit.Meta.Attributes;
using System.Numerics;
using LeagueToolkit.Meta.Dump;
using System.Reflection;

namespace LeagueToolkit.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            BinTree nn = new BinTree(@"C:\Users\Crauzer\Downloads\D373034A82E109D1.bin");


            BinTree binTree = new BinTree(@"C:\Users\Crauzer\Desktop\New folder\data\characters\aatrox\skins\skin0.bin");
            MetaEnvironment environment = MetaEnvironment.Create(
                Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.IsClass && x.Namespace == "LeagueToolkit.Meta.Classes")
                .ToList());

            MetaDump dump = MetaDump.Deserialize(File.ReadAllText(@"C:\Users\Crauzer\Downloads\meta_10.21.339.2173.json"));

            List<string> propertyNames = new List<string>();
            List<string> classNames = new List<string>();

            foreach(string line in File.ReadAllLines(@"C:\Users\Crauzer\Downloads\hashes.binfields.txt"))
            {
                propertyNames.Add(line.Split(' ')[1]);
            }
            foreach (string line in File.ReadAllLines(@"C:\Users\Crauzer\Downloads\hashes.bintypes.txt"))
            {
                classNames.Add(line.Split(' ')[1]);
            }

            dump.WriteMetaClasses(@"C:\Users\Crauzer\Downloads\meta_10.21.339.2173.cs", classNames, propertyNames);

            //var scdp = MetaSerializer.Deserialize<SkinCharacterDataProperties>(environment, binTree.Objects[0]);
        }

        static void TestMapgeo()
        {
            MapGeometry mgeo = new MapGeometry(@"C:/Users/Crauzer/Desktop/data/maps/mapgeometry/sr/base_srx.mapgeo");

            string randomMaterialName = mgeo.Models[180].Submeshes[0].Material;

            mgeo.Models.Clear();

            OBJFile object1 = new OBJFile("room155.obj");
            OBJFile object2 = new OBJFile("room156.obj");
            OBJFile object3 = new OBJFile("room157.obj");

            AddOBJ(object1, "MapGeo_Instance_0");
            AddOBJ(object2, "MapGeo_Instance_1");
            AddOBJ(object3, "MapGeo_Instance_2");

            mgeo.Write("base_srx.mapgeo.edited", 7);

            void AddOBJ(OBJFile obj, string name)
            {
                //We will add each object 2 times just for fun to see how transformation works

                (List<ushort> indices, List<MapGeometryVertex> vertices) = obj.GetMGEOData();

                Matrix4x4 transformation = Matrix4x4.CreateTranslation(new Vector3(0, 50, 100));

                MapGeometrySubmesh submesh = new MapGeometrySubmesh("", 0, (uint)indices.Count, 0, (uint)vertices.Count);
                MapGeometryModel model1 = new MapGeometryModel(name, vertices, indices, new List<MapGeometrySubmesh>() { submesh }, MapGeometryLayer.AllLayers);

                mgeo.AddModel(model1);
            }
        }

        static void TestWGEO()
        {
            WorldGeometry wgeo = new WorldGeometry("room.wgeo");
            Directory.CreateDirectory("kek");

            for (int i = 0; i < 128; i++)
            {
                for (int j = 0; j < 128; j++)
                {
                    BucketGridBucket bucket = wgeo.BucketGrid.Buckets[i, j];

                    List<uint> indices = wgeo.BucketGrid.Indices
                        .GetRange((int)bucket.StartIndex, (bucket.InsideFaceCount + bucket.StickingOutFaceCount) * 3)
                        .Select(x => (uint)x)
                        .ToList();

                    if (indices.Count != 0)
                    {
                        int startVertex = (int)indices.Min();
                        int vertexCount = (int)indices.Max() - startVertex;
                        List<Vector3> vertices = wgeo.BucketGrid.Vertices.GetRange(startVertex + (int)bucket.BaseVertex, vertexCount);

                        new OBJFile(vertices, indices).Write(string.Format("kek/bucket{0}_{1}.obj", i, j));
                    }
                }
            }
        }

        static void TestStaticObject()
        {
            StaticObject sco = StaticObject.ReadSCB("aatrox_base_w_ground_ring.scb");
            sco.WriteSCO(@"C:\Users\Crauzer\Desktop\zzzz.sco");

            StaticObject x = StaticObject.ReadSCB(@"C:\Users\Crauzer\Desktop\zzzz.scb");

        }
    }
}
