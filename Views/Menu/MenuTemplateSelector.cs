using MaCamp.Views.CustomCells;

namespace MaCamp.Views.Menu
{
    public class MenuTemplateSelector : DataTemplateSelector
    {
        private DataTemplate MasterPageItemTemplate { get; }
        private DataTemplate MasterPageSubItemTemplate { get; }
        private DataTemplate DivisoriaTemplate { get; }

        public MenuTemplateSelector()
        {
            // Retain instances!
            MasterPageItemTemplate = new DataTemplate(typeof(MasterPageItemViewCell));
            MasterPageSubItemTemplate = new DataTemplate(typeof(MasterPageSubItemViewCell));
            DivisoriaTemplate = new DataTemplate(typeof(DivisoriaMenuViewCell));
        }

        protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
        {
            if (item is ItemMenu itemMenu)
            {
                switch (itemMenu.TipoLayout)
                {
                    case TipoLayoutMenu.Item:
                        return MasterPageItemTemplate;
                    case TipoLayoutMenu.SubItem:
                        return MasterPageSubItemTemplate;
                    case TipoLayoutMenu.Divisoria:
                        return DivisoriaTemplate;
                    default:
                        return MasterPageItemTemplate;
                }
            }

            return default;
        }
    }
}