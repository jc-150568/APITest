using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Windows.Input;

namespace Sample001
{
    public class MainPage : BindableObject
    {
        // ListViewのデータソース
        public ObservableCollection<Color> Colors
        {
            get;
            private set;
        }

        // ListView.IsRefreshingと同期させるプロパティ
        private bool isRefreshing;
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set
            {
                if (value == isRefreshing)
                    return;
                isRefreshing = value;
                OnPropertyChanged();
            }
        }

        // ListViewを引っ張った時に実行させるコマンド
        public ICommand RefreshCommand
        {
            get;
            private set;
        }

        public MainPageViewModel()
        {
            Colors = new ObservableCollection<Color> {
                Color.Aqua,
                Color.Blue,
                Color.Fuchsia,
                Color.Gray,
                Color.Green,
                Color.Lime,
                Color.Maroon,
                Color.Navy,
                Color.Olive,
                Color.Pink,
            };

            var random = new Random(140);

            RefreshCommand = new Command(async (nothing) => {
                // ランダムな色に更新
                for (var i = 0; i < Colors.Count; i++)
                {
                    await Task.Delay(100);
                    Colors[i] = new Color(
                        random.NextDouble(),
                        random.NextDouble(),
                        random.NextDouble()
                    );
                }

                // Binding機構経由でListViewのIsRefreshingプロパティも変更する
                IsRefreshing = false;
            },
                // ICommand.CanExecuteにもバインドしたプロパティを利用できる
                (nothing) => !IsRefreshing
            );
        }
    }
}