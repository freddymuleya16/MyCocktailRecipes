using MyCocktailRecipes.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace MyCocktailRecipes.Views
{
    /// <summary>
    /// The Detail page.
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DrinkDetailPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrinkDetailPage" /> class.
        /// </summary>
        public DrinkDetailPage()
        {
            this.InitializeComponent();
            //this.BindingContext = DrinkDetailPageViewModel.BindingContext;
        }

        /// <summary>
        /// Invoked when view size is changed.
        /// </summary>
        /// <param name="width">The Width</param>
        /// <param name="height">The Height</param>
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width > height)
            {
                this.Rotator.ItemTemplate = (DataTemplate)this.Resources["LandscapeTemplate"];
            }
            else
            {
                this.Rotator.ItemTemplate = (DataTemplate)this.Resources["PortraitTemplate"];
            }
        }
    }
}