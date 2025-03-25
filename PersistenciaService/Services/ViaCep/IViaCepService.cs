namespace PersistenciaService.Services.ViaCep
{
    public interface IViaCepService
    {
        Task<ViaCepResponse?> ConsultarCepAsync(string cep);
    }
}
