using MaCamp.Services;
using MaCamp.Utils;

namespace MaCamp.ViewModels
{
    public class FiltrosViewModel
    {
        public List<string> ObterFiltrosEstabelecimento()
        {
            var estabelecimentos = DBContract.GetKeyValue(AppConstants.Filtro_EstabelecimentoSelecionados);

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

        public List<string> ObterFiltrosServicos()
        {
            var servicos = DBContract.GetKeyValue(AppConstants.Filtro_ServicoSelecionados);

            if (servicos != null)
            {
                return servicos.Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList();
            }

            return new List<string>();
        }

        public void SalvarFiltros(List<string> estabelecimentos, List<string> comodidades)
        {
            var valorEstabelecimentos = string.Join(",", estabelecimentos);
            var valorComodidades = string.Join(",", comodidades);

            DBContract.UpdateKeyValue(AppConstants.Filtro_EstabelecimentoSelecionados, valorEstabelecimentos);
            DBContract.UpdateKeyValue(AppConstants.Filtro_ServicoSelecionados, valorComodidades);
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
