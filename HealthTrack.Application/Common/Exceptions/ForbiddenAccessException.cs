namespace HealthTrack.Application.Common.Exceptions
{
    public sealed class ForbiddenAccessException : Exception
    {
        public ForbiddenAccessException(string message) : base(message)
        {
        }

        public ForbiddenAccessException() : base("You do not have access to this resource.")
        {
        }
    }
}
