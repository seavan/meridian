using System;
using System.Collections.Generic;
using meridian.core;

namespace meridian.diagram
{
    public static class CSharpFieldMapper
    {
        static CSharpFieldMapper()
        {
            m_Mapper[FieldType.ftByte] = "byte";
            m_Mapper[FieldType.ftFloat] = "double";
            
            m_Mapper[FieldType.ftString] = "string";
            m_Mapper[FieldType.ftInt16] = "short";
            m_Mapper[FieldType.ftInt] = "int";
            m_Mapper[FieldType.ftUInt] = "uint";
            m_Mapper[FieldType.ftLong] = "long";
            m_Mapper[FieldType.ftDateTime] = "DateTime";
            m_Mapper[FieldType.ftChar] = "string";
            m_Mapper[FieldType.ftBool] = "bool";
            m_Mapper[FieldType.ftGuid] = "System.Guid";
            m_Mapper[FieldType.ftDecimal] = "System.Decimal";
            m_Mapper[FieldType.ftTimeStamp] = "byte[]";
            m_Mapper[FieldType.ftBinary] = "byte[]";

            m_Mapper2[FieldType.ftBinary] = "new byte[] {}";
            m_Mapper2[FieldType.ftByte] = "(byte)0";
            m_Mapper2[FieldType.ftString] = "\"\"";
            m_Mapper2[FieldType.ftFloat] = "(double)0";
            m_Mapper2[FieldType.ftInt16] = "(short)0";
            m_Mapper2[FieldType.ftInt] = "0";
            m_Mapper2[FieldType.ftUInt] = "0";
            m_Mapper2[FieldType.ftLong] = "0";
            m_Mapper2[FieldType.ftDateTime] = "DateTime.MinValue";
            m_Mapper2[FieldType.ftDecimal] = "new Decimal()";
            m_Mapper2[FieldType.ftChar] = "\"\"";
            m_Mapper2[FieldType.ftBool] = "false";
            m_Mapper2[FieldType.ftGuid] = "new System.Guid()";
            m_Mapper2[FieldType.ftTimeStamp] = "new byte[] {}";
            
        }

        public static string ToCSharp(this FieldType _ft)
        {
            return Map(_ft);
        }

        public static bool IsNullableFieldType(this FieldType _ft)
        {
            return _ft.Equals(FieldType.ftString);
        }

        public static string DefaultCSharp(this FieldType _ft)
        {
            return Map2(_ft);
        }

        public static string DataReaderConverter(this FieldType _ft, string _readerVarName, FieldDescription _field, string _dbType)
        {
            if (_dbType == "MySql")
            {
                return MySqlFieldMapper.GetNullChecker(_readerVarName, _field);
            }
            else
            {
                return String.Format(
                    "_reader[\"{0}\"].GetType() != typeof(System.DBNull) ? ({1})_reader[\"{0}\"] : {2}", _field.Backend,
                    _field.FieldType.ToCSharp(), _field.FieldType.DefaultCSharp());
            }
            
        }

        public static string Map(FieldType _type)
        {
            if (m_Mapper.IndexOfKey(_type) == -1)
            {
                Tracer.I.Error("SQL type not found {0}", _type);
                return "void";
            }
            return m_Mapper[_type];
        }

        public static string Map2(FieldType _type)
        {
            if (m_Mapper2.IndexOfKey(_type) == -1)
            {
                Tracer.I.Error("SQL type not found {0}", _type);
                return "void";
            }
            return m_Mapper2[_type];
        }

        private static SortedList<FieldType, string> m_Mapper = new SortedList<FieldType, string>();
        private static SortedList<FieldType, string> m_Mapper2 = new SortedList<FieldType, string>();
    }

    public static class SqlFieldMapper
    {
        static SqlFieldMapper()
        {
            m_Mapper["nvarchar"] = FieldType.ftString;
            m_Mapper["text"] = FieldType.ftString;
            m_Mapper["ntext"] = FieldType.ftString;
            m_Mapper["varchar"] = FieldType.ftString;
            m_Mapper["smallint"] = FieldType.ftInt16;
            m_Mapper["tinyint"] = FieldType.ftByte;
            m_Mapper["int"] = FieldType.ftInt;
            m_Mapper["short"] = FieldType.ftInt;
            m_Mapper["byte"] = FieldType.ftInt;
            m_Mapper["bigint"] = FieldType.ftLong;
            m_Mapper["long"] = FieldType.ftLong;
            m_Mapper["float"] = FieldType.ftFloat;
            m_Mapper["date"] = FieldType.ftDateTime;
            m_Mapper["smalldatetime"] = FieldType.ftDateTime;
            m_Mapper["datetime"] = FieldType.ftDateTime;
            m_Mapper["datetime2"] = FieldType.ftDateTime;
            m_Mapper["char"] = FieldType.ftChar;
            m_Mapper["nchar"] = FieldType.ftChar;
            m_Mapper["bit"] = FieldType.ftBool;
            m_Mapper["uniqueidentifier"] = FieldType.ftGuid;
            m_Mapper["money"] = FieldType.ftDecimal;
            m_Mapper["timestamp"] = FieldType.ftTimeStamp;
            m_Mapper["binary"] = FieldType.ftBinary;
        }

        /*public static FieldType FromSql(this FieldType _ft, string _src)
        {
            return Map(_src);
        }*/

        public static FieldType Map(string _type, int _dbLength)
        {
            if (m_Mapper.IndexOfKey(_type) == -1)
            {
                Tracer.I.Error("SQL type not found {0}", _type);
                throw new Exception("SQL type not found");
                return FieldType.ftString;
            }
            var t = m_Mapper[_type];

            if ((t == FieldType.ftChar) && (_dbLength > 1))
            {
                t = FieldType.ftString;
            }

            return t;
        }

        private static SortedList<string, FieldType> m_Mapper = new SortedList<string, FieldType>();
    }
}