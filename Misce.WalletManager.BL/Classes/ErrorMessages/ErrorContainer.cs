namespace Misce.WalletManager.BL.Classes.ErrorMessages
{
    public class ErrorContainer
    {
        public IEnumerable<Error> Errors { get; set; }

        public ErrorContainer()
        {
            Errors = new List<Error>(0);
        }

        public ErrorContainer(IEnumerable<Error> errors)
        {
            Errors = errors;
        }

        public ErrorContainer(string field, string message)
        {
            Errors = new List<Error> { new Error(field, message) };
        }
    }
}
