using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace meridian.diagram
{
    public class LoaderClassFile : ClassFile
    {
        public LoaderClassFile(IOperationContext _context, IBackendOperationContext _backendContext)
        {
            ClassName = "Meridian";
            m_OperationContext = _context;
            m_BackendOperationContext = _backendContext;
            Usings.Add("meridian.core");
        }

        private IOperationContext m_OperationContext;
        private IBackendOperationContext m_BackendOperationContext;

        public override void Constructor(CSharpWriter _writer)
        {
            base.Constructor(_writer);
        }

        public void TriggerGenerator(CSharpWriter _writer)
        {


            using(_writer.Block("public void PassUpdate({0}Connection _conn, string _protoName, int _action, long _protoId)",
                AdoNetPrefix))
            {
                using (_writer.Block("switch (_protoName)"))
                {

                    foreach (var proto in m_OperationContext.GetAllProtos())
                    {
                        if (proto.Fields.Count(s => s.Name.Equals("id")) == 0 && String.IsNullOrEmpty(proto.CustomKeyName)) continue;


                        _writer.WriteIndentBlank("case \"{0}\":", proto.Name);
                        using (_writer.Block("switch (_action)"))
                        {
                            _writer.WriteIndentBlank("case 0: {0}Store.InsertSync(_conn, _protoId, this); break;",
                                                     proto.Name);
                            _writer.WriteIndentBlank("case 1: {0}Store.UpdateSync(_conn, _protoId, this); break;",
                                                     proto.Name);
                            _writer.WriteIndentBlank("case 2: {0}Store.DeleteSync(_conn, _protoId, this); break;",
                                                     proto.Name);
                        }
                        _writer.WriteIndentEol("break");

                    }
                }
            }

            _writer.WriteIndentBlank("/*");

            foreach (var proto in m_OperationContext.GetAllProtos())
            {
                if (proto.Fields.Count(s => s.Name.Equals("id")) == 0 && String.IsNullOrEmpty(proto.CustomKeyName)) continue;

                var keyName = String.IsNullOrEmpty(proto.CustomKeyName)
                                  ? proto.Fields.First(s => s.Name.Equals("id")).Name
                                  : proto.CustomKeyName;

                var triggerName = "";
                triggerName = proto.Backend + "_insert";
                _writer.WriteIndentBlank("                GO");
                _writer.WriteIndentBlank("                IF OBJECT_ID ('{0}', 'TR') IS NOT NULL", triggerName);
                _writer.WriteIndentBlank("                   DROP TRIGGER dbo.{0};", triggerName);
                _writer.WriteIndentBlank("                GO");
                _writer.WriteIndentBlank("                CREATE TRIGGER [dbo].[{0}]", triggerName);
                _writer.WriteIndentBlank("                   ON  [dbo].[{0}]", proto.Backend);
                _writer.WriteIndentBlank("                   AFTER INSERT");
                _writer.WriteIndentBlank("                AS ");
                _writer.WriteIndentBlank("                BEGIN");
                _writer.WriteIndentBlank("                    SET NOCOUNT ON;");
                _writer.WriteIndentBlank("                    DECLARE InsertCursor CURSOR FOR SELECT {0} FROM Inserted", keyName);
                _writer.WriteIndentBlank("                    DECLARE @curId bigint");
                _writer.WriteIndentBlank("                    OPEN InsertCursor");
                _writer.WriteIndentBlank("                    FETCH NEXT FROM InsertCursor INTO @curId");
                _writer.WriteIndentBlank("                    WHILE @@FETCH_STATUS = 0");
                _writer.WriteIndentBlank("                    BEGIN");
                _writer.WriteIndentBlank("		                INSERT INTO [dbo].meridian_updates(proto, proto_id, ");
                _writer.WriteIndentBlank("			                [action], dt, instance) VALUES('{0}',", proto.Backend);
                _writer.WriteIndentBlank("			                @curId, 0, GETDATE(), SYSTEM_USER)");
                _writer.WriteIndentBlank("                        FETCH NEXT FROM InsertCursor INTO @curId");
                _writer.WriteIndentBlank("                    END");
                _writer.WriteIndentBlank("                    CLOSE InsertCursor");
                _writer.WriteIndentBlank("                    DEALLOCATE InsertCursor");
                _writer.WriteIndentBlank("                END");
                _writer.WriteIndentBlank("                GO");

                triggerName = proto.Backend + "_update";
                _writer.WriteIndentBlank("                GO");
                _writer.WriteIndentBlank("                IF OBJECT_ID ('{0}', 'TR') IS NOT NULL", triggerName);
                _writer.WriteIndentBlank("                   DROP TRIGGER dbo.{0};", triggerName);
                _writer.WriteIndentBlank("                GO");
                _writer.WriteIndentBlank("                CREATE TRIGGER [dbo].[{0}]", triggerName);
                _writer.WriteIndentBlank("                   ON  [dbo].[{0}]", proto.Backend);
                _writer.WriteIndentBlank("                   AFTER UPDATE");
                _writer.WriteIndentBlank("                AS ");
                _writer.WriteIndentBlank("                BEGIN");
                _writer.WriteIndentBlank("                    SET NOCOUNT ON;");
                _writer.WriteIndentBlank("                    DECLARE InsertCursor CURSOR FOR SELECT {0} FROM Inserted", keyName);
                _writer.WriteIndentBlank("                    DECLARE @curId bigint");
                _writer.WriteIndentBlank("                    OPEN InsertCursor");
                _writer.WriteIndentBlank("                    FETCH NEXT FROM InsertCursor INTO @curId");
                _writer.WriteIndentBlank("                    WHILE @@FETCH_STATUS = 0");
                _writer.WriteIndentBlank("                    BEGIN");
                _writer.WriteIndentBlank("		                INSERT INTO [dbo].meridian_updates(proto, proto_id, ");
                _writer.WriteIndentBlank("			                [action], dt, instance) VALUES('{0}',", proto.Backend);
                _writer.WriteIndentBlank("			                @curId, 1, GETDATE(), SYSTEM_USER)");
                _writer.WriteIndentBlank("                        FETCH NEXT FROM InsertCursor INTO @curId");
                _writer.WriteIndentBlank("                    END");
                _writer.WriteIndentBlank("                    CLOSE InsertCursor");
                _writer.WriteIndentBlank("                    DEALLOCATE InsertCursor");
                _writer.WriteIndentBlank("                END");
                _writer.WriteIndentBlank("                GO");

                triggerName = proto.Backend + "_delete";
                _writer.WriteIndentBlank("                GO");
                _writer.WriteIndentBlank("                IF OBJECT_ID ('{0}', 'TR') IS NOT NULL", triggerName);
                _writer.WriteIndentBlank("                   DROP TRIGGER dbo.{0};", triggerName);
                _writer.WriteIndentBlank("                GO");
                _writer.WriteIndentBlank("                CREATE TRIGGER [dbo].[{0}]", triggerName);
                _writer.WriteIndentBlank("                   ON  [dbo].[{0}]", proto.Backend);
                _writer.WriteIndentBlank("                   AFTER DELETE");
                _writer.WriteIndentBlank("                AS ");
                _writer.WriteIndentBlank("                BEGIN");
                _writer.WriteIndentBlank("                    SET NOCOUNT ON;");
                _writer.WriteIndentBlank("                    DECLARE InsertCursor CURSOR FOR SELECT {0} FROM Deleted", keyName);
                _writer.WriteIndentBlank("                    DECLARE @curId bigint");
                _writer.WriteIndentBlank("                    OPEN InsertCursor");
                _writer.WriteIndentBlank("                    FETCH NEXT FROM InsertCursor INTO @curId");
                _writer.WriteIndentBlank("                    WHILE @@FETCH_STATUS = 0");
                _writer.WriteIndentBlank("                    BEGIN");
                _writer.WriteIndentBlank("		                INSERT INTO [dbo].meridian_updates(proto, proto_id, ");
                _writer.WriteIndentBlank("			                [action], dt, instance) VALUES('{0}',", proto.Backend);
                _writer.WriteIndentBlank("			                @curId, 2, GETDATE(), SYSTEM_USER)");
                _writer.WriteIndentBlank("                        FETCH NEXT FROM InsertCursor INTO @curId");
                _writer.WriteIndentBlank("                    END");
                _writer.WriteIndentBlank("                    CLOSE InsertCursor");
                _writer.WriteIndentBlank("                    DEALLOCATE InsertCursor");
                _writer.WriteIndentBlank("                END");
                _writer.WriteIndentBlank("                GO");

            }

            _writer.WriteIndentBlank("*/");
        }


        public override void Properties(CSharpWriter _writer)
        {
            TriggerGenerator(_writer);
            base.Properties(_writer);

            _writer.WriteIndentBlank("public string ConnectionString {{get; set;}}");

            foreach (var proto in m_OperationContext.GetAllProtos())
            {
                _writer.WriteIndentBlank("public protoStore.{0}Store {0}Store {{get; set;}}", proto.Name);
            }


            var compositions = m_OperationContext.GetAllCompositions();

            //var backends = compositions.Select(s => s.MediatorBackend).Distinct();

            foreach (var cmp in compositions)
            {



                using (_writer.Block("public class {0}", cmp.MediatorBackend))
                {
                    var fields = m_BackendOperationContext.GetTableFields(cmp.MediatorBackend);
                    foreach (var fieldDescription in fields)
                    {
                        _writer.WriteIndentBlank("public {0} {1} {{get; set;}}", fieldDescription.FieldType.ToCSharp(), fieldDescription.Name);
                    }

                    _writer.WriteIndentBlank("public proto.{0} {1} {{get; set;}}", cmp.FirstElementName, cmp.FirstElementName);
                    _writer.WriteIndentBlank("public proto.{0} {1} {{get; set;}}", cmp.SecondElementName, cmp.SecondElementName);


                    var fieldList = String.Join(", ", fields.Select(s => s.Name));

                    using (_writer.Block("public static List<{0}> Select({1}Connection _connection, Meridian _mediator)", 
                        cmp.MediatorBackend, AdoNetPrefix))
                    {
                        _writer.WriteIndentEol("var res = new List<{0}>();", cmp.MediatorBackend);
                        _writer.WriteIndentEol("var cmd = new {2}Command(\"SELECT {0} FROM {1}\")",
                            fieldList, cmp.MediatorBackend, AdoNetPrefix);
                        _writer.WriteIndentEol("cmd.Connection = _connection");
                        _writer.WriteIndentBlank("using(var reader = cmd.ExecuteReader())");
                        using (_writer.Block("while(reader.Read())"))
                        {
                            _writer.WriteIndentEol("var item = new {0}()", cmp.MediatorBackend);
                            _writer.WriteIndentEol("item.LoadFromReader(reader)");

                            using (_writer.Block("if((item.{1} > 0) && _mediator.{0}Store.Exists(item.{1}))", cmp.FirstElementName, cmp.MediatorFirstKeyName))
                            {
                                _writer.WriteIndentEol("item.{0} = _mediator.{0}Store.Get(item.{1})", cmp.FirstElementName, cmp.MediatorFirstKeyName);
                                using (_writer.Block("if((item.{1} > 0) && _mediator.{0}Store.Exists(item.{1}))", cmp.SecondElementName, cmp.MediatorSecondKeyName))
                                {
                                    _writer.WriteIndentEol("item.{0}.{2}.Add(item.{1})", cmp.FirstElementName, cmp.SecondElementName, cmp.Name);
                                    _writer.WriteIndentEol("item.{0} = _mediator.{0}Store.Get(item.{1})", cmp.SecondElementName, cmp.MediatorSecondKeyName);
                                    _writer.WriteIndentEol("item.{0}.{2}.Add(item.{1})", cmp.SecondElementName, cmp.FirstElementName, cmp.Name);
                                }
                            }

                            _writer.WriteIndentEol("res.Add(item)");
                        }

                        _writer.WriteIndentEol("return res");
                    }

                    using (_writer.Block("public void LoadFromReader({0}DataReader _reader)", AdoNetPrefix))
                    {
                        foreach (var f in fields)
                        {
                            _writer.WriteIndentEol("{0} = _reader[\"{0}\"].GetType() != typeof(System.DBNull) ? ({1})_reader[\"{0}\"] : {2}", f.Name, f.FieldType.ToCSharp(), f.FieldType.DefaultCSharp());
                        }
                    }

                }
                _writer.WriteIndentBlank("public List<{0}> {0}Mediator {{get; set;}}", cmp.MediatorBackend);
            }

        }

        public override void Functions(CSharpWriter _writer)
        {
            var storedProcs = m_BackendOperationContext.GetStoredProcedures();
            foreach (var proc in storedProcs)
            {
                var parameterList =
                    String.Join(", ",
                    proc.ParameterTypes.Select(s => s.FieldType.ToCSharp() + " @" + s.Name.Replace("@", "")).ToArray());

                using (_writer.Block("public void Call{0}({1})", proc.Name, parameterList))
                {
                    using (_writer.Block("using(var connection = new {0}Connection(ConnectionString))", AdoNetPrefix))
                    {
                        _writer.WriteIndentEol("connection.Open()");
                        _writer.WriteIndentEol("var command = new {0}Command(\"{1}\", connection) ", AdoNetPrefix, proc.Name);
                        _writer.WriteIndentEol("command.CommandType = System.Data.CommandType.StoredProcedure");
                        foreach (var parameter in proc.ParameterTypes)
                        {
                            _writer.WriteIndentEol("command.Parameters.AddWithValue(\"{0}\", @{2})",
                                parameter.Name, "", parameter.Name.Replace("@", ""));
                        }

                        _writer.WriteIndentEol("command.ExecuteNonQuery()");

                        _writer.WriteIndentEol("connection.Close()");
                    }
                }
            }


            base.Functions(_writer);
            _writer.WriteFunction("public", "void", "Init", new string[] { "string" }, new string[] { "_connection" });
            _writer.OpenBlock();
            _writer.WriteIndentEol("ConnectionString = _connection");
            _writer.WriteIndentBlank("using(var connection = new {0}Connection(_connection))", AdoNetPrefix);
            _writer.OpenBlock();
            _writer.WriteIndentEol("m_Meridian = new Meridian()");
            _writer.WriteIndentEol("m_Meridian.ConnectionString = _connection");
            _writer.WriteIndentEol("connection.Open()");
            _writer.WriteIndentEol("m_Meridian.Load(connection)");
            _writer.WriteIndentEol("connection.Close()");
            _writer.CloseBlock();
            _writer.CloseBlock();


            //base.Functions(_writer);
            _writer.WriteFunction("public", "void", "Load", new string[] { AdoNetPrefix + "Connection" }, new string[] { "_connection" });
            _writer.OpenBlock();


            var protos = m_OperationContext.GetAllProtos();

            foreach (var proto in protos)
            {
                _writer.WriteIndentEol("{0}Store = new protoStore.{0}Store()", proto.Name);

                if(!proto.SkipLoad)
                _writer.WriteIndentEol("{0}Store.Select(_connection)", proto.Name);
            }

            foreach (var proto in protos)
            {
                _writer.WriteIndentEol("{0}Store.LoadAggregations(this)", proto.Name);
                if ((proto.Compositions.Count() > 0) || (proto.InlineCompositions.Count() > 0))
                {
                    _writer.WriteIndentEol("{0}Store.LoadCompositions(this)", proto.Name);
                }
            }


            /*            foreach (var proto in protos)
                        {
                            if (proto.Compositions.Count() > 0)
                            {
                                _writer.WriteIndentEol("{0}Store.LoadCompositions(this)", proto.Name);
                            }
                        } */

            var compositions = m_OperationContext.GetAllCompositions();
            var backends = compositions.Select(s => s.MediatorBackend).Distinct();

            foreach (var backend in backends)
            {
                _writer.WriteIndentEol("{0}Mediator = {0}.Select(_connection, this)", backend);
            }



            _writer.CloseBlock();
            /*        public static Meridian Default
                    {
                        get
                        {
                            return m_Meridian;
                        }
                    }

                    private static Meridian m_Meridian = new Meridian();

                    public void TestStores()
                    {
                        Tracer.I.Debug("Loaded accountsStore {0}", accountsStore.All().Count);
                    }
                        */

            using (_writer.Block("public static {0} Default", ClassName))
            {
                using (_writer.Block("get"))
                {
                    _writer.WriteIndentEol("return m_Meridian");
                }
            }

            _writer.WriteIndentEol("private static Meridian m_Meridian = new Meridian()");

            using (_writer.Block("public void TestStores()"))
            {
                foreach (var proto in protos)
                {
                    _writer.WriteIndentEol("Tracer.I.Debug(\"Loaded {0}Store: {{0}}\", {0}Store.All().Count)", proto.Name);
                }
            }

        }
    }
}