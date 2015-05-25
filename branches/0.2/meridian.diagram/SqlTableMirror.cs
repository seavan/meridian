using System.Collections.Generic;

namespace meridian.diagram
{
    public class SqlTableMirror
    {
        public SqlTableMirror()
        {
            Fields = new List<SqlFieldMirror>();
            IsMysql = false;
        }

        public string Name { get; set; }
        public List<SqlFieldMirror> Fields { get; set; }
        public bool IsMysql { get; set; }
    }
}