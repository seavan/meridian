namespace meridian.diagram
{
    public interface IGenerator
    {
        void Generate();
        ProtoClassFile CreateProtoClass(ProtoDescription _proto);
        ProtoStoreClassFile CreateprotoStoreClass(ProtoDescription _proto);
        LoaderClassFile CreateLoaderClass(IOperationContext _simpleOperationContext);
        ControllerClassFile CreateControllerClass(Controller _proto);
        ControllerImplClassFile CreateControllerImplClass(Controller _proto);
        MetaClassFile CreateMetaFile(ProtoDescription _proto);
        AspxGridFile CreateGrid(ProtoDescription _proto);
        AspxIndexFile CreateIndex(ProtoDescription _proto);
        AspxSingleFile CreateSingle(ProtoDescription _proto);
    }
}