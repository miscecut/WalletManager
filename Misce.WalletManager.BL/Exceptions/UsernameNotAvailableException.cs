namespace Misce.WalletManager.BL.Exceptions
{
    [Serializable]
    public class UsernameNotAvailableException : Exception
    {
        public UsernameNotAvailableException() { }

        public UsernameNotAvailableException(string message) : base(message) { }
    }
}
