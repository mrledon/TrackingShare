using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Data.CommonData
{
    public static class MEDIA_TYPE
    {
        public const string DEFAULT = "DEFAULT";
        public const string SELFIE = "SELFIE";
        public const string TRANH_PEPSI_AND_7UP = "TRANH_PEPSI_AND_7UP";
        public const string STICKER_7UP = "STICKER_7UP";
        public const string STICKER_PEPSI = "STICKER_PEPSI";
        public const string BANNER_PEPSI = "BANNER_PEPSI";
        public const string BANNER_7UP_TET = "BANNER_7UP_TET";
        public const string BANNER_MIRINDA = "BANNER_MIRINDA";
        public const string BANNER_TWISTER = "BANNER_TWISTER";
        public const string BANNER_REVIVE = "BANNER_REVIVE";
        public const string BANNER_OOLONG = "BANNER_OOLONG";
        public const string STORE_FAILED = "STORE_FAILED";

        public static readonly IList<string> POSM = new ReadOnlyCollection<string>(
            new List<string> {
                TRANH_PEPSI_AND_7UP,
                STICKER_7UP,
                STICKER_PEPSI,
                BANNER_PEPSI,
                BANNER_7UP_TET,
                BANNER_MIRINDA,
                BANNER_TWISTER,
                BANNER_REVIVE,
                BANNER_OOLONG
            });
    }
}
