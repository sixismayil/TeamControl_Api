using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.DTO.RequestModels;
using TeamControlV2.DTO.ResponseModels.Inner;

namespace TeamControlV2.Services.Interface
{
    public interface IBonusAndPrizeService
    {
        void CreateBonusAndPrize(BonusAndPrizePayload bonusAndPrize, int currentUserId, ref int errorCode, ref string message, string traceId);

        List<BONUS_AND_PRIZE_VIEW_MODEL> GetBonusesAndPrizes(BONUS_AND_PRIZE_FILTER_VIEW_MODEL model, int skip, int limit, ref decimal totalCount, bool isExport, ref int errorCode, ref string message, string traceId);

        BonusAndPrizePayload GetBonusAndPrize(int id, ref int errorCode, ref string message, string traceId);

        void UpdateBonusAndPrize(BonusAndPrizePayload project, int currentUserId, int id, ref int errorCode, ref string message, string traceId);

        void DeleteBonusAndPrize(int id, int currentUserId, ref int errorCode, ref string message, string traceId);
    }
}
