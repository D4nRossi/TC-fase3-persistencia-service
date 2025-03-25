namespace PersistenciaService.Data.Models
{
    public class MunicipioModel
    {
        public int MNC_ID { get; set; }
        public string MNC_NOME { get; set; } = string.Empty;
        public int MNC_UF { get; set; }
        public int MNC_DDD { get; set; }
        public int MNC_IBGE { get; set; }
    }
}
