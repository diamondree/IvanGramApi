namespace IvanGram.Exeptions
{
    public class NotFoundException : System.Exception
    {
        public string? Model { get; set; }
        public override string Message => $"{Model} not found";
    }

    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException()
        {
            Model = "User";
        }
    }
    
    public class PostNotFoundException : NotFoundException
    {
        public PostNotFoundException()
        {
            Model = "Post";
        }
    }

    public class PostCommentsNotFoundException : NotFoundException
    {
        public PostCommentsNotFoundException()
        {
            Model = "Post comments";
        }
    }

    public class FileNotFoundException : NotFoundException
    { 
        public FileNotFoundException()
        {
            Model = "File";
        }
    }

    public class SessionNotFoundException : NotFoundException
    { 
        public SessionNotFoundException()
        {
            Model = "Session";
        }
    }
}
