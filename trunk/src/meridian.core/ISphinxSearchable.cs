using System.Text;

namespace meridian.core
{
    public interface ISphinxSearchable
    {
        long GetId();
        long GetGroupId();
        long GetTimestamp();
        string GetText();
        string GetTitle();
        string GetObjectType();
        string GetUrl();
        void BuildText(StringBuilder _builder);
    }
}