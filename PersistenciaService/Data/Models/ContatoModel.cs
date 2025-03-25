namespace PersistenciaService.Data.Models
{
    public class ContatoModel
    {
        public int CTT_ID { get; set; }
        public DateTime CTT_DTCRIACAO { get; set; } = DateTime.Now;
        public string CTT_NOME { get; set; } = string.Empty;
        public string CTT_EMAIL { get; set; } = string.Empty;
        public int CTT_DDD { get; set; }
        public string CTT_NUMERO { get; set; } = string.Empty;
    }
}
