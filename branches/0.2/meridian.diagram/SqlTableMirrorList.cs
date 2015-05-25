using System.Collections.Generic;
using System.Linq;

namespace meridian.diagram
{
    public class SqlTableMirrorList : List<SqlTableMirror>
    {
        public SqlTableMirror New()
        {
            var r = new SqlTableMirror();
            Add(r);
            return r;
        }

        public SqlTableMirror Get(string _name)
        {
            return this.Single(s => s.Name.Equals(_name));
        }
    }
}