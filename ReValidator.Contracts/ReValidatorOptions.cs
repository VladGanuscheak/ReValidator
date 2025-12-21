using System;
using System.Collections.Generic;

namespace ReValidator.Contracts
{
    public sealed class ReValidatorOptions
    {
        public HashSet<Type> RegisteredTypes { get; } = new HashSet<Type>();

        public void RegisterType<T>()
            => RegisteredTypes.Add(typeof(T));
    }
}
