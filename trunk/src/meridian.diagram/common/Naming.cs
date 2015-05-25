namespace meridian.diagram
{
    public static class Naming
    {
        /// <summary>
        /// Converts a literal name, e.g. "string" to enumeration
        /// </summary>
        /// <param name="_name"></param>
        /// <returns></returns>
        public static FieldType CastFieldType(string _name)
        {
            return FieldType.ftString;
        }
    }
}