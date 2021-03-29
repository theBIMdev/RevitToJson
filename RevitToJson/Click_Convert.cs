using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitToJson
{
    [Transaction(TransactionMode.ReadOnly)]
    class Click_Convert : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            Result result = ConvertToJson.Convert(doc);
            //using (Transaction transaction = new Transaction(doc, "Exporting Json"))
            //{
            //    result = ConvertToJson.Convert(doc);
            //};

            return result;
        }
    }
}
