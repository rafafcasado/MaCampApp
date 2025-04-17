using System.Text;
using MaCamp.Utils;

namespace MaCamp.Services
{
    public class WebService
    {
        private string PAGE_K { get; set; }
        private string LIMIT_K { get; set; }
        private string TAG_K { get; set; }
        private string QUERY_K { get; set; }
        private string NIGHTVIEW_K { get; set; }

        // Page e Limit precisam possuir um valor padrão (não podem ser enviados sem valor)
        private int PAGE_V { get; set; }
        private int LIMIT_V { get; set; }
        private string TAG_V { get; set; }
        private string QUERY_V { get; set; }
        private string NIGHTVIEW_V { get; set; }

        public WebService()
        {
            PAGE_K = "page";
            LIMIT_K = "limit";
            TAG_K = "tag";
            QUERY_K = "q";
            NIGHTVIEW_K = "nightview";

            PAGE_V = 1;
            LIMIT_V = AppConstants.QuantidadeNoticiasPorLote;
            TAG_V = string.Empty;
            QUERY_V = string.Empty;
            NIGHTVIEW_V = string.Empty;
        }

        public async Task<List<T>> GetListAsync<T>(string endpoint, int page, string? tag = null, string? parametrosBusca = null) where T : class
        {
            PAGE_V = page;
            //Só envia o parâmetro TAG se o usuário tiver selecionado alguma efetivamente
            TAG_V = string.IsNullOrWhiteSpace(tag) || tag == AppConstants.ParametroTodasTags ? string.Empty : tag;
            QUERY_V = parametrosBusca ?? string.Empty;

            var builder = new StringBuilder(endpoint);

            builder.Append(endpoint.Contains("?") ? "&" : "?");
            builder.Append(PAGE_K);
            builder.Append("=" + PAGE_V);
            builder.Append("&" + LIMIT_K);
            builder.Append("=" + LIMIT_V);
            builder.Append("&" + TAG_K);
            builder.Append("=" + TAG_V);
            builder.Append("&" + QUERY_K);
            builder.Append("=" + QUERY_V);

            var url = builder.ToString();
            var data = await AppNet.GetListAsync<T>(url);

            return data;
        }
    }
}