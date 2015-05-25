using System.Linq;
using meridian.core;

namespace meridian.diagram
{
    public class MySqlDiagramContext : SimpleDiagramContext
    {
        public MySqlDiagramContext(string _connectionString)
        {
            m_BackendOperationContext = new MySqlBackendOperationContext(_connectionString);
        }

        public override IBackendOperationContext GetBackendOperationContext()
        {
            return m_BackendOperationContext;
        }

        public void SyncAll()
        {
            GetBackendOperationContext().GetTableNames().ToList().ForEach(s => SyncTable(s));
        }

        public void SyncTable(string _tableName)
        {
            if (!GetBackendOperationContext().HasTable(_tableName))
            {
                Tracer.I.Error("Backend has no such table {0}", _tableName);
                return;
            }

            if (GetOperationContext().Exists(ElementType.Proto, _tableName))
            {
                Tracer.I.Notice("Proto already exists {0}", _tableName);
                Tracer.I.Notice("Deleting proto {0}", _tableName);
                GetOperationContext().DropProto(_tableName);
            }

            var proto = GetOperationContext().CreateProto(_tableName);

            var fields = GetBackendOperationContext().GetTableFields(_tableName);

            foreach (var field in fields)
            {
                GetOperationContext().CreateField(proto, field);
            }
        }

        private IBackendOperationContext m_BackendOperationContext;
    }
}