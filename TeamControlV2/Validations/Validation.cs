using TeamControlV2.DTO.HelperModels.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamControlV2.Validations
{
    public class Validation: IValidation
    {
        public int CheckErrorCode(int error)
        {
            switch (error)
            {
                case ErrorCode.AUTH:
                    return StatusCode.AUTH;

                case ErrorCode.LOOKUP:
                case ErrorCode.REQUIRED:
                case ErrorCode.FORMAT:
                    return StatusCode.BAD_REQUEST;

                case ErrorCode.AVIS_NOT_FOUND:
                case ErrorCode.IAMAS_NOT_FOUND:
                case ErrorCode.IAMAS_DOC_PASSIVE:
                case ErrorCode.CUSTOMER:
                case ErrorCode.RELATED_PEOPLE:
                case ErrorCode.OPERATION:
                    return StatusCode.OK;

                case ErrorCode.IAMAS_SERVER:
                case ErrorCode.AVIS_SERVER:
                case ErrorCode.SYSTEM:
                case ErrorCode.DB:
                case ErrorCode.MODEL_STATE:
                case ErrorCode.BULK:
                    return StatusCode.INTERNEL_SERVER;
            }
            return StatusCode.OK;
        }
    }
}
