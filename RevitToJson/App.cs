using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitToJson
{
    public class App : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            // Create button
            string btnName = "Convert";
            string btnLabel = "Convert\r\nNow";
            string btnAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string btnCommandClassName = "RevitToJson.Click_Convert";

            var btnData = new PushButtonData(btnName, btnLabel, btnAssembly, btnCommandClassName)
            {
                LargeImage = bitmapToImageSource(Properties.Resources.unityLogoLg)
            };

            //Create Ribbon Tab
            string tabName = "App Development";
            application.CreateRibbonTab(tabName);

            //Create Panel
            string pnlName = "Unity";
            RibbonPanel panel = application.CreateRibbonPanel(tabName, pnlName);
            panel.AddItem(btnData);

            return Result.Succeeded;
        }

        private BitmapImage bitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            // Convert System.Drawing.Bitmap to a BitmapImage that Revit can use for buttons
            using (MemoryStream mem = new MemoryStream())
            {
                bitmap.Save(mem, System.Drawing.Imaging.ImageFormat.Png);
                mem.Position = 0;

                BitmapImage bitmapImg = new BitmapImage();
                bitmapImg.BeginInit();
                bitmapImg.StreamSource = mem;
                bitmapImg.CacheOption = BitmapCacheOption.OnLoad;
                //bitmapImg.CreateOptions = BitmapCreateOptions.None;
                bitmapImg.EndInit();

                return bitmapImg;
            }
        }
    }
}
