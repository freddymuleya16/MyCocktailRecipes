using MyCocktailRecipes.ViewModels;
using System;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization.Json;

namespace MyCocktailRecipes.DataService
{
    public class NavigationDataService
    {

        private static NavigationDataService navigationDataService;

        private NavigationViewModel navigationViewModel;


        public static NavigationDataService Instance => navigationDataService ?? (navigationDataService = new NavigationDataService());

        public NavigationViewModel NavigationViewModel =>
            this.navigationViewModel ??
            (this.navigationViewModel = PopulateData<NavigationViewModel>("navigation.json"));

        private static T PopulateData<T>(string fileName)
        {
            var file = "MyCocktailRecipes.Data." + fileName;

            var assembly = typeof(App).GetTypeInfo().Assembly;

            T data;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://www.thecocktaildb.com/api/");

                var responseTask = client.GetAsync("json/v1/1/search.php?s=");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStreamAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                    var serializer = new DataContractJsonSerializer(typeof(T));
                    data = (T)serializer.ReadObject(readTask);
                }
                else
                {
                    using (var stream = assembly.GetManifestResourceStream(file))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(T));
                        data = (T)serializer.ReadObject(stream);
                    }
                }
            }
            return data;
        }

    }
}
