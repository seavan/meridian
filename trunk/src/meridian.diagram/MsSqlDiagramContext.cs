namespace meridian.diagram
{
    public class MsSqlDiagramContext : SimpleDiagramContext
    {
        public MsSqlDiagramContext(string _connectionString, bool _loadSchema = true, bool _loadStoredProcs = false)
        {
            m_BackendOperationContext = new MsSqlBackendOperationContext(_connectionString, _loadSchema, _loadStoredProcs);
        }


        public override IBackendOperationContext GetBackendOperationContext()
        {
            return m_BackendOperationContext;
        }

        private IBackendOperationContext m_BackendOperationContext;
    }
}