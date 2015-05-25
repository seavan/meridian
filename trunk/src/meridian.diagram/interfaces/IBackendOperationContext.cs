namespace meridian.diagram
{
    public interface IBackendOperationContext
    {
        void CreateTable(string _name);
        void DropTable(string _name);
        void AddColumn(string _tableName, string _name, FieldType _type);
        void DropColumn(string _tableName, string _name);
        /// <summary>
        /// will not be used for now
        /// </summary>
        void AddPrimary(string _tableName);
        void AddForeign(string _foreignTable, string _relation);
        void DropForeign(string _foreignTable, string _relation);
        void RenameColumn(string _tableName, string _oldName, string _newName);
        void ChangeColumnType(string _tableName, string _columnName, FieldType _newType);

        bool HasTable(string _name);
        bool HasField(string _tableName, string _name);
        bool HasCorrectFieldType(string _tableName, string _name, FieldType _type);

        FieldDescription[] GetTableFields(string _name);
        string[] GetTableNames();

        FieldType GetFieldType(string _tableName, string _name);

        CallableDescription[] GetStoredProcedures();
    }
}