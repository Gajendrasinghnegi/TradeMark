using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMR_AgentRegistration.Infrastructure.Common
{
    public interface IOracleConnectionFactory
    {
        OracleConnection CreateConnection();
    }
}
