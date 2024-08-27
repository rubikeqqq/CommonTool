using System;

namespace CommonTool.Core
{
    public class SingleTon<T> where T : class
    {
        private static readonly Lazy<T> Lazy = new Lazy<T>(Activator.CreateInstance<T>);

        public static T Instance
        {
            get { return Lazy.Value; }
        }
    }
}