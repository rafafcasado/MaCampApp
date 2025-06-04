using MaCamp.Services;
using MaCamp.Utils;

namespace MaCamp.ViewModels
{
    public class FiltrosViewModel
    {
        public async Task<List<string>> ObterFiltrosEstabelecimentoAsync()
        {
            var estabelecimentos = await DBContract.GetKeyValueAsync(AppConstants.Filtro_EstabelecimentoSelecionados);

            if (estabelecimentos != null)
            {
                return estabelecimentos.Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList();

            }

            return new List<string>
            {
                "Campings",
                "PontodeApoioaRV`s",
                "CampingSelvagem/WildCamping/Bushcfaft",
                "SemFunçãoCamping/ApoioouFechado"
            };
        }

        public async Task<List<string>> ObterFiltrosServicosAsync()
        {
            var servicos = await DBContract.GetKeyValueAsync(AppConstants.Filtro_ServicoSelecionados);

            if (servicos != null)
            {
                return servicos.Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList();
            }

            return new List<string>();
        }

        public async Task SalvarFiltrosAsync(List<string> estabelecimentos, List<string> comodidades)
        {
            var valorEstabelecimentos = string.Join(",", estabelecimentos);
            var valorComodidades = string.Join(",", comodidades);

            await DBContract.UpdateKeyValue(AppConstants.Filtro_EstabelecimentoSelecionados, valorEstabelecimentos);
            await DBContract.UpdateKeyValue(AppConstants.Filtro_ServicoSelecionados, valorComodidades);
        }

        public void AlternarEstabelecimento(List<string> estabelecimentos, string filtro)
        {
            if (estabelecimentos.Contains(filtro))
            {
                estabelecimentos.Remove(filtro);
            }
            else
            {
                estabelecimentos.Add(filtro);
            }
        }

        public void AlternarServico(List<string> comodidades, string filtro)
        {
            if (comodidades.Contains(filtro))
            {
                comodidades.Remove(filtro);
            }
            else
            {
                comodidades.Add(filtro);
            }
        }
    }

}
