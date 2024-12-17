using MaCamp.Models;

namespace MaCamp.Views.CustomCells
{
    public class ItemDataTemplateSelector : DataTemplateSelector
    {
        private DataTemplate ItemTemplate { get; }
        private DataTemplate CampingTemplate { get; }
        private DataTemplate AnuncioTemplate { get; }
        private DataTemplate AdMobRectangleTemplate { get; }
        private DataTemplate AnuncioCardTemplate { get; }

        public ItemDataTemplateSelector()
        {
            ItemTemplate = new DataTemplate(typeof(ItemViewCell));
            CampingTemplate = new DataTemplate(typeof(CampingViewCell));
            AnuncioTemplate = new DataTemplate(typeof(AnuncioViewCell));
            AdMobRectangleTemplate = new DataTemplate(typeof(AdMobRectangleViewCell));
            AnuncioCardTemplate = new DataTemplate(typeof(AnuncioCardViewCell));
        }

        protected override DataTemplate OnSelectTemplate(object itemAtual, BindableObject container)
        {
            if (itemAtual is Item item)
            {
                if (item.EhAnuncio)
                {
                    return AnuncioCardTemplate;
                }

                if (item.IdCamping != 0)
                {
                    return CampingTemplate;
                }

                if (item.EhAdMobRetangulo)
                {
                    return AdMobRectangleTemplate;
                }
            }

            return ItemTemplate;
        }
    }
}