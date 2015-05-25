using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using meridian.core;

namespace meridian.diagram
{
    public class MsSqlBackendOperationContext : BackendOperationContextBase
    {
        public MsSqlBackendOperationContext(string _connectionString, bool _loadSchema, bool _loadStoredProcs) : base(_connectionString, _loadSchema, _loadStoredProcs)
        {
        }

        public void GetAllNullBits()
        {
            var list = new List<SqlFieldMirror>();
            m_Mirror.ForEach(s => list.AddRange(s.Fields));

            list.Where(s => (s.DbType == "bit") && (s.DbIsNullable == "YES")).ToList().ForEach(s =>  Tracer.I.Notice(s.DbIsNullable + ":" + s.TableName + "." + s.Name));
        }

        protected DataTable m_Schema { get; set; }

        public override void LoadStoredProcs()
        {
            using (var conn = new SqlConnection(m_ConnectionString))
            {
                conn.Open();
                // get stored procedures

                var getProceduresCommand = new SqlCommand(
                    @"SELECT SCHEMA_NAME(SCHEMA_ID) AS [Schema], 
SO.name AS [ObjectName],
SO.Type_Desc AS [ObjectType],
P.parameter_id AS [ParameterID],
P.name AS [ParameterName],
TYPE_NAME(P.user_type_id) AS [ParameterDataType],
P.max_length AS [ParameterMaxBytes],
P.is_output AS [IsOutPutParameter]
FROM sys.objects AS SO
INNER JOIN sys.parameters AS P 
ON SO.OBJECT_ID = P.OBJECT_ID
WHERE SO.OBJECT_ID IN ( SELECT OBJECT_ID 
FROM sys.objects
WHERE TYPE IN ('P'))
ORDER BY [Schema], SO.name, P.parameter_id"
                    );

                getProceduresCommand.Connection = conn;

                m_StoredProcedures.Clear();
                
                using (var reader = getProceduresCommand.ExecuteReader())
                {
                    var currentProcedureDescription = new StoredProcedureDescription();
                    while (reader.Read())
                    {
                        var objectName = reader["ObjectName"].ToString();
                        var objectType = reader["ObjectType"].ToString();
                        var parameterId = Convert.ToInt32(reader["ParameterID"]);
                        var parameterName = reader["ParameterName"].ToString();
                        var parameterDataType = reader["ParameterDataType"].ToString();
                        var parameterMaxBytes = Convert.ToInt32(reader["ParameterMaxBytes"]);
                        var isOutParameter = Convert.ToInt32(reader["IsOutPutParameter"]);

                        if (String.IsNullOrEmpty(currentProcedureDescription.Name))
                        {
                            currentProcedureDescription.Name = objectName;
                        }

                        if (currentProcedureDescription.Name != objectName)
                        {
                            m_StoredProcedures.Add(currentProcedureDescription);
                            currentProcedureDescription = new StoredProcedureDescription();
                            currentProcedureDescription.Name = objectName;
                        }

                        

                        var parameterDescription = new ParameterDescription();

                        parameterDescription.Name = parameterName;
                        try
                        {
                            parameterDescription.FieldType = SqlFieldMapper.Map(parameterDataType, parameterMaxBytes);
                        }
                        catch (Exception _e)
                        {
                            Tracer.I.Error("id: {0}, name: {1}, dataType: {2}, maxBytes: {3}", parameterId, parameterName, parameterDataType, parameterMaxBytes);
                            continue;
                            ;
                            
                        }
                        
                        parameterDescription.MaxLength = parameterMaxBytes;
                        parameterDescription.IsOut = isOutParameter > 0;

                        if (parameterId == 0)
                        {
                            currentProcedureDescription.ReturnType = parameterDescription;
                        }
                        else
                        {
                            currentProcedureDescription.ParameterTypes.Add(parameterDescription);
                        }
                    }

                    if (!String.IsNullOrEmpty(currentProcedureDescription.Name))
                    {
                        m_StoredProcedures.Add(currentProcedureDescription);
                    }
                }

            }
        }

        public override void LoadSchema()
        {
            using (var conn = new SqlConnection(m_ConnectionString))
            {
                conn.Open();
                m_Schema = conn.GetSchema(System.Data.SqlClient.
                                              SqlClientMetaDataCollectionNames.Tables);
                var tableSchema = conn.GetSchema(System.Data.SqlClient.SqlClientMetaDataCollectionNames.Columns);
                var columnList = new MultiValueDictionary<string, DataRow>();
                for (int i = 0; i < tableSchema.Rows.Count; ++i)
                {
                    var row = tableSchema.Rows[i];
                    columnList.Add(row["table_name"].ToString() , row);
                }

                for (int i = 0; i < m_Schema.Rows.Count; ++i)
                {
                    var row = m_Schema.Rows[i];
                    var t = m_Mirror.New();
                    t.Name = row["table_name"].ToString();
                    t.Fields.AddRange(columnList.GetValues(t.Name, true).Select(s => new SqlFieldMirror() 
                        { 
                            TableName = t.Name,
                            DbIsNullable = s["is_nullable"].ToString(),
                            Name = s["column_name"].ToString(), 
                            DbType = s["data_type"].ToString(), 
                            DbLength = s["character_maximum_length"].GetType() != typeof(System.DBNull) ? (int)s["character_maximum_length"] : 0
                        }
                        ));

                }

              
            }
        }
    }
}