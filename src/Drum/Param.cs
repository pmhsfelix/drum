namespace Drum
{
    public static class Param<TValue>
    {
        public static TValue Any
        {
            get { return default(TValue); }
        }
    }
}