using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitToJson
{
    public class Model
    {
        public string Name { get; set; }
        public List<ModelElement> Parts { get; } = new List<ModelElement>();

        public class ModelElement
        {
            public string Name { get; set; }
            public string Id { get; set; }
            public string Activity { get; set; }
            public string Level { get; set; }
            public string Sub1 { get; set; }
            public string Sub2 { get; set; }
            public string Sub3 { get; set; }
            public List<(string, string)> Parameters { get; set; }

            public List<PartSolid> Solids { get; } = new List<PartSolid>();

            public class PartSolid
            {
                public List<SolidFace> Faces { get; } = new List<SolidFace>();

                public class SolidFace
                {
                    public List<double[]> Vertices { get; } = new List<double[]>();
                    public List<int[]> Triangles { get; } = new List<int[]>();
                }
            }
        }
    }
}
