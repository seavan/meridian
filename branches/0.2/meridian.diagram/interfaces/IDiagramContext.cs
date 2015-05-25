namespace meridian.diagram
{
    public interface IDiagramContext
    {
        void DecreaseVersion();
        void IncreaseVersion();
        long GetVersion();
        IOperationContext GetOperationContext();
        IBackendOperationContext GetBackendOperationContext();
    }
}