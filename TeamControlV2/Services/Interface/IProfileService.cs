using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.DTO.RequestModels;
using TeamControlV2.DTO.ResponseModels.Inner;

namespace TeamControlV2.Services.Interface
{
    public interface IProfileService
    {
        PROFILE_VIEW_MODEL GetData(int currentUserId, ref int errorCode, ref string message, string traceId);

        void UpdateData(ProfilePayload profile, int currentUserId, ref int errorCode, ref string message, string traceId);
    }
}
