using Aspbrasil.Models;
using Xamarin.Forms;

namespace Aspbrasil.Views.CustomCells
{
    public class ItemDataTemplateSelector : DataTemplateSelector
    {
        public static DataTemplate ItemTemplate = new DataTemplate(typeof(ItemViewCell));
        public static DataTemplate CampingTemplate = new DataTemplate(typeof(CampingViewCell));
        public static DataTemplate AnuncioTemplate = new DataTemplate(typeof(AnuncioViewCell));
        public static DataTemplate AdMobRectangleTemplate = new DataTemplate(typeof(AdMobRectangleViewCell));
        public static DataTemplate AnuncioCardTemplate = new DataTemplate(typeof(AnuncioCardViewCell));

        protected override DataTemplate OnSelectTemplate(object itemAtual, BindableObject container)
        {
            Item item = (Item)itemAtual;
            if (item.EhAnuncio)
            {
                return AnuncioCardTemplate;
            }
            else if (item.IdCamping != 0)
            {
                return CampingTemplate;
            }
            else if (item.EhAdMobRetangulo)
            {
                return AdMobRectangleTemplate;
            }
            return ItemTemplate;
        }
    }
}