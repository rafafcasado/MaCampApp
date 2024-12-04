using Aspbrasil.AppSettings;
using Aspbrasil.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspbrasil.DataAccess
{
    class WebService<T> where T : class
    {
        private const string PAGE_K = "page";
        private const string LIMIT_K = "limit";
        private const string TAG_K = "tag";
        private const string QUERY_K = "q";
        private const string NIGHTVIEW_K = "nightview";
        // Page e Limit precisam possuir um valor padrão (não podem ser enviados sem valor)
        private int PAGE_V = 1;
        private const int LIMIT_V = AppConstants.QUANTIDADE_NOTICIAS_POR_LOTE;
        private string TAG_V = "";
        private string QUERY_V = "";
        private string NIGHTVIEW_V = "";

        public async Task<List<T>> Get(string endPoint, int pagina, string tag = "", string parametrosBusca = "")
        {
            PAGE_V = pagina;
            //Só envia o parâmetro TAG se o usuário tiver selecionado alguma efetivamente
            TAG_V = (string.IsNullOrWhiteSpace(tag) || tag == AppConstants.PARAMETRO_TODAS_TAGS) ? string.Empty : tag;
            QUERY_V = parametrosBusca ?? string.Empty;

            StringBuilder builder = new StringBuilder(endPoint);
            if (!endPoint.Contains("?")) { builder.Append("?"); }
            else { builder.Append("&"); }

            builder.Append(PAGE_K);
            builder.Append("=" + PAGE_V);
            builder.Append("&" + LIMIT_K);
            builder.Append("=" + LIMIT_V);
            builder.Append("&" + TAG_K);
            builder.Append("=" + TAG_V);
            builder.Append("&" + QUERY_K);
            builder.Append("=" + QUERY_V);

            string url = builder.ToString();
            string jsonItens = await NetUtils.GetString(url);
            IEnumerable<T> itens = JsonConvert.DeserializeObject<IEnumerable<T>>(jsonItens);
            return itens == null ? new List<T>() : itens.ToList();
        }
    }
}
