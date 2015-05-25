namespace meridian.core
{
    public interface IFeeder
    {
        bool Eof();
        string Next();
    }
}