using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using meridian.core;

namespace meridian.diagram
{
    public static class MySqlFieldMapper
    {
        static MySqlFieldMapper()
        {
            m_Mapper["text"] = FieldType.ftString;
            m_Mapper["enum"] = FieldType.ftString;
            m_Mapper["longtext"] = FieldType.ftString;
            m_Mapper["mediumtext"] = FieldType.ftString;
            m_Mapper["tinytext"] = FieldType.ftString;
            m_Mapper["varchar"] = FieldType.ftString;
            m_Mapper["int"] = FieldType.ftInt;
            m_Mapper["int(11) unsigned"] = FieldType.ftUInt;
            m_Mapper["mediumint"] = FieldType.ftInt;
            m_Mapper["tinyint"] = FieldType.ftByte;
            m_Mapper["smallint"] = FieldType.ftInt;
            m_Mapper["short"] = FieldType.ftInt;
            m_Mapper["byte"] = FieldType.ftInt;
            m_Mapper["bigint"] = FieldType.ftLong;
            m_Mapper["long"] = FieldType.ftLong;
            m_Mapper["float"] = FieldType.ftFloat;
            m_Mapper["double"] = FieldType.ftFloat;
            m_Mapper["date"] = FieldType.ftDateTime;
            m_Mapper["datetime"] = FieldType.ftDateTime;
            m_Mapper["decimal"] = FieldType.ftDecimal;
            m_Mapper["char"] = FieldType.ftChar;
            m_Mapper["nchar"] = FieldType.ftChar;
            m_Mapper["bit"] = FieldType.ftBool;
            m_Mapper["binary"] = FieldType.ftBinary;
            m_Mapper["uniqueidentifier"] = FieldType.ftGuid;


            m_ConverterMapper[FieldType.ftBinary] = "{0}.GetBinary(\"{1}\")";
            m_ConverterMapper[FieldType.ftDecimal] = "{0}.GetDecimal(\"{1}\")";
            m_ConverterMapper[FieldType.ftString] = "{0}.GetString(\"{1}\")";
            m_ConverterMapper[FieldType.ftInt] = "{0}.GetInt32(\"{1}\")";
            m_ConverterMapper[FieldType.ftUInt] = "{0}.GetUInt32(\"{1}\")";
            m_ConverterMapper[FieldType.ftByte] = "{0}.GetByte(\"{1}\")";
            m_ConverterMapper[FieldType.ftLong] = "{0}.GetInt64(\"{1}\")";
            m_ConverterMapper[FieldType.ftFloat] = "{0}.GetFloat(\"{1}\")";
            m_ConverterMapper[FieldType.ftDateTime] = "{0}.GetDateTime(\"{1}\")";
            m_ConverterMapper[FieldType.ftChar] = "{0}.GetChar(\"{1}\")";
            m_ConverterMapper[FieldType.ftBool] = "{0}.GetBoolean(\"{1}\")";
            m_ConverterMapper[FieldType.ftGuid] = "!String.IsNullOrEmpty({0}[\"{1}\"].ToString()) ? {0}.GetGuid(\"{1}\") : " + FieldType.ftGuid.DefaultCSharp();
        }

        /*public static FieldType FromSql(this FieldType _ft, string _src)
        {
            return Map(_src);
        }*/

        public static string GetNullChecker(string _readerVar, FieldDescription _desc)
        {
            if (m_ConverterMapper.IndexOfKey(_desc.FieldType) == -1)
            {
                throw new Exception(String.Format("Type not found {0}, {1}, {2}", _desc.Name, _desc.FieldType, _desc.Backend));
            }
            return String.Format("{0}[\"{1}\"].GetType() != typeof(System.DBNull) ? {2} : {3}", 
                _readerVar,
                _desc.Backend,
                String.Format(m_ConverterMapper[_desc.FieldType], _readerVar, _desc.Backend),
                _desc.FieldType.DefaultCSharp());
        }

        public static void Map(string _input, SqlFieldMirror _mirror)
        {
            string _type = "";
            int _dbLength = 0 ;

            var vals = _input.Split('(', ')');

            if (vals.Length > 0)
            {
                _type = vals[0];
            }

            if ( (_type == "enum") || (_type == "float"))
            {
                
            }
            else
            if (vals.Length > 1)
            {
                var subParams = vals[1].Split(',');
                _dbLength = int.Parse(subParams[0]);
            }

            if (m_Mapper.IndexOfKey(_type) == -1)
            {
                Tracer.I.Error("SQL type not found {0}", _type);
                _mirror.LocalType = FieldType.ftString;
            }
            var t = m_Mapper[_type];

            if ((t == FieldType.ftChar) && (_dbLength > 1))
            {
                t = FieldType.ftString;
            }

            if ((t == FieldType.ftString) && (_dbLength == 36))
            {
                t = FieldType.ftGuid;
            }


            if ((t == FieldType.ftByte))
            {

                // todo boolean bits
                if (_dbLength == 1)
                {
                    t = FieldType.ftBool;
                }
                    /*
                else
                    if (_dbLength <= 20)
                    {
                        t = FieldType.ftInt;
                    }
                    else
                    {
                        t = FieldType.ftLong;
                    }*/
            }



            if (_input.IndexOf("unsigned") != -1)
            {
                switch (t)
                {
                    case FieldType.ftInt:
                        t = FieldType.ftUInt;
                        break;
                    default:
                        break;
                }
            }
            _mirror.DbType = _input;
            _mirror.DbLength = _dbLength;
            _mirror.LocalType = t;

        }

        private static SortedList<string, FieldType> m_Mapper = new SortedList<string, FieldType>();
        private static SortedList<FieldType, string> m_ConverterMapper = new SortedList<FieldType, string>();
    }
}