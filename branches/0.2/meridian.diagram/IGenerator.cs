namespace meridian.diagram
{
    public interface IGenerator
    {
        void Generate();
        ProtoClassFile CreateProtoClass(ProtoDescription _proto);
        ProtoStoreClassFile CreateprotoStoreClass(ProtoDescription _proto);
        LoaderClassFile CreateLoaderClass(IOperationContext _simpleOperationContext);
    }
}