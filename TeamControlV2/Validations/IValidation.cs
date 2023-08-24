using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamControlV2.Validations
{
    public interface IValidation
    {
        int CheckErrorCode(int error);
    }
}
