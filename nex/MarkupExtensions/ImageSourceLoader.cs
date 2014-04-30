using System;
using System.Windows.Markup;
using nex.Utilities;

namespace nex.MarkupExtensions
{
    public sealed class ImageSourceLoader : MarkupExtension
    {
        #region Props
        [ConstructorArgument("iconName")]
        public string IconName { get; set; }
        #endregion

        #region ctor
        public ImageSourceLoader()
        {
        }

        public ImageSourceLoader(string iconName)
        {
            IconName = iconName;
        }

        #endregion

        #region ProvideValue
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (IconName == null)
                throw new ArgumentNullException("IconName");

            return Utility.LoadImageSource(IconName);
        }
        #endregion
    }
}
