namespace meridian.diagram
{
    public class SimpleExecutor : Executor
    {
        public SimpleExecutor(IDiagramContext _context) : base(_context)
        {
            Register(new CreateAggregationCommandExecutor());
            Register(new CreateFieldCommandExecutor());
            Register(new CreateProtoCommandExecutor());
            Register(new DropFieldCommandExecutor());
            Register(new DropProtoCommandExecutor());
            Register(new RecreateProtosCommandExecutor());
            Register(new CreateCompositionCommandExecutor());
            Register(new CreateInlineCompositionCommandExecutor());
        }
    }
}