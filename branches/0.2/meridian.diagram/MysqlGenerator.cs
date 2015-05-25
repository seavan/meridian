using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace meridian.diagram
{
    public class MysqlGenerator : BasicGenerator
    {
        public MysqlGenerator(IOperationContext _context, IBackendOperationContext _backend)
            : base(_context, _backend)
        {
        }

        public override string GetAdoNetNamespace()
        {
            return "MySql.Data.MySqlClient";
        }

        public override string GetAdoNetPrefix()
        {
            return "MySql";
        }

        public override string GetLeftFieldSeparator()
        {
            return "`";
        }

        public override string GetRightFieldSeparator()
        {
            return "`";
        }
    }
}
