using MaCamp.Utils;
using MaCamp.Views.CustomViews;

namespace MaCamp.Views.Menu
{
    public class MenuTemplateSelector : DataTemplateSelector
    {
        private DataTemplate MasterPageItemTemplate { get; }
        private DataTemplate MasterPageSubItemTemplate { get; }
        private DataTemplate DivisoriaTemplate { get; }

        public MenuTemplateSelector()
        {
            MasterPageItemTemplate = new DataTemplate(typeof(MasterPageItemContentView));
            MasterPageSubItemTemplate = new DataTemplate(typeof(MasterPageSubItemContentView));
            DivisoriaTemplate = new DataTemplate(typeof(DivisoriaMenuContentView));
        }

        protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
        {
            if (item is ItemMenu itemMenu)
            {
                switch (itemMenu.TipoLayout)
                {
                    case Enumeradores.TipoLayoutMenu.Item:
                        return MasterPageItemTemplate;
                    case Enumeradores.TipoLayoutMenu.SubItem:
                        return MasterPageSubItemTemplate;
                    case Enumeradores.TipoLayoutMenu.Divisoria:
                        return DivisoriaTemplate;
                    default:
                        return MasterPageItemTemplate;
                }
            }

            return default;
        }
    }
}