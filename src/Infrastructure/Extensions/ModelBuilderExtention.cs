using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uni.Scan.Infrastructure.Extensions
{
    public  static class ModelBuilderExtention
    {
        public static void SetStringMaxLenght<T>(this ModelBuilder builder,int value)
        {
            foreach (var property in builder.Model.GetEntityTypes()
                         .AsEnumerable().Where(e => e.ClrType == typeof(T)).SelectMany(t => t.GetProperties())
                         .Where(p => p.ClrType == typeof(string)))
            {
                if (property.GetMaxLength() == null)
                {
                    property.SetMaxLength(value);
                }
            }
        }
    }
}
