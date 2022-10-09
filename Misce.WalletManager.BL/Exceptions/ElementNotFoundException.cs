namespace Misce.WalletManager.BL.Exceptions
{
    [Serializable]
    public class ElementNotFoundException : Exception
    {
        public ElementNotFoundException() { }

        public ElementNotFoundException(string message) : base(message) { }
    }
}
