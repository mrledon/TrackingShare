using System;
using System.Collections.Generic;

namespace EmployeeTracking.Core
{
    public static class ExtensionClass
    {
        public static IDictionary<string, string> _imagesMappingDictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {
        {".art", "image/x-jg"},
        {".bmp", "image/bmp"},
        {".cmx", "image/x-cmx"},
        {".cod", "image/cis-cod"},
        {".dib", "image/bmp"},
        {".gif", "image/gif"},
        {".ico", "image/x-icon"},
        {".ief", "image/ief"},
        {".jfif", "image/pjpeg"},
        {".jpg", "image/jpeg"},
        {".mac", "image/x-macpaint"},
        {".pbm", "image/x-portable-bitmap"},
        {".pct", "image/pict"},
        {".pgm", "image/x-portable-graymap"},
        {".pic", "image/pict"},
        {".pict", "image/pict"},
        {".png", "image/png"},
        {".pnm", "image/x-portable-anymap"},
        {".pnt", "image/x-macpaint"},
        {".pntg", "image/x-macpaint"},
        {".pnz", "image/png"},
        {".ppm", "image/x-portable-pixmap"},
        {".qti", "image/x-quicktime"},
        {".qtif", "image/x-quicktime"},
        {".ras", "image/x-cmu-raster"},
        {".rf", "image/vnd.rn-realflash"},
        {".rgb", "image/x-rgb"},
        {".tif", "image/tiff"},
        {".tiff", "image/tiff"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".xbm", "image/x-xbitmap"},
        {".xpm", "image/x-xpixmap"},
        {".xwd", "image/x-xwindowdump"}
        };
    }
}
