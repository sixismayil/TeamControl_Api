using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamControlV2.Domain.Models;

namespace TeamControlV2.DTO.HelperModels
{
    public class ResponseTotal<T>
    {
        public List<T> Data { get; set; }

        public decimal Total { get; set; }

        public static explicit operator ResponseTotal<T>(DbSet<CUSTOMER> v)
        {
            throw new NotImplementedException();
        }

    }
}
