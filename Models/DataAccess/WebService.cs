using System.Text;
using MaCamp.AppSettings;
using Newtonsoft.Json;

// ReSharper disable InconsistentNaming
namespace MaCamp.Models.DataAccess
{
    internal class WebService<T> where T : class
    {
        private const string PAGE_K = "page";
        private const string LIMIT_K = "limit";
        private const string TAG_K = "tag";
        private const string QUERY_K = "q";
        //private const string NIGHTVIEW_K = "nightview";

        // Page e Limit precisam possuir um valor padrão (não podem ser enviados sem valor)
        private int PAGE_V = 1;
        private static int LIMIT_V => AppConstants.QuantidadeNoticiasPorLote;
        private string TAG_V = "";
        private string QUERY_V = "";
        //private string NIGHTVIEW_V = "";

        public async Task<List<T>> Get(string endPoint, int pagina, string tag = "", string? parametrosBusca = "")
        {
            PAGE_V = pagina;
            //Só envia o parâmetro TAG se o usuário tiver selecionado alguma efetivamente
            TAG_V = string.IsNullOrWhiteSpace(tag) || tag == AppConstants.ParametroTodasTags ? string.Empty : tag;
            QUERY_V = parametrosBusca ?? string.Empty;

            var builder = new StringBuilder(endPoint);

            builder.Append(!endPoint.Contains("?") ? "?" : "&");
            builder.Append(PAGE_K);
            builder.Append("=" + PAGE_V);
            builder.Append("&" + LIMIT_K);
            builder.Append("=" + LIMIT_V);
            builder.Append("&" + TAG_K);
            builder.Append("=" + TAG_V);
            builder.Append("&" + QUERY_K);
            builder.Append("=" + QUERY_V);

            var url = builder.ToString();
            var jsonItens = await NetUtils.GetString(url);
            var itens = JsonConvert.DeserializeObject<IEnumerable<T>>(jsonItens);

            return itens == null ? new List<T>() : itens.ToList();
        }
    }
}