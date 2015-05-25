using System;
using System.Collections.Generic;
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
            m_Mapper["short"] = FieldType.ftInt;
            m_Mapper["byte"] = FieldType.ftInt;
            m_Mapper["bigint"] = FieldType.ftLong;
            m_Mapper["long"] = FieldType.ftLong;
            m_Mapper["float"] = FieldType.ftFloat;
            m_Mapper["double"] = FieldType.ftFloat;
            m_Mapper["date"] = FieldType.ftDateTime;
            m_Mapper["datetime"] = FieldType.ftDateTime;
            m_Mapper["char"] = FieldType.ftChar;
            m_Mapper["nchar"] = FieldType.ftChar;
            m_Mapper["bit"] = FieldType.ftBool;
            m_Mapper["uniqueidentifier"] = FieldType.ftGuid;
        }

        /*public static FieldType FromSql(this FieldType _ft, string _src)
        {
            return Map(_src);
        }*/

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
                _dbLength = int.Parse(vals[1]);
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
    }
}