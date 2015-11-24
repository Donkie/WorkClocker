using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace WorkClocker.Helpers
{
    [Serializable]
    public class WindowExe
    {
        [XmlIgnore]
        public ImageSource Icon { get; set; }
        public string Path { get; set; }
        public string Exe { get; set; }
        public string Title { get; set; }
        public bool IsAfkExe { get; set; }
        
        public static ImageSource DefaultIcon { get; private set; }

        public static void SetDefaultIcon(Icon icon)
        {
            using (var bmp = icon.ToBitmap())
            {
                var stream = new MemoryStream();
                bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                DefaultIcon = BitmapFrame.Create(stream);
            }
        }

        public void LoadIcon()
        {
            try
            {
                var icon = System.Drawing.Icon.ExtractAssociatedIcon(Path);

                if (icon == null) return;
                using (var bmp = icon.ToBitmap())
                {
                    var stream = new MemoryStream();
                    bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    Icon = BitmapFrame.Create(stream);
                }
            }
            catch (Exception)
            {
                Icon = DefaultIcon;
            }

        }
    }
}
