using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace NATSInternal.Services;

public class VietnamTimeInterceptor : DbCommandInterceptor
{
    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        command.CommandText = string.Format("SET time_zone = '+07:00'; {0}", command.CommandText);
        return result;
    }
}