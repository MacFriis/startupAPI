using System;
namespace StartupApi.Model
{
    public class Collection<T> : Resource
    {
        public T[] Value { get; set; }
    }
}
