using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.Domain.Models;
using TeamControlV2.DTO.HelperModels.Const;
using TeamControlV2.DTO.RequestModels;
using TeamControlV2.DTO.ResponseModels.Inner;
using TeamControlV2.Infrastructure.Repository;
using TeamControlV2.Logging;
using TeamControlV2.Services.Interface;

namespace TeamControlV2.Services.Implementation
{
    public class PositionService : IPositionService
    {
        AppConfiguration config = new AppConfiguration();
        private readonly IRepository<POSITION> _positions;
        private readonly IRepository<EMPLOYEE_TO_POSITION> _employees_to_positions;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly ISqlService _sqlService;
        public PositionService(
            IRepository<POSITION> positions,
            IRepository<EMPLOYEE_TO_POSITION> employees_to_positions,
            IMapper mapper,
            ILoggerManager logger,
            ISqlService sqlService
            )
        {
            _positions = positions;
            _employees_to_positions = employees_to_positions;
            _logger = logger;
            _mapper = mapper;
            _sqlService = sqlService;
        }

        public void CreatePosition(PositionPayload position, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                POSITION pos = _mapper.Map<POSITION>(position);
                pos.IsActive = true;
                _positions.Insert(pos);
                _positions.Save();
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB create position error";
                _logger.LogError($"PositionService CreatePosition : {traceId}" + $"{ex}");
            }
        }

        public List<POSITION_VIEW_MODEL> GetPositions(int skip, int limit, ref decimal totalCount, bool isExport, ref int errorCode, ref string message, string traceId)
        {
            List<POSITION_VIEW_MODEL> response = new List<POSITION_VIEW_MODEL>();

            try
            {
                using(SqlConnection con = new SqlConnection(config.ConnectionString))
                {
                    SqlCommand cmd;
                    using (cmd = con.CreateCommand())
                    {
                        con.Open();

                        cmd.CommandText = _sqlService.Positions(false, isExport, limit, skip);
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            POSITION_VIEW_MODEL position_view_model = new POSITION_VIEW_MODEL()
                            {
                                Id = (int)rdr["Id"],
                                Name = rdr["Name"].ToString(),
                                Key = rdr["Key"].ToString()
                            };
                            response.Add(position_view_model);
                        }
                        rdr.Close();
                        cmd = con.CreateCommand();
                        if (!isExport)
                        {
                            cmd.CommandText = _sqlService.Positions(true, false, limit, skip);

                            rdr = cmd.ExecuteReader();
                            
                            while (rdr.Read())
                            {
                                totalCount = (int)rdr["totalCount"];
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB get positions error";
                _logger.LogError($"PositionService GetPositions : {traceId}" + $"{ex}");
            }
            return response;
        }

        public PositionPayload GetPosition(int id, ref int errorCode, ref string message, string traceId)
        {
            PositionPayload result = new PositionPayload();
            try
            {
                POSITION position = _positions.AllQuery.FirstOrDefault(x => x.Id == id);
                result = _mapper.Map<PositionPayload>(position);
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB get position error";
                _logger.LogError($"PositionService GetPosition : {traceId}" + $"{ex}");
            }
            return result;
        }

        public void UpdatePosition(PositionPayload position, int id, ref int errorCode, ref string message, string traceId)
        {
            try
            {
                POSITION oldData = _positions.AllQuery.AsNoTracking().Where(x => x.IsActive == true).FirstOrDefault(x => x.Id == id);
                POSITION newData = _mapper.Map<POSITION>(position);
                newData.Id = id;
                newData.IsActive = true;
                oldData = newData;
                _positions.Update(oldData);
                _positions.Save();
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = ex.Message;
                _logger.LogError($"PositionService UpdatePosition : {traceId}" + $"{ex}");
            }
        }

        public void DeletePosition(int id, ref int errorCode, ref bool positionExists, ref string message, string traceId)
        {
            try
            {
                POSITION position = _positions.AllQuery.Where(x => x.IsActive == true).FirstOrDefault(x => x.Id == id);
                EMPLOYEE_TO_POSITION emp_to_pos = _employees_to_positions.AllQuery.FirstOrDefault(x=> x.PositionId == position.Id);
                if(emp_to_pos == null)
                {
                    position.IsActive = false;
                    _positions.Update(position);
                    _positions.Save();
                }
                else
                {
                    errorCode = ErrorCode.OPERATION;
                    positionExists = true;
                    message = "Bu vəzifə hal-hazırda istifadədə olduğu üçün silinə bilməz.";
                }
            }
            catch (Exception ex)
            {
                errorCode = ErrorCode.DB;
                message = "DB delete position error";
                _logger.LogError($"PositionService DeletePosition : {traceId}" + $"{ex}");
            }
        }
    }
}
