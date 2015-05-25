using System;
using System.Linq;

namespace meridian.diagram
{
    public class ProtoStoreClassFile : ClassFile
    {
        public ProtoStoreClassFile(ProtoDescription _descr)
        {
            ClassName = _descr.Name + "Store";
            ProtoClassName = "proto." + _descr.Name;
            m_Description = _descr;
            if (_descr.Fields.Count > 0)
            {
                if ( !String.IsNullOrEmpty(_descr.CustomKeyName) || _descr.Fields.Count(s => s.Name.ToLower().Equals("id")) > 0)
                {
                    ProtoPrimaryIdName = "id";
                }
                else if (!String.IsNullOrEmpty(_descr.CustomKeyName))
                {
                    ProtoPrimaryIdName = _descr.CustomKeyName;
                }
                else
                {
                    ProtoPrimaryIdName = _descr.Fields[0].Name;    
                }
                
            }
        }

        public string ProtoClassName { get; set; }
        public string ProtoPrimaryIdName { get; set; }

        private ProtoDescription m_Description;

        public override void Constructor(CSharpWriter _writer)
        {
            base.Constructor(_writer);
            _writer.WriteIndentEol("m_Items = new SortedList<long, {0}>()", ProtoClassName);
        }

        public override void Fields(CSharpWriter _writer)
        {
            base.Fields(_writer);
            _writer.WriteIndentEol("private SortedList<long, {0}> m_Items", ProtoClassName);
        }



        public override void Functions(CSharpWriter _writer)
        {
            base.Functions(_writer);
            using (_writer.Block("public void LoadAggregations(Meridian _meridian)"))
            {
                using (_writer.Block("foreach(var item in m_Items.Values)"))
                {
                    _writer.WriteIndentEol("item.LoadAggregations(_meridian)");
                }
            }

            if ((m_Description.Compositions.Count() > 0) || (m_Description.InlineCompositions.Count() > 0))
            {
                using (_writer.Block("public void LoadCompositions(Meridian _meridian)"))
                {
                    using (_writer.Block("foreach(var item in m_Items.Values)"))
                    {
                        _writer.WriteIndentEol("item.LoadCompositions(_meridian)");
                    }
                }
            }
            _writer.WriteFunction("public", "void", "Select", new string[] { this.AdoNetPrefix + "Connection" }, new string[] { "_connection" });
            _writer.OpenBlock();

            //if(m_Description.

            var fieldList = String.Join(", ", m_Description.Fields.Select(s => this.FieldSeparatorL + s.Name + this.FieldSeparatorR));

            _writer.WriteIndentEol("var cmd = new {2}Command(\"SELECT {0} FROM {1}\")", fieldList, m_Description.Backend, this.AdoNetPrefix);
            _writer.WriteIndentEol("cmd.Connection = _connection");
            _writer.WriteIndentBlank("using(var reader = cmd.ExecuteReader())");
            _writer.OpenBlock();
            _writer.WriteIndentBlank("while(reader.Read())");
            _writer.OpenBlock();
            _writer.WriteIndentEol("var item = new {0}()", ProtoClassName);
            _writer.WriteIndentEol("item.LoadFromReader(reader)");


            if (m_Description.Fields.Count > 0)
            {
                _writer.WriteIndentEol("m_Items[item.{0}] = item", ProtoPrimaryIdName);
            }

            _writer.CloseBlock();

            _writer.CloseBlock();

            _writer.CloseBlock();



            fieldList = String.Join(", ", m_Description.Fields.Where(s => s.FieldType != FieldType.ftTimeStamp && s.Name != "id" ).Select(s => this.FieldSeparatorL + s.Name + this.FieldSeparatorR));
            var parametrizedFieldList = String.Join(", ", m_Description.Fields.Where(s => s.Name != "id").Select(s => "@" + s.Name));

            var SELECT_SCOPE_IDENTITY = "SELECT CAST(scope_identity() AS bigint);";

            // don't do this kids
            if (this.AdoNetPrefix == "MySql")
            {
                SELECT_SCOPE_IDENTITY = "SELECT LAST_INSERT_ID();";
            }


            using (_writer.Block("public {0} Insert({1}Connection _connection, {0} _item)", ProtoClassName, this.AdoNetPrefix))
            {
                
                _writer.WriteIndentEol("var cmd = new {3}Command(\"INSERT INTO {1} ( {0} ) VALUES ( {2} ); {4}\"); ", fieldList, m_Description.Backend, parametrizedFieldList, this.AdoNetPrefix, SELECT_SCOPE_IDENTITY);

                foreach (var f in m_Description.Fields.Where(s => s.Name != "id"))
                {
                    
                    if (f.FieldType == FieldType.ftDateTime)
                    {
                        _writer.WriteIndentEol("cmd.Parameters.Add( new {2}Parameter() {{ ParameterName = \"{0}\", Value = (_item.{1} != null && _item.{1}.Year > 1800) ? _item.{1} : new DateTime(1800, 1, 1) }})", f.Name, NormalizeName(f.Name), this.AdoNetPrefix);                        
                    }
                    else
                    {
                        _writer.WriteIndentEol("cmd.Parameters.Add( new {2}Parameter() {{ ParameterName = \"{0}\", Value = _item.{1} }})", f.Name, NormalizeName(f.Name), this.AdoNetPrefix);                        
                    }

                }

                _writer.WriteIndentEol("cmd.Connection = _connection");

                if (m_Description.Fields.Count(s => s.Name.Equals("id")) > 0)
                {
                    _writer.WriteIndentEol("_item.id = Convert.ToInt64(cmd.ExecuteScalar())");
                }
                else
                {
                    _writer.WriteIndentEol("cmd.ExecuteScalar()");
                }

                _writer.WriteIndentEol("m_Items.Add(_item.{0}, _item)", ProtoPrimaryIdName);
                _writer.WriteIndentEol("_item.LoadAggregations(Meridian.Default)");
                _writer.WriteIndentEol("return _item");
            }

            var updateFieldList = String.Join(", ", m_Description.Fields.Where(s => s.FieldType != FieldType.ftTimeStamp && s.Name != "id").Select(s => this.FieldSeparatorL + s.Name + this.FieldSeparatorR + "= @" + s.Name));

            using (_writer.Block("public {0} Update({1}Connection _connection, {0} _item)", ProtoClassName, this.AdoNetPrefix))
            {
                if (m_Description.Fields.Count(s => s.Name.Equals("id")) > 0)
                {
                    _writer.WriteIndentEol("bool changed =  false");
                    _writer.WriteIndentEol("var cmdText =  \"\"");
                    _writer.WriteIndentEol("var cmd = new {3}Command(\"UPDATE {1} SET {0} WHERE id=@id\"); ", updateFieldList, m_Description.Backend, parametrizedFieldList, this.AdoNetPrefix);

                    foreach (var f in m_Description.Fields)
                    {
                        using (_writer.Block("if(_item.mc_{0})", f.Name))
                        {
                            if(f.Name != "id")
                            {
                                _writer.WriteIndentEol("changed =  true");
                                _writer.WriteIndentEol("cmdText += (cmdText.Length > 0 ? \", \" : \"\") + \"{0} = @{0}\"", f.Name);
                                _writer.WriteIndentEol("cmd.Parameters.Add( new {2}Parameter() {{ ParameterName = \"{0}\", {1} }})", 
                                    f.Name, String.Format(f.FieldType.IsNullableFieldType() ? 
                                    "Value = _item.{0} != null ? (object)_item.{0} : DBNull.Value" : 
                                    "Value = _item.{0}", NormalizeName(f.Name)), this.AdoNetPrefix);
                            }
                        }
                        if(f.Name == "id")
                        {
                            _writer.WriteIndentEol("cmd.Parameters.Add( new {2}Parameter() {{ ParameterName = \"{0}\", Value = _item.{1} }})"
                                , f.Name, f.Name, this.AdoNetPrefix);
                        }
                    }

                    using (_writer.Block("if(changed)"))
                    {
                        _writer.WriteIndentEol("cmd.CommandText =  \"UPDATE {0} SET \" + cmdText + \" WHERE id=@id\"",
                                               m_Description.Backend);
                        _writer.WriteIndentEol("cmd.Connection = _connection");
                        _writer.WriteIndentEol("cmd.ExecuteNonQuery()");
                        _writer.WriteIndentEol("_item.LoadAggregations(Meridian.Default)");
                    }
                    
                }
                _writer.WriteIndentEol("return _item");
            }
            
            using (_writer.Block("public {0} Delete({1}Connection _connection, {0} _item)", ProtoClassName, this.AdoNetPrefix))
            {
                if (m_Description.Fields.Count(s => s.Name.Equals("id")) > 0)
                {
                    _writer.WriteIndentEol("var cmd = new {3}Command(\"DELETE FROM {1} WHERE id=@id\"); ",
                        updateFieldList, m_Description.Backend, parametrizedFieldList, this.AdoNetPrefix);

                    _writer.WriteIndentEol("cmd.Parameters.Add( new {2}Parameter() {{ ParameterName = \"{0}\", Value = _item.{1} }})",
                        "id", "id", this.AdoNetPrefix);

                    _writer.WriteIndentEol("cmd.Connection = _connection");

                    _writer.WriteIndentEol("cmd.ExecuteScalar()");

                }
                _writer.WriteIndentEol("return _item");
            }
            
            using (_writer.Block("public {0} Insert({0} _item)", ProtoClassName))
            {
                _writer.WriteIndentEol("MeridianMonitor.Default.{0}ActionForeground((conn) => Insert(conn, _item));", this.AdoNetPrefix);
                _writer.WriteIndentEol("return _item");
            }

            using (_writer.Block("public {0} Persist({0} _item)", ProtoClassName))
            {
                using (_writer.Block("if(_item.id <= 0)", ProtoClassName))
                {
                    _writer.WriteIndentEol("_item = Insert(_item)");    
                }
                _writer.WriteIndentEol("_item = Update(_item)");
                _writer.WriteIndentEol("return _item");
            }

            using (_writer.Block("public {0} Delete({0} _item)", ProtoClassName))
            {
                if (m_Description.Fields.Count(s => s.Name.Equals("id")) > 0)
                {
                    _writer.WriteIndentEol("_item.DeleteCompositions(Meridian.Default)");
                    _writer.WriteIndentEol("_item.DeleteAggregations()");
                    _writer.WriteIndentEol("m_Items.Remove(_item.id)", ProtoPrimaryIdName);
                    _writer.WriteIndentEol("MeridianMonitor.Default.{0}ActionBackground((conn) => Delete(conn, _item));", this.AdoNetPrefix);
                }
                _writer.WriteIndentEol("return _item");
            }

            using (_writer.Block("public {0} Update({0} _item)", ProtoClassName))
            {
                _writer.WriteIndentEol("MeridianMonitor.Default.{0}ActionBackground((conn) => Update(conn, _item));", this.AdoNetPrefix);
                _writer.WriteIndentEol("return _item");
            }
            
            using (_writer.Block("public IList<{0}> All()", ProtoClassName))
            {
                _writer.WriteIndentEol("return m_Items.Values");
            }

            using (_writer.Block("public {0} Get(long _id)", ProtoClassName))
            {
                _writer.WriteIndentEol("return m_Items[_id]");
            }

            using (_writer.Block("public bool Exists(long _id)", ProtoClassName))
            {
                _writer.WriteIndentEol("return m_Items.ContainsKey(_id)");
            }


            var selectFieldList = String.Join(", ", m_Description.Fields.Select(s => this.FieldSeparatorL + s.Name + this.FieldSeparatorR));

            var selectKeyName = String.IsNullOrEmpty(m_Description.CustomKeyName)
                                    ? "id"
                                    : m_Description.CustomKeyName;

            if (ProtoPrimaryIdName.Equals("id"))
            {

                using (
                    _writer.Block("public void UpdateSync({1}Connection _connection, long _id, Meridian _meridian)",
                                  ProtoClassName, AdoNetPrefix))
                {
                    _writer.WriteIndentBlank("if (!Exists(_id))");
                    _writer.WriteIndentBlank("{{");
                    _writer.WriteIndentBlank("return;");
                    _writer.WriteIndentBlank("}}");
                    _writer.WriteIndentBlank("var item = Get(_id);");
                    _writer.WriteIndentEol("var cmd = new {2}Command(\"SELECT {0} FROM {1} WHERE {3} = \" + _id.ToString())",
                        selectFieldList, m_Description.Backend, this.AdoNetPrefix, selectKeyName);
                    _writer.WriteIndentEol("cmd.Connection = _connection");
                    _writer.WriteIndentBlank("using(var reader = cmd.ExecuteReader())");
                    _writer.OpenBlock();
                    _writer.WriteIndentBlank("while(reader.Read())");
                    _writer.OpenBlock();
                    _writer.WriteIndentEol("item.DeleteCompositions(Meridian.Default)");
                    _writer.WriteIndentEol("item.DeleteAggregations()");
                    _writer.WriteIndentEol("item.LoadFromReader(reader)");
                    _writer.WriteIndentEol("item.LoadAggregations(_meridian)");
                    _writer.WriteIndentEol("item.LoadCompositions(_meridian)");
                    _writer.CloseBlock();
                    _writer.CloseBlock();
                }

                using (
                    _writer.Block("public void InsertSync({1}Connection _connection, long _id, Meridian _meridian)",
                                  ProtoClassName, this.AdoNetPrefix))
                {
                    _writer.WriteIndentEol("if(Exists(_id)) return;");
                    _writer.WriteIndentEol("var cmd = new {2}Command(\"SELECT {0} FROM {1} WHERE {3} = \" + _id.ToString())", 
                        selectFieldList, m_Description.Backend, this.AdoNetPrefix, selectKeyName);
                    _writer.WriteIndentEol("cmd.Connection = _connection");
                    _writer.WriteIndentBlank("using(var reader = cmd.ExecuteReader())");
                    _writer.OpenBlock();
                    _writer.WriteIndentBlank("while(reader.Read())");
                    _writer.OpenBlock();
                    _writer.WriteIndentEol("var item = new {0}()", ProtoClassName);
                    _writer.WriteIndentEol("item.LoadFromReader(reader)");
                    _writer.WriteIndentEol("item.LoadAggregations(_meridian)");
                    _writer.WriteIndentEol("item.LoadCompositions(_meridian)");
                    _writer.WriteIndentEol("m_Items.Add(item.id, item)");
                    _writer.CloseBlock();
                    _writer.CloseBlock();
                }

                using (
                    _writer.Block("public void DeleteSync({1}Connection _connection, long _id, Meridian _meridian)",
                                  ProtoClassName, this.AdoNetPrefix))
                {
                    _writer.WriteIndentBlank("if (!Exists(_id))");
                    _writer.WriteIndentBlank("{{");
                    _writer.WriteIndentBlank("return;");
                    _writer.WriteIndentBlank("}}");
                    _writer.WriteIndentBlank("var item = Get(_id);");
                    _writer.WriteIndentBlank("item.DeleteCompositions(Meridian.Default);");
                    _writer.WriteIndentBlank("item.DeleteAggregations();");
                    _writer.WriteIndentBlank("m_Items.Remove(item.id);");
                }
            }
        }
    }
}