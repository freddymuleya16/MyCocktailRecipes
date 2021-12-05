using MyCocktailRecipes.DataService;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace MyCocktailRecipes.Views
{
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DrinkListPage
    {
        public DrinkListPage()
        {
            this.InitializeComponent();
            this.BindingContext = NavigationDataService.Instance.NavigationViewModel;
        }
    }
}