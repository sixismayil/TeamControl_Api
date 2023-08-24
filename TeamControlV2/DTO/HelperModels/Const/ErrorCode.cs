using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamControlV2.DTO.HelperModels.Const
{
    public class ErrorCode
    {
        public const int AUTH = 11;

        // 400
        public const int LOOKUP = 21;
        public const int REQUIRED = 22;
        public const int FORMAT = 23;

        // 200
        public const int IAMAS_NOT_FOUND = 41;
        public const int IAMAS_DOC_PASSIVE = 42;
        public const int AVIS_NOT_FOUND = 43;
        public const int CUSTOMER = 44;
        public const int RELATED_PEOPLE = 45;
        public const int OPERATION = 46;
        public const int DB_NOT_FOUND = 47;

        // 500
        public const int IAMAS_SERVER = 61;
        public const int AVIS_SERVER = 62;
        public const int SYSTEM = 63;
        public const int DB = 64;
        public const int MODEL_STATE = 65;
        public const int BULK = 66;
    }
}
