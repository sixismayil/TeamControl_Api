using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.DTO.RequestModels;
using TeamControlV2.DTO.ResponseModels.Inner;

namespace TeamControlV2.Services.Interface
{
    public interface IPositionService
    {
        void CreatePosition(PositionPayload position, ref int errorCode, ref string message, string traceId);
        List<POSITION_VIEW_MODEL> GetPositions(int skip, int limit, ref decimal totalCount, bool isExport, ref int errorCode, ref string message, string traceId);
        PositionPayload GetPosition(int id, ref int errorCode, ref string message, string traceId);
        void UpdatePosition(PositionPayload position, int id, ref int errorCode, ref string message, string traceId);
        void DeletePosition(int id, ref int errorCode, ref bool positionExists, ref string message, string traceId);
    }
}
