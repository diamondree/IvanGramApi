namespace IvanGram.Exeption
{
    public class BlackListException : System.Exception
    {
        public override string Message => "You cant do this, you are in black list";
    }
}
