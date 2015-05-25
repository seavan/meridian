using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace meridian.diagram
{

    public abstract class BackendOperationContextBase : IBackendOperationContext
    {
        protected SqlTableMirrorList m_Mirror = new SqlTableMirrorList();

        protected List<CallableDescription> m_StoredProcedures = new List<CallableDescription>();


        public BackendOperationContextBase(string _connectionString, bool _loadSchema = true, bool _loadStoredProcs = true)
        {
            m_ConnectionString = _connectionString;
            if (_loadSchema)
            {
                LoadSchema();
            }

            if (_loadStoredProcs)
            {
                LoadStoredProcs();
            }
            //GetAllNullBits();
        }

        protected string m_ConnectionString { get; set; }

        public abstract void LoadSchema();
        public abstract void LoadStoredProcs();

        public void CreateTable(string _name)
        {
            throw new System.NotImplementedException();
        }

        public void DropTable(string _name)
        {
            throw new System.NotImplementedException();
        }

        public void AddColumn(string _tableName, string _name, FieldType _type)
        {
            throw new System.NotImplementedException();
        }

        public void DropColumn(string _tableName, string _name)
        {
            throw new System.NotImplementedException();
        }

        public void AddPrimary(string _tableName)
        {
            throw new System.NotImplementedException();
        }

        public void AddForeign(string _foreignTable, string _relation)
        {
            throw new System.NotImplementedException();
        }

        public void DropForeign(string _foreignTable, string _relation)
        {
            throw new System.NotImplementedException();
        }

        public void RenameColumn(string _tableName, string _oldName, string _newName)
        {
            throw new System.NotImplementedException();
        }

        public void ChangeColumnType(string _tableName, string _columnName, FieldType _newType)
        {
            throw new System.NotImplementedException();
        }

        public bool HasTable(string _name)
        {
            return m_Mirror.Exists(s => s.Name.Equals(_name));
        }

        public bool HasField(string _tableName, string _name)
        {
            return HasTable(_tableName) && m_Mirror.Get(_tableName).Fields.Exists(s => s.Name.Equals(_name));
        }

        public bool HasCorrectFieldType(string _tableName, string _name, FieldType _type)
        {
            return HasTable(_tableName) && m_Mirror.Get(_tableName).Fields.Single(s => s.Name.Equals(_name)).LocalType.Equals(_type);
        }

        public FieldDescription[] GetTableFields(string _name)
        {
            return m_Mirror.Get(_name).Fields.Select(s => new FieldDescription() { Name = s.Name, Backend = s.Name, FieldType = s.LocalType, MaxLength = s.DbLength }).ToArray();
        }

        public CallableDescription[] GetStoredProcedures()
        {
            return m_StoredProcedures.ToArray();
        }

        public string[] GetTableNames()
        {
            return m_Mirror.Select(s => s.Name).ToArray();
        }

        public FieldType GetFieldType(string _tableName, string _name)
        {
            return m_Mirror.Get(_tableName).Fields.Single(s => s.Name.Equals(_name)).LocalType;
        }
    }
}