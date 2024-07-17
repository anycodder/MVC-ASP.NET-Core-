namespace WebApplicationTRY_TWO.Models
{
    public class CommentViewModel
    {
        public int id { get; set; }
        public string comment_text { get; set; }
        public DateTime? comment_date { get; set; }
        public string comment_type { get; set; }
        public int comment_score { get; set; }
        public int? product_id { get; set; }
        public string product_name { get; set; } // İlişkili ürün adı
        public int? user_id { get; set; }
        public string user_name { get; set; } // Yorumu yapan kullanıcının adı
        public int? answer_id { get; set; }
    }
}