namespace ExceptionHandling
{
    public class AuthorizationException : Exception
    {
        public IEnumerable<string> Errors { get; }

        public AuthorizationException(IEnumerable<string> errors)
            : base("Authorization error occurred.")
        {
            Errors = errors;
        }

        public override string ToString()
        {
            return $"{Message}: {string.Join("; ", Errors)}";
        }
    }
}
