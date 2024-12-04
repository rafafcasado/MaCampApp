using MaCamp.CustomControls;

namespace MaCamp.Handlers
{
    public partial class IconViewHandler
    {
        private static IPropertyMapper<IconView, IconViewHandler> Mapper => new PropertyMapper<IconView, IconViewHandler>(ViewMapper);

        public IconViewHandler() : base(Mapper)
        {
        }

        public partial void MapSource(IconViewHandler handler, IconView iconView);
    }
}