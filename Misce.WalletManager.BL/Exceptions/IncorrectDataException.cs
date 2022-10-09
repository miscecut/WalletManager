namespace Misce.WalletManager.BL.Exceptions
{
    [Serializable]
    public class IncorrectDataException : Exception
    {
        public IncorrectDataException() { }

        public IncorrectDataException(string message) : base(message) { }
    }
}
