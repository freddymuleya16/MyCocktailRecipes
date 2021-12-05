using MyCocktailRecipes.Models;
using MyCocktailRecipes.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using NavigationModel = MyCocktailRecipes.Models.NavigationModel;

namespace MyCocktailRecipes.ViewModels
{
    /// <summary>
    /// ViewModel for the Navigation list with cards page.
    /// </summary>
    [Preserve(AllMembers = true)]
    [DataContract]
    public class NavigationViewModel : BaseViewModel
    {
        private Command<object> itemTappedCommand;

        public NavigationViewModel()
        {
        }

        public Command<object> ItemTappedCommand
        {
            get
            {
                return this.itemTappedCommand ?? (this.itemTappedCommand = new Command<object>(this.NavigateToNextPage));
            }
        }

        [DataMember(Name = "drinks")]
        public ObservableCollection<Drink> drinks { get; set; }

        private void NavigateToNextPage(Object selectedItem)
        {

            var selectedDrink = (Drink)selectedItem;
           var drink = GetDrinkById<DrinkDetailPageViewModel>(selectedDrink.idDrink);
            var newPage = new DrinkDetailPage();
            //var temp = drink.drinks.FirstOrDefault();
            newPage.BindingContext = drink;
            Application.Current.MainPage = new NavigationPage(newPage);

        }

        private T GetDrinkById<T>(string id)
        {

            

            T data;

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("https://www.thecocktaildb.com/api/");

                    var responseTask = client.GetAsync("json/v1/1/lookup.php?i=" + id);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (true)//result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsStreamAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                        var serializer = new DataContractJsonSerializer(typeof(T));
                        data = (T)serializer.ReadObject(readTask);
                    }
                    else
                    {
                         Type type = typeof(T) ;

                        //data = Drink;
                    }
                
            return data;

            }
                catch (Exception)
                {

                    throw;
                }
                
        }

    }

}
}