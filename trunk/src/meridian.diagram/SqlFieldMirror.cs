namespace meridian.diagram
{
    public class SqlFieldMirror
    {
        public string TableName { get; set;  }
        public string DbIsNullable { get; set; }
        public string DbExtra { get; set; }
        public bool DbIsPrimary { get; set; }
        public string DbDefault { get; set; }
        public string Name { get; set; }
        public string DbType { get; set; }
        public int DbLength { get; set; }
        public bool IsMysql { get; set; }

        public SqlFieldMirror()
        {
            IsMysql = false;
        }

        private FieldType? m_FieldType;

        public FieldType LocalType
        {
            get
            {
                if (m_FieldType == null)
                {
                    m_FieldType = SqlFieldMapper.Map(DbType.ToLower().Trim(), DbLength);
                }
                return m_FieldType.Value;
            }
            set { m_FieldType = value; }
        }
    }
}