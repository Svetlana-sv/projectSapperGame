
using System.Collections.ObjectModel;
using SapperGame.Interface;
using System.IO;


using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SapperGame
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RatingPage : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }

        public RatingPage()
        {
            InitializeComponent();

            
            
            Items = new ObservableCollection<string>(DependencyService.Get<IFileService>().ReadFile());
           

            MyListView.ItemsSource = Items;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            
            ((ListView)sender).SelectedItem = null;
        }


       
    }
}
