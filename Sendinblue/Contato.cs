namespace Sendinblue
{
    public class Contato
    {
        public bool Ativo { get; set; }
        public string NOME { get; set; }
        public string DtNascimento { get; set; }
        public string Email { get; set; }
        public int Id { get; set; }
        public bool EmailBlacklisted { get; set; }
        public bool SmsBlacklisted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public List<int> ListIds { get; set; }
        public Dictionary<string, object> Attributes { get; set; }
    }
}