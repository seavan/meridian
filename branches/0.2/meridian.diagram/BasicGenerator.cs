using System.IO;
using System.Linq;
using meridian.core;
using System;

namespace meridian.diagram
{
    public class BasicGenerator : IGenerator
    {
        public BasicGenerator(IOperationContext _context, IBackendOperationContext _backend)
        {
            m_Context = _context;
            m_BackendContext = _backend;
            ProtoClassPath = "proto";
            SystemClassPath = "system";
            ProtoStoreClassPath = "protoStore";
        }

        private IOperationContext m_Context;
        private IBackendOperationContext m_BackendContext;

        public string RootPath { get; set; }
        public string ProtoClassPath { get; set; }
        public string SystemClassPath { get; set; }
        public string ProtoStoreClassPath { get; set; }
        public string Namespace { get; set; }
        public string ProjectName { get; set; }

        public void Generate()
        {
            m_Context.Generate(this);
        }

        public virtual string GetAdoNetNamespace()
        {
            return "System.Data.SqlClient";
        }

        public virtual string GetAdoNetPrefix()
        {
            return "Sql";
        }

        public virtual string GetLeftFieldSeparator()
        {
            return "[";
        }

        public virtual string GetRightFieldSeparator()
        {
            return "]";
        }

        public void InitClassFile(ClassFile _file)
        {
            _file.Usings.Add(GetAdoNetNamespace());
            _file.FieldSeparatorL = GetLeftFieldSeparator();
            _file.FieldSeparatorR = GetRightFieldSeparator();
            _file.AdoNetPrefix = GetAdoNetPrefix();
        }

        public ProtoStoreClassFile CreateprotoStoreClass(ProtoDescription _proto)
        {
            var protoStoreClassFile = new ProtoStoreClassFile(_proto);
            InitClassFile(protoStoreClassFile);
            protoStoreClassFile.Usings.Add(Namespace);

            protoStoreClassFile.Usings.Add(Namespace + ".system");

            protoStoreClassFile.FilePath = Path.Combine(GetProjectPath(), ProtoStoreClassPath, _proto.Name + "Store.cs");
            protoStoreClassFile.Namespace = Namespace + ".protoStore";
            
            Tracer.I.Notice("{1} file {0}", protoStoreClassFile.FilePath, _proto.Name);
            return protoStoreClassFile;
        }

        public LoaderClassFile CreateLoaderClass(IOperationContext _simpleOperationContext)
        {
            var loaderClassFile = new LoaderClassFile(_simpleOperationContext, m_BackendContext);
            InitClassFile(loaderClassFile);
            loaderClassFile.Usings.Add(Namespace);

            loaderClassFile.FilePath = Path.Combine(GetProjectPath(), SystemClassPath, "Meridian.cs");
            loaderClassFile.Namespace = Namespace + ".system";
            Tracer.I.Notice("file {0}", loaderClassFile.FilePath);
            return loaderClassFile;
        }

        public ProtoClassFile CreateProtoClass(ProtoDescription _proto)
        {
            var protoClassFile = new ProtoClassFile(_proto);
            InitClassFile(protoClassFile);
            // todo
            _proto.Aggregations = m_Context.GetAggregationsFor(_proto).ToList();
            _proto.ParentAggregations = m_Context.GetParentAggregationsFor(_proto).ToList();
            _proto.Compositions = m_Context.GetCompositionsFor(_proto).ToList();
            _proto.InlineCompositions = m_Context.GetInlineCompositionsFor(_proto).ToList();

            protoClassFile.Usings.Add(Namespace);
            protoClassFile.Usings.Add(Namespace + ".system");
            
            protoClassFile.FilePath = Path.Combine(GetProjectPath(), ProtoClassPath, _proto.Name + ".cs");
            protoClassFile.Namespace = Namespace + ".proto";
            Tracer.I.Notice("{1} file {0}", protoClassFile.FilePath, _proto.Name);

            return protoClassFile;
        }

        protected virtual string GetProjectPath()
        {
            if (!string.IsNullOrEmpty(RootPath))
                return RootPath;

            if (string.IsNullOrEmpty(ProjectName))
                throw new InvalidOperationException("Neither RootPath nor ProjectName properties were set");

            RootPath = PathUtils.DetectProjectPath(ProjectName);

            return RootPath;
        }
    }
}