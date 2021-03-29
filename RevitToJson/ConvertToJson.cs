using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;

namespace RevitToJson
{
    public static class ConvertToJson
    {
        public static Result Convert(Document doc)
        {
            ICollection<BuiltInCategory> categoriesToExport = new List<BuiltInCategory>()
            {
                BuiltInCategory.OST_Floors,
                BuiltInCategory.OST_StructuralColumns,
                BuiltInCategory.OST_StructuralFoundation,
                BuiltInCategory.OST_StructuralFraming,
                BuiltInCategory.OST_Walls,
                BuiltInCategory.OST_Casework
            };
            ElementMulticategoryFilter filter = new ElementMulticategoryFilter(categoriesToExport);
            FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(filter).WhereElementIsNotElementType();

            Model model = new Model()
            {
                Name = doc.PathName
            };
            // Add ModelElements to Model
            foreach (Element element in collector)
            {
                var part = new Model.ModelElement
                {
                    Name = element.Name,
                    Id = element.Id.ToString(),
                    Level = getConditioning(element, "Activity Level"),
                    Activity = getConditioning(element, "Activity"),
                    Sub1 = getConditioning(element, "Sub-Activity 1"),
                    Sub2 = getConditioning(element, "Sub-Activity 2"),
                    Sub3 = getConditioning(element, "Sub-Activity 3"),
                    Parameters = getQuantityParams(element)
                };
                var solids = GetSolids(element);

                // Add Solids to ModelElement
                foreach (Solid revitSolid in solids)
                {
                    var solid = new Model.ModelElement.PartSolid();

                    // Add Faces to Solid
                    foreach (Face revitFace in revitSolid.Faces)
                    {
                        Mesh mesh = revitFace.Triangulate(1);
                        var face = new Model.ModelElement.PartSolid.SolidFace();

                        // Add Vertices Array to Face
                        foreach (XYZ revitVertex in mesh.Vertices)
                        {
                            double[] vertex = new double[3] { revitVertex.X, revitVertex.Y, revitVertex.Z };
                            face.Vertices.Add(vertex);
                        }

                        // Add Triangle Array to Face
                        for (int i = 0; i < mesh.NumTriangles; i++)
                        {
                            MeshTriangle revitTriangle = mesh.get_Triangle(i);

                            int[] triangle = new int[3];
                            triangle[0] = (int)revitTriangle.get_Index(0);
                            triangle[1] = (int)revitTriangle.get_Index(1);
                            triangle[2] = (int)revitTriangle.get_Index(2);
                            face.Triangles.Add(triangle);
                        }
                        solid.Faces.Add(face);
                    }
                    part.Solids.Add(solid);
                }
                model.Parts.Add(part);
            }

            string output = JsonConvert.SerializeObject(model);
            File.WriteAllText(@"C:\Users\leand\Desktop\test.json", output);

            return Result.Succeeded;
        }


        public static List<Solid> GetSolids(Element element)
        {
            List<Solid> output = new List<Solid>();

            try
            {
                Options geoOptions = new Options { IncludeNonVisibleObjects = false };
                GeometryElement geoElem = element.get_Geometry(geoOptions);

                // Try to retrive a Solid from the current GeometryObject through a simple cast, or by digging into the GeometryInstance if the cast returns null.
                foreach (GeometryObject geoObject in geoElem)
                {
                    Solid geoSolid = geoObject as Solid;
                    if (geoSolid != null && geoSolid.Faces.Size > 0) { output.Add(geoSolid); }
                    else
                    {
                        GeometryInstance geoInst = geoObject as GeometryInstance;
                        if (geoInst == null) { continue; }
                        GeometryElement geoElemTmp = geoInst.GetInstanceGeometry();
                        foreach (GeometryObject geoObjectTmp in geoElemTmp)
                        {
                            geoSolid = geoObjectTmp as Solid;
                            if (geoSolid != null && geoSolid.Faces.Size > 0) { output.Add(geoSolid); }
                        }
                    }
                }
                return output;
            }
            catch { Debug.WriteLine($"Error retrieving Solid for element - {element.Name}"); return output; }
        }
        private static List<(string, string)> getQuantityParams(Element element)
        {
            string[] customParams = new string[] { "0D", "1D", "2D", "3D" };
            var output = new List<(string, string)>();

            foreach (Parameter param in element.Parameters)
            {
                string test = param.Definition.Name.Split('-')[0].Trim();
                if (customParams.Contains<string>(test))
                {
                    output.Add((param.Definition.Name, param.AsValueString()));
                }
            }
            return output;
        }
        private static string getConditioning(Element element, string parameter)
        {
            if (element.LookupParameter(parameter) is Parameter P) { return P.AsString(); }
            else { return ""; }
        }
    }
}
