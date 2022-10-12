namespace Misce.WalletManager.BL.Classes.ErrorMessages
{
    public class ErrorContainer
    {
        public IEnumerable<Error> Errors { get; init; }

        public ErrorContainer(IEnumerable<Error> errors)
        {
            Errors = errors;
        }
    }
}
