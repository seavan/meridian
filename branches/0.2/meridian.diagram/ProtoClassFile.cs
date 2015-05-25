using System.Collections.Generic;
using System.Linq;

namespace meridian.diagram
{
    public class InlineCompositionEqualityComparer : IEqualityComparer<InlineCompositionDescription>
    {
        #region IEqualityComparer<InlineCompositionDescription> Members

        public bool Equals(InlineCompositionDescription x, InlineCompositionDescription y)
        {
            return x.MediatorKeyName.Equals(y.MediatorKeyName);
        }

        public int GetHashCode(InlineCompositionDescription obj)
        {
            return obj.MediatorKeyName.GetHashCode();
        }

        #endregion
    }


    public class ProtoClassFile : ClassFile
    {
        public ProtoClassFile(ProtoDescription _descr)
        {
            ClassName = _descr.Name;
            m_Description = _descr;
        }

        private ProtoDescription m_Description;

        public override void Constructor(CSharpWriter _writer)
        {
            base.Constructor(_writer);


            if (m_Description.Aggregations != null)
            {
                foreach (var agg in m_Description.Aggregations)
                {
                    _writer.WriteIndentEol("{0} = new List<{1}>()", agg.Name, agg.ForeignElementName);
                }
            }


            if (m_Description.Compositions != null)
            {
                foreach (var comp in m_Description.Compositions)
                {
                    _writer.WriteIndentEol("{0} = new List<{1}>()", comp.Name, comp.SecondElementName);
                }
            }

            if (m_Description.InlineCompositions != null)
            {
                foreach (var comp in m_Description.InlineCompositions.Distinct(new InlineCompositionEqualityComparer()))
                {
                    // todo
                    if (comp.Reversed && !string.IsNullOrEmpty(comp.FirstElementInterface) && (comp.FirstElementInterface != "None"))
                    {
                        _writer.WriteIndentEol("{0}_items = new List<{1}>()", comp.MediatorKeyName, comp.FirstElementInterface);
                    }
                    else
                    {
                        _writer.WriteIndentEol("{0}_items = new List<{1}>()", comp.MediatorKeyName, comp.SecondElementName);
                    }
                }
            }
        }

        public override void Fields(CSharpWriter _writer)
        {
            base.Fields(_writer);

            foreach (var f in m_Description.Fields)
            {
                _writer.WriteIndentEol("private {0} m_{1} = {2}", f.FieldType.ToCSharp(), f.Name, f.FieldType.DefaultCSharp());
                _writer.WriteIndentBlank("internal bool mc_{1} {{ get; private set; }}", f.FieldType.ToCSharp(), f.Name);
            }
        }


        public override void Properties(CSharpWriter _writer)
        {
            base.Properties(_writer);

            if (!string.IsNullOrEmpty(m_Description.CustomKeyName))
            {
                _writer.WriteIndentBlank("public long id");
                _writer.OpenBlock();

                _writer.WriteIndentBlank("get");
                _writer.OpenBlock();
                _writer.WriteIndentEol("return m_{0}", m_Description.CustomKeyName);
                _writer.CloseBlock();

                _writer.WriteIndentBlank("set");
                _writer.OpenBlock();
                _writer.WriteIndentEol("m_{0} = ({1})value", m_Description.CustomKeyName, m_Description.Fields.Where(s => s.Name.Equals(m_Description.CustomKeyName)).Select(s => s.FieldType.ToCSharp()).FirstOrDefault());
                _writer.WriteIndentEol("// call update worker thread");
                _writer.CloseBlock();

                _writer.CloseBlock();

            }

            _writer.WriteIndentBlank("/* metafile template ");
            using (_writer.Block("internal class {0}_meta", ClassName))
            {

                foreach (var f in m_Description.Fields)
                {
                    _writer.WriteIndentBlank("public {0} {1} {{ get; set; }}", f.FieldType.ToCSharp(), NormalizeName(f.Name));
                }
            }
            _writer.WriteIndentBlank(" metafile template */");

            foreach (var f in m_Description.Fields)
            {
                _writer.WriteIndentBlank("public {0} {1}", f.FieldType.ToCSharp(), NormalizeName(f.Name));
                _writer.OpenBlock();

                _writer.WriteIndentBlank("get");
                _writer.OpenBlock();
                _writer.WriteIndentEol("return m_{0}", f.Name);
                _writer.CloseBlock();

                _writer.WriteIndentBlank("set");
                _writer.OpenBlock();
                using (_writer.Block("if(m_{0} != value)", f.Name))
                {
                    _writer.WriteIndentEol("m_{0} = value != null ? value : {1}", f.Name, f.FieldType.DefaultCSharp());

                    if (f.FieldType == FieldType.ftDateTime)
                    {
                        _writer.WriteIndentEol("if(m_{0}.Year < 1800) value = DateTime.MinValue", f.Name, f.FieldType.DefaultCSharp());

                    }

                    _writer.WriteIndentEol("mc_{0} = true", f.Name);
                    _writer.WriteIndentEol("// call update worker thread");
                }
                _writer.CloseBlock();

                _writer.CloseBlock();
            }

            if (m_Description.Aggregations != null)
            {
                foreach (var agg in m_Description.Aggregations)
                {
                    using (_writer.Block("public List<{1}> {0}", agg.Name, agg.ForeignElementName))
                    {
                        _writer.WriteIndentBlank("get; set; ");
                    }

                    using (_writer.Block("public {1} Add_{0}({1} _item)", agg.Name, agg.ForeignElementName))
                    {
                        // todo
                        _writer.WriteIndentEol("_item.{0} = (int)id", agg.ForeignKey);
                        _writer.WriteIndentEol("_item = Meridian.Default.{0}Store.Insert(_item)", agg.ForeignElementName);
                        _writer.WriteIndentEol("_item.LoadAggregations(Meridian.Default)");
                        //_writer.WriteIndentEol("{0}.Add(_item)", agg.Name);
                        _writer.WriteIndentEol("return _item", agg.Name);
                    }

                    using (_writer.Block("public {1} Remove_{0}({1} _item)", agg.Name, agg.ForeignElementName))
                    {
                        // todo
                        _writer.WriteIndentEol("{0}.Remove(_item)", agg.Name);
                        _writer.WriteIndentEol("return _item", agg.Name);
                    }
                }
            }

            if (m_Description.ParentAggregations != null)
            {
                foreach (var agg in m_Description.ParentAggregations)
                {
                    using (_writer.Block("public {0} {1}_{0}", agg.ThisElementName, agg.Name))
                    {
                        _writer.WriteIndentBlank("get; set; ");
                    }
                }
            }


            foreach (var comp in m_Description.Compositions)
            {
                using (_writer.Block("public List<{1}> {0}", comp.Name, comp.SecondElementName))
                {
                    _writer.WriteIndentBlank("get; set; ");
                }
            }

            if (m_Description.InlineCompositions != null)
            {
                foreach (var comp in m_Description.InlineCompositions.Distinct(new InlineCompositionEqualityComparer()))
                {
                    using (_writer.Block("public List<{1}> {0}_items", comp.MediatorKeyName,
                        (comp.Reversed && comp.FirstElementInterface != "None" && !string.IsNullOrEmpty(comp.FirstElementInterface))
                        ? comp.FirstElementInterface : comp.SecondElementName))
                    {
                        _writer.WriteIndentBlank("get; set; ");
                    }
                }
            }

        }

        public override void Functions(CSharpWriter _writer)
        {
            base.Functions(_writer);
            _writer.WriteFunction("public", "void", "LoadFromReader", new string[] { AdoNetPrefix + "DataReader" }, new string[] { "_reader" });
            _writer.OpenBlock();

            foreach (var f in m_Description.Fields)
            {
                if ((f.FieldType == FieldType.ftDateTime) && AdoNetPrefix == "MySql")
                {
                    _writer.WriteIndentEol("try {{ m_{0} = _reader[\"{0}\"].GetType() != typeof(System.DBNull) ? Convert.ToDateTime(_reader[\"{0}\"]) : {2}; }} catch(System.Exception) {{ m_{0} = new DateTime(); }}", f.Name, f.FieldType.ToCSharp(), f.FieldType.DefaultCSharp());
                }
                else
                {
                    _writer.WriteIndentEol("m_{0} = _reader[\"{0}\"].GetType() != typeof(System.DBNull) ? ({1})_reader[\"{0}\"] : {2}", f.Name, f.FieldType.ToCSharp(), f.FieldType.DefaultCSharp());
                }

                _writer.WriteIndentEol("mc_{0} = false", f.Name);
            }

            _writer.CloseBlock();

            using (_writer.Block("public void LoadAggregations(Meridian _meridian)"))
            {
                /*if (m_Description.Aggregations != null)
                {
                    foreach (var agg in m_Description.Aggregations)
                    {
                        _writer.WriteIndentEol("{0}.Clear()", agg.Name);
                        _writer.WriteIndentEol("{0}.AddRange(_meridian.{1}Store.All().Where( s => s.{2}.Equals(this.id)))", agg.Name, agg.ForeignElementName, agg.ForeignKey);
                    }
                }*/

                if (m_Description.ParentAggregations != null)
                {
                    foreach (var agg in m_Description.ParentAggregations)
                    {
                        using (_writer.Block("if(({0} > 0) && (_meridian.{1}Store.Exists({0})))", agg.ForeignKey, agg.ThisElementName))
                        {
                            _writer.WriteIndentEol("this.{0}_{1} = _meridian.{1}Store.Get({2});", agg.Name, agg.ThisElementName, agg.ForeignKey);
                            _writer.WriteIndentBlank("if(this.{0}_{1}.{0}.IndexOf(this) == -1)", agg.Name, agg.ThisElementName, agg.ForeignKey);
                            _writer.WriteIndentEol("this.{0}_{1}.{0}.Add(this)", agg.Name, agg.ThisElementName, agg.ForeignKey);
                        }
                    }
                }
            }

            using (_writer.Block("public void DeleteAggregations()"))
            {
                if (m_Description.ParentAggregations != null)
                {
                    foreach (var agg in m_Description.ParentAggregations)
                    {
                        using (_writer.Block("if(this.{0}_{1} != null)", agg.Name, agg.ThisElementName, agg.ForeignKey))
                        {
                            _writer.WriteIndentEol("this.{0}_{1}.Remove_{0}(this)", agg.Name, agg.ThisElementName, agg.ForeignKey);
                        }
                    }
                }
            }

            using (_writer.Block("public void LoadCompositions(Meridian _meridian)"))
            {
                if (m_Description.Compositions != null)
                {
                    foreach (var comp in m_Description.Compositions)
                    {
                        /*                        _writer.WriteIndentEol("{0}.Clear()", comp.Name);
                                                // todo
                                                _writer.WriteIndentEol("var items = _meridian.{0}Mediator.Where(s => s.{1}.Equals(id)).Select( c => c.{2})", comp.MediatorBackend, comp.MediatorFirstKeyName, comp.MediatorSecondKeyName);
                                                _writer.WriteIndentEol("{0}.AddRange(_meridian.{1}Store.All().Where( s => items.Count(i => i.Equals(s.id)) > 0))", comp.Name, comp.SecondElementName, comp.MediatorSecondKeyName);*/
                    }
                }

                if (m_Description.InlineCompositions != null)
                {
                    _writer.WriteIndentEol("string[] keyIds = null");
                    foreach (var comp in m_Description.InlineCompositions.Where(s => !s.Reversed))
                    {
                        _writer.WriteIndentEol("keyIds = {0}.Split(',')", comp.MediatorKeyName);
                        using (_writer.Block("foreach(var foreignIdStr in keyIds)"))
                        {
                            _writer.WriteIndentEol("long foreignId = 0");
                            using (_writer.Block("if(long.TryParse(foreignIdStr, out foreignId))"))
                            {
                                using (_writer.Block("if((foreignId > 0) && (_meridian.{1}Store.Exists(foreignId)))", comp.MediatorKeyName, comp.SecondElementName))
                                {
                                    _writer.WriteIndentEol("var foreignItem = _meridian.{1}Store.Get(foreignId)", comp.MediatorKeyName, comp.SecondElementName);
                                    _writer.WriteIndentEol("this.{0}_items.Add(foreignItem)", comp.MediatorKeyName);
                                    _writer.WriteIndentEol("foreignItem.{0}_items.Add(this)", comp.MediatorKeyName);
                                }
                            }
                        }
                    }
                }
            }

            using (_writer.Block("public void SaveCompositions(Meridian _meridian)"))
            {
                if (m_Description.InlineCompositions != null)
                {
                    foreach (var comp in m_Description.InlineCompositions.Where(s => !s.Reversed))
                    {
                        _writer.WriteIndentEol("{0} = string.Join(\",\", this.{0}_items.Select(s => s.id))", comp.MediatorKeyName);
                    }
                }
            }

            using (_writer.Block("public void DeleteCompositions(Meridian _meridian)"))
            {
                if (m_Description.Compositions != null)
                {
                    foreach (var comp in m_Description.Compositions)
                    {
                        /*                        _writer.WriteIndentEol("{0}.Clear()", comp.Name);
                                                // todo
                                                _writer.WriteIndentEol("var items = _meridian.{0}Mediator.Where(s => s.{1}.Equals(id)).Select( c => c.{2})", comp.MediatorBackend, comp.MediatorFirstKeyName, comp.MediatorSecondKeyName);
                                                _writer.WriteIndentEol("{0}.AddRange(_meridian.{1}Store.All().Where( s => items.Count(i => i.Equals(s.id)) > 0))", comp.Name, comp.SecondElementName, comp.MediatorSecondKeyName);*/
                    }
                }

                if (m_Description.InlineCompositions != null)
                {
                    _writer.WriteIndentEol("string[] keyIds = null");

                    foreach (var comp in m_Description.InlineCompositions.Where(s => !s.Reversed))
                    {
                        _writer.WriteIndentEol("keyIds = {0}.Split(',')", comp.MediatorKeyName);
                        using (_writer.Block("foreach(var foreignIdStr in keyIds)"))
                        {
                            _writer.WriteIndentEol("long foreignId = 0");
                            using (_writer.Block("if(long.TryParse(foreignIdStr, out foreignId))"))
                            {
                                using (_writer.Block("if((foreignId > 0) && (_meridian.{1}Store.Exists(foreignId)))", comp.MediatorKeyName, comp.SecondElementName))
                                {
                                    _writer.WriteIndentEol("var foreignItem = _meridian.{1}Store.Get(foreignId)", comp.MediatorKeyName, comp.SecondElementName);
                                    _writer.WriteIndentEol("if(this.{0}_items.Contains(foreignItem)) this.{0}_items.Remove(foreignItem)", comp.MediatorKeyName);
                                    _writer.WriteIndentEol("if(foreignItem.{0}_items.Contains(this)) foreignItem.{0}_items.Remove(this)", comp.MediatorKeyName);
                                }
                            }
                        }
                    }
                }


            }
        }

    }
}