using MyCocktailRecipes.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using System.Linq;
using RestSharp.Deserializers;

namespace MyCocktailRecipes.ViewModels
{
    /// <summary>
    /// ViewModel for detail page.
    /// </summary>
    [Preserve(AllMembers = true)]
    [DataContract]
    public partial class DrinkDetailPageViewModel : BaseViewModel
    {
        #region Fields

       

        private static DrinkDetailPageViewModel detailPageViewModel;

        private double productRating;

        private ObservableCollection<Category> categories;

        private ObservableCollection<Review> reviews;

        private bool isFavourite;

        private bool isReviewVisible;

        private int? cartItemCount;

        private double actualPrice;

        private double discountPercent;

        private List<string> previewImages;

        private double overallRating;

        private double discountPrice;

        private ObservableCollection<Drink> drink;

        private Command addFavouriteCommand;

        private Command addToCartCommand;

        private Command shareCommand;

        private Command variantCommand;

        private Command itemSelectedCommand;

        private Command cardItemCommand;

        private Command loadMoreCommand;

        private string img;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance for the <see cref="DrinkDetailPageViewModel" /> class.
        /// </summary>
        static DrinkDetailPageViewModel()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the value of detail page view model.
        /// </summary>
        public static DrinkDetailPageViewModel BindingContext =>
            detailPageViewModel = PopulateData<DrinkDetailPageViewModel>("detail.json");

        /// <summary>
        /// Gets or sets the property that has been bound with StackLayout, which displays the categories using ComboBox.
        /// </summary>
        public ObservableCollection<Category> Categories
        {
            get
            {
                return this.categories;
            }

            private set
            {
                if (this.categories == value)
                {
                    return;
                }

                this.SetProperty(ref this.categories, value);
            }
        }
        

        /// <summary>
        /// Gets or sets the review of the customers .
        /// </summary>
        [DataMember(Name = "detailPageReviews")]
        public ObservableCollection<Review> Reviews
        {
            get
            {
                return this.reviews;
            }

            set
            {
                this.reviews = value;
                this.CalculateOverallRating();
                this.NotifyPropertyChanged(nameof(this.Reviews));
            }
        }

        /// <summary>
        /// Gets or sets the property that has been bound with view, which displays the Favourite.
        /// </summary>
        public bool IsFavourite
        {
            get
            {
                return this.isFavourite;
            }

            set
            {
                this.SetProperty(ref this.isFavourite, value);
            }
        }

        /// <summary>
        /// Gets or sets the property that has been bound with view, which displays the empty message.
        /// </summary>
        public bool IsReviewVisible
        {
            get
            {
                if (this.Reviews == null || this.Reviews.Count == 0)
                {
                    this.isReviewVisible = true;
                }

                return this.isReviewVisible;
            }

            set
            {
                this.SetProperty(ref this.isReviewVisible, value);
            }
        }

        /// <summary>
        /// Gets or sets the property that has been bound with label, which displays the overall rating of the product.
        /// </summary>
        public double OverallRating
        {
            get
            {
                return this.overallRating;
            }

            set
            {
                this.overallRating = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the property that has been bound with view, which displays the cart items count.
        /// </summary>
        public int? CartItemCount
        {
            get
            {
                return this.cartItemCount;
            }

            set
            {
                this.SetProperty(ref this.cartItemCount, value);
            }
        }

        /// <summary>
        /// Gets or sets the property that has been bound with a label, which displays the product name.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the property that has been bound with a label, which displays the product summary.
        /// </summary>
        [DataMember(Name = "summary")]
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the property that has been bound with a label, which displays the product description.
        /// </summary>
        [DataMember(Name = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the property that has been bound with SfCombobox, which displays the product variants.
        /// </summary>
        [DataMember(Name = "sizevariants")]
        public List<string> SizeVariants { get; set; }

        /// <summary>
        /// Gets or sets the property that has been bound with a label, which displays the actual price of the product.
        /// </summary>
        [DataMember(Name = "actualPrice")]
        public double ActualPrice
        {
            get
            {
                return this.actualPrice;
            }

            set
            {
                this.actualPrice = value;
                this.NotifyPropertyChanged(nameof(this.ActualPrice));
            }
        }

        /// <summary>
        /// Gets or sets the property that has been bound with a label, which displays the discounted percent of the product.
        /// </summary>
        [DataMember(Name = "discountPercent")]
        public double DiscountPercent
        {
            get
            {
                return this.discountPercent;
            }

            set
            {
                this.discountPercent = value;
                this.NotifyPropertyChanged(nameof(this.DiscountPercent));
            }
        }

        /// <summary>
        /// Gets or sets the property that has been bound with a label, which displays the discounted price of the product.
        /// </summary>
        public double DiscountPrice
        {
            get
            {
                return this.ActualPrice - (this.ActualPrice * (this.DiscountPercent / 100));
            }

            set
            {
                this.discountPrice = value;
                this.NotifyPropertyChanged(nameof(this.DiscountPrice));
            }
        }

        /// <summary>
        /// Gets or sets the property that has been bound with SfRotator, which displays the item images.
        /// </summary>
        [DataMember(Name = "previewImages")]
        public List<string> PreviewImages
        {
            get
            {
                for (var i = 0; i < this.previewImages.Count; i++)
                {
                    this.previewImages[i] = this.previewImages[i].Contains(App.ImageServerPath) ? this.previewImages[i] : App.ImageServerPath + this.previewImages[i];
                }

                return this.previewImages;
            }

            set
            {
                this.previewImages = value;
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets or sets the command that will be executed when the Favourite button is clicked.
        /// </summary>
        public Command AddFavouriteCommand
        {
            get
            {
                return this.addFavouriteCommand ?? (this.addFavouriteCommand = new Command(this.AddFavouriteClicked));
            }
        }

        /// <summary>
        /// Gets or sets the command that will be executed when the AddToCart button is clicked.
        /// </summary>
        public Command AddToCartCommand
        {
            get
            {
                return this.addToCartCommand ?? (this.addToCartCommand = new Command(this.AddToCartClicked));
            }
        }

        /// <summary>
        /// Gets or sets the command that will be executed when the Share button is clicked.
        /// </summary>
        public Command ShareCommand
        {
            get
            {
                return this.shareCommand ?? (this.shareCommand = new Command(this.ShareClicked));
            }
        }

        /// <summary>
        /// Gets or sets the command that will be executed when the size button is clicked.
        /// </summary>
        public Command VariantCommand
        {
            get
            {
                return this.variantCommand ?? (this.variantCommand = new Command(this.VariantClicked));
            }
        }

        /// <summary>
        /// Gets or sets the command that will be executed when an item is selected.
        /// </summary>
        public Command ItemSelectedCommand
        {
            get
            {
                return this.itemSelectedCommand ?? (this.itemSelectedCommand = new Command(this.ItemSelected));
            }
        }

        /// <summary>
        /// Gets or sets the command that will be executed when cart button is clicked.
        /// </summary>
        public Command CardItemCommand
        {
            get
            {
                return this.cardItemCommand ?? (this.cardItemCommand = new Command(this.CartClicked));
            }
        }

        /// <summary>
        /// Gets or sets the command that will be executed when the Show All button is clicked.
        /// </summary>
        public Command LoadMoreCommand
        {
            get
            {
                return this.loadMoreCommand ?? (this.loadMoreCommand = new Command(this.LoadMoreClicked));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Populates the data for view model from json file.
        /// </summary>
        /// <typeparam name="T">Type of view model.</typeparam>
        /// <param name="fileName">Json file to fetch data.</param>
        /// <returns>Returns the view model object.</returns>
        private static T PopulateData<T>(string fileName)
        {
            var file = "MyCocktailRecipes.Data." + fileName;

            var assembly = typeof(App).GetTypeInfo().Assembly;

            T data;

            using (var stream = assembly.GetManifestResourceStream(file))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                data = (T)serializer.ReadObject(stream);
            }

            return data;
        }

        /// <summary>
        /// Invoked when the Favourite button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private void CalculateOverallRating()
        {
            if (this.Reviews == null || this.Reviews.Count == 0)
            {
                this.IsReviewVisible = true;
            }
            else
            {
                foreach (var review in this.Reviews)
                {
                    this.productRating += review.Rating;
                }
            }

            if (this.productRating > 0)
            {
                this.OverallRating = this.productRating / this.Reviews.Count;
            }
        }

        /// <summary>
        /// Invoked when the Favourite button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private void AddFavouriteClicked(object obj)
        {
            if (obj is DrinkDetailPageViewModel model)
            {
                model.IsFavourite = !model.IsFavourite;
            }
        }

        /// <summary>
        /// Invoked when the Cart button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private void AddToCartClicked(object obj)
        {
            this.cartItemCount = this.cartItemCount ?? 0;
            this.CartItemCount += 1;
        }

        /// <summary>
        /// Invoked when the Share button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private void ShareClicked(object obj)
        {
            // Do something.
        }

        /// <summary>
        /// Invoked when the variant button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private void VariantClicked(object obj)
        {
            // Do something
        }

        /// <summary>
        /// Invoked when an item is selected.
        /// </summary>
        /// <param name="attachedObject">The Object</param>
        private void ItemSelected(object attachedObject)
        {
            // Do something
        }

        /// <summary>
        /// Invoked when cart icon button is clicked.
        /// </summary>
        /// <param name="obj">The Object.</param>
        private void CartClicked(object obj)
        {
            // Do something
        }

        /// <summary>
        /// Invoked when Load more button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private void LoadMoreClicked(object obj)
        {
            // Do something
        }

        #endregion
    }

    public partial class DrinkDetailPageViewModel : BaseViewModel
    {
        private string previewImage;
        private string StrAlcoholic;

        public string idDrink { get; set; }
        public string MyProperty { get; set; }
        public string strDrink { get; set; }
        public string strDrinkAlternate { get; set; }
        public string strTags { get; set; }
        public string strVideo { get; set; }
        public string strCategory { get; set; }
        public string strIBA { get; set; }
        public string strAlcoholic { get; set; }
        

        public string strGlass { get; set; }
        public string strInstructions { get; set; }
        public string strInstructionsES { get; set; }
        public string strInstructionsDE { get; set; }
        public string strInstructionsFR { get; set; }
        public string strInstructionsIT { get; set; }
        [DeserializeAs(Name = "strInstructionsZH-HANS")]
        public string strInstructionsZH_HANS { get; set; }
        [DeserializeAs(Name = "strInstructionsZH-HANT")]
        public string strInstructionsZH_HANT { get; set; }
        public string strDrinkThumb
        {
            get { return previewImage + "/preview"; }
            set { previewImage = value; }
        }
        public string strIngredient1 { get; set; }
        public string strIngredient2 { get; set; }
        public string strIngredient3 { get; set; }
        public string strIngredient4 { get; set; }
        public string strIngredient5 { get; set; }
        public string strIngredient6 { get; set; }
        public string strIngredient7 { get; set; }
        public string strIngredient8 { get; set; }
        public string strIngredient9 { get; set; }
        public string strIngredient10 { get; set; }
        public string strIngredient11 { get; set; }
        public string strIngredient12 { get; set; }
        public string strIngredient13 { get; set; }
        public string strIngredient14 { get; set; }
        public string strIngredient15 { get; set; }
        public string strMeasure1 { get; set; }
        public string strMeasure2 { get; set; }
        public string strMeasure3 { get; set; }
        public string strMeasure4 { get; set; }
        public string strMeasure5 { get; set; }
        public string strMeasure6 { get; set; }
        public string strMeasure7 { get; set; }
        public string strMeasure8 { get; set; }
        public string strMeasure9 { get; set; }
        public string strMeasure10 { get; set; }
        public string strMeasure11 { get; set; }
        public string strMeasure12 { get; set; }
        public string strMeasure13 { get; set; }
        public string strMeasure14 { get; set; }
        public string strMeasure15 { get; set; }
        public string strImageSource { get; set; }
        public string strImageAttribution { get; set; }
        public string strCreativeCommonsConfirmed { get; set; }
        public string dateModified { get; set; }
        [DataMember(Name = "drinks")]
        public ObservableCollection<Drink> drinks
        {
            get { return drink; }
            set
            {

                this.drink = value;
                this.idDrink = value.FirstOrDefault().idDrink;
                this.strDrink =  value.FirstOrDefault().strDrink;
                this.strDrinkAlternate =  value.FirstOrDefault().strDrinkAlternate;
                this.strTags =  value.FirstOrDefault().strTags;
                this.strVideo =  value.FirstOrDefault().strVideo;
                this.strCategory =  value.FirstOrDefault().strCategory;
                this.strIBA =  value.FirstOrDefault().strIBA;
                this.strAlcoholic =  value.FirstOrDefault().strAlcoholic;
                this.strGlass =  value.FirstOrDefault().strGlass;
                this.strInstructions =  value.FirstOrDefault().strInstructions;
                this.strInstructionsES =  value.FirstOrDefault().strInstructionsES;
                this.strInstructionsDE =  value.FirstOrDefault().strInstructionsDE;
                this.strInstructionsFR =  value.FirstOrDefault().strInstructionsFR;
                this.strInstructionsIT =  value.FirstOrDefault().strInstructionsIT;
                this.strInstructionsZH_HANS =  value.FirstOrDefault().strInstructionsZH_HANS;
                this.strInstructionsZH_HANT =  value.FirstOrDefault().strInstructionsZH_HANT;
                this.strDrinkThumb =  value.FirstOrDefault().strDrinkThumb;
                this.strIngredient1 =  value.FirstOrDefault().strIngredient1;
                this.strIngredient2 =  value.FirstOrDefault().strIngredient2;
                this.strIngredient3 =  value.FirstOrDefault().strIngredient3;
                this.strIngredient4 =  value.FirstOrDefault().strIngredient4;
                this.strIngredient5 =  value.FirstOrDefault().strIngredient5;
                this.strIngredient6 =  value.FirstOrDefault().strIngredient6;
                this.strIngredient7 =  value.FirstOrDefault().strIngredient7;
                this.strIngredient8 =  value.FirstOrDefault().strIngredient8;
                this.strIngredient9 =  value.FirstOrDefault().strIngredient9;
                this.strIngredient10 =  value.FirstOrDefault().strIngredient10;
                this.strIngredient11 =  value.FirstOrDefault().strIngredient11;
                this.strIngredient12 =  value.FirstOrDefault().strIngredient12;
                this.strIngredient13 =  value.FirstOrDefault().strIngredient13;
                this.strIngredient14 =  value.FirstOrDefault().strIngredient14;
                this.strIngredient15 =  value.FirstOrDefault().strIngredient15;
                this.strMeasure1 =  value.FirstOrDefault().strMeasure1;
                this.strMeasure2 =  value.FirstOrDefault().strMeasure2;
                this.strMeasure3 =  value.FirstOrDefault().strMeasure3;
                this.strMeasure4 =  value.FirstOrDefault().strMeasure4;
                this.strMeasure5 =  value.FirstOrDefault().strMeasure5;
                this.strMeasure6 =  value.FirstOrDefault().strMeasure6;
                this.strMeasure7 =  value.FirstOrDefault().strMeasure7;
                this.strMeasure8 =  value.FirstOrDefault().strMeasure8;
                this.strMeasure9 =  value.FirstOrDefault().strMeasure9;
                this.strMeasure10 =  value.FirstOrDefault().strMeasure10;
                this.strMeasure11 =  value.FirstOrDefault().strMeasure11;
                this.strMeasure12 =  value.FirstOrDefault().strMeasure12;
                this.strMeasure13 =  value.FirstOrDefault().strMeasure13;
                this.strMeasure14 =  value.FirstOrDefault().strMeasure14;
                this.strMeasure15 =  value.FirstOrDefault().strMeasure15;
                this.strImageSource =  value.FirstOrDefault().strImageSource;
                this.strImageAttribution =  value.FirstOrDefault().strImageAttribution;
                this.strCreativeCommonsConfirmed =  value.FirstOrDefault().strCreativeCommonsConfirmed;
                this.dateModified = value.FirstOrDefault().dateModified;
                //this.previewImages.Add(value.FirstOrDefault().strImageSource);
            }
        }
    }
}
