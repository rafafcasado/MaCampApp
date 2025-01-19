using MaCamp.Models;

namespace MaCamp.Views.CustomViews
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
            ItemTemplate = new DataTemplate(typeof(ItemContentView));
            CampingTemplate = new DataTemplate(typeof(CampingContentView));
            AnuncioTemplate = new DataTemplate(typeof(AnuncioContentView));
            AdMobRectangleTemplate = new DataTemplate(typeof(AdMobRectangleContentView));
            AnuncioCardTemplate = new DataTemplate(typeof(AnuncioCardContentView));
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