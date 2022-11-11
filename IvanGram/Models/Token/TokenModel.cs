namespace IvanGram.Models.Token
{
    public class TokenModel
    {
        public string AcessToken { get; set; }
        public string RefershToken { get; set; }

        public TokenModel(string accessToken, string refreshToken)
        {
            AcessToken = accessToken;
            RefershToken = refreshToken;
        }
    }
}
