using System.Collections.Generic;

namespace meridian.diagram
{
    public enum ElementType
    {
        None,
        Proto,
        Field,
        Inherit,
        Aggregation,
        Composition,
        InlineComposition,
        Association,
        Entity,
        View,
        Set,
        Foreign,
        Primary,
        StoredProcedure
    }

    public static class ElementTypeMapper
    {
        static ElementTypeMapper()
        {
            m_Mapper["NONE"] = ElementType.None;
            m_Mapper["PROTO"] = ElementType.Proto;
            m_Mapper["FIELD"] = ElementType.Field;
            m_Mapper["INHERIT"] = ElementType.Inherit;
            m_Mapper["AGGREGATION"] = ElementType.Aggregation;
            m_Mapper["COMPOSITION"] = ElementType.Composition;
            m_Mapper["INLINECOMPOSITION"] = ElementType.InlineComposition;
            m_Mapper["ASSOCIATION"] = ElementType.Association;
            m_Mapper["ENTITY"] = ElementType.Entity;
            m_Mapper["VIEW"] = ElementType.View;
            m_Mapper["SET"] = ElementType.Set;
            m_Mapper["FOREIGN"] = ElementType.Foreign;
            m_Mapper["PRIMARY"] = ElementType.Primary;
        }

        public static bool IsType(string _type)
        {
            return m_Mapper.IndexOfKey(_type.Trim().ToString().ToUpper()) != -1;
        }

        public static ElementType Map(string _type)
        {
            return m_Mapper[_type.Trim().ToString().ToUpper()];
        }

        private static SortedList<string, ElementType> m_Mapper = new SortedList<string,ElementType>();
    }
}