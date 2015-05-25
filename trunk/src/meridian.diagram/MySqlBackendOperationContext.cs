using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using meridian.core;

namespace meridian.diagram
{
    public class MySqlBackendOperationContext : BackendOperationContextBase
    {
        public MySqlBackendOperationContext(string _connectionString)
            : base(_connectionString)
        {
        }
        public override void LoadSchema()
        {
            Tracer.I.Notice("Loading MySQL schema");
            using (var conn = new MySqlConnection(m_ConnectionString))
            {
                
                conn.Open();
                var cmd = new MySqlCommand("SHOW TABLES", conn);
                var tableList = new List<string>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; ++i)
                        {
                            Tracer.I.Notice("table {0}", reader[i]);
                            tableList.Add(reader[i].ToString());
                        }
                    }
                }

                foreach (var t in tableList)
                {
                    Tracer.I.Notice("Loading schema for {0}", t);
                    var newTable = m_Mirror.New();
                    newTable.Name = t;
                    newTable.IsMysql = true;
                    using (var reader = (new MySqlCommand("DESCRIBE " + t, conn)).ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var field = new SqlFieldMirror()
                            {
                                IsMysql = true,
                                TableName = newTable.Name
                            };

                            for (int i = 0; i < reader.FieldCount; ++i)
                            {
                                var fName = reader.GetName(i);
                                var fVal = reader[i] != null ? reader[i].ToString().ToLower().Trim() : "";
                                //Tracer.I.Notice("{0}: {2}", fName, i, fVal);

                                

                                switch (fName.ToLower())
                                {
                                    case "field":
                                        field.Name = fVal;
                                        break;
                                    case "type":
                                        MySqlFieldMapper.Map(fVal, field);
                                        break;
                                    case "null":
                                        if (fVal == "yes")
                                        {
                                            field.DbIsNullable = "YES";
                                        }
                                        else
                                        {
                                            field.DbIsNullable = "NO";
                                        }
                                        break;
                                    case "extra":
                                        field.DbExtra = fVal;
                                        break;
                                    case "default":
                                        field.DbDefault = fVal;
                                        break;
                                    case "key":
                                        if (fVal == "pri")
                                        {
                                            field.DbIsPrimary = true;
                                        }
                                        break;
                                }
                            }

                            newTable.Fields.Add(field);
                        }
                    }

                }


                /*m_Schema = conn.GetSchema(System.Data.SqlClient.
                                              SqlClientMetaDataCollectionNames.Tables);
                var tableSchema = conn.GetSchema(System.Data.SqlClient.SqlClientMetaDataCollectionNames.Columns);
                var columnList = new MultiValueDictionary<string, DataRow>();
                for (int i = 0; i < tableSchema.Rows.Count; ++i)
                {
                    var row = tableSchema.Rows[i];
                    columnList.Add(row["table_name"].ToString(), row);
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

                }*/
                conn.Close();
            }
        }

        public override void LoadStoredProcs()
        {
            // do nothing
            // todo
        }
    }
}