using System;

namespace SimpleRulesEngine.Exceptions
{
    public class SimpleRulesEngineException : Exception
    {
        public SimpleRulesEngineException(string message)
            : base(message)
        {
            // no-op
        }

        public SimpleRulesEngineException(string message, Exception innerException)
            : base(message, innerException)
        {
            // no-op
        }
    }
}
