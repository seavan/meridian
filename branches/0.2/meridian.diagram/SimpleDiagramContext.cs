using meridian.core;

namespace meridian.diagram
{
    public class SimpleDiagramContext : IDiagramContext
    {
        public SimpleDiagramContext()
        {
            m_OperationContext = new SimpleOperationContext();
        }

        public void DecreaseVersion()
        {
            throw new System.NotImplementedException();
        }

        public void IncreaseVersion()
        {
            throw new System.NotImplementedException();
        }

        public long GetVersion()
        {
            throw new System.NotImplementedException();
        }

        public virtual IOperationContext GetOperationContext()
        {
            return m_OperationContext;
        }

        public virtual IBackendOperationContext GetBackendOperationContext()
        {
            throw new System.NotImplementedException();
        }

        private IOperationContext m_OperationContext;

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
    }
}