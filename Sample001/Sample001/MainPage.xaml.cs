using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Sample001

{
    public partial class MainPage : ContentPage
    {
        private string url;
        static string requestUrl;
        ObservableCollection<string> scanedData;

        public MainPage()
        {
            InitializeComponent();
            url = "https://app.rakuten.co.jp/services/api/BooksBook/Search/20170404?format=json&applicationId=1051637750796067320&formatVersion=2"; //formatVersion=2にした
            scanedData = new ObservableCollection<string>();
            this.BindingContext = scanedData;
        }

        async void ScanButtonClicked(object sender, EventArgs s)
        {
            try
            {
                var scanPage = new ZXingScannerPage()
                {
                    DefaultOverlayTopText = "バーコードを読み取ります",
                    DefaultOverlayBottomText = "",
                };

                // スキャナページを表示
                await Navigation.PushAsync(scanPage);

                scanPage.OnScanResult += async (result) =>
                {
                    // スキャン停止
                    scanPage.IsScanning = false;

                    // PopAsyncで元のページに戻り、結果をダイアログで表示
                    Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Navigation.PopAsync();
                        // await DisplayAlert("スキャン完了", result.Text, "OK");
                    });

                    string isbncode = result.Text;

                    requestUrl = url + "&isbn=" + isbncode; //URLにISBNコードを挿入

                    

                    //HTTPアクセスメソッドを呼び出す
                    string APIdata = await GetApiAsync(); //jsonをstringで受け取る

                    //HTTPアクセス失敗処理(404エラーとか名前解決失敗とかタイムアウトとか)
                    if (APIdata is null)
                    {
                        await DisplayAlert("接続エラー", "接続に失敗しました", "OK");
                    }
                    
                    

                    /*
                    //レスポンス(JSON)をstringに変換-------------->しなくていい
                    Stream s = GetMemoryStream(APIdata); //GetMemoryStreamメソッド呼び出し
                    StreamReader sr = new StreamReader(s);
                    string json = sr.ReadToEnd();
                    */
                    /*
                    //デシリアライズ------------------>しなくていい
                    var rakutenBooks = JsonConvert.DeserializeObject<RakutenBooks>(json.ToString());
                    */

                    //パースする *重要*   パースとは、文法に従って分析する、品詞を記述する、構文解析する、などの意味を持つ英単語。
                    var json = JObject.Parse(APIdata); //stringのAPIdataをJObjectにパース
                    var Items = JArray.Parse(json["Items"].ToString()); //Itemsは配列なのでJArrayにパース
                   
                    //ここまで来てる---------------------
                    //結果を出力
                    foreach (JObject jobj in Items)
                    {
                        //↓のように取り出す
                        JValue titleValue = (JValue)jobj["title"];
                        string title = (string)titleValue.Value;
                        
                        JValue titleKanaValue = (JValue)jobj["titleKana"];
                        string titleKana = (string)titleKanaValue.Value;
                       
                        JValue itemCaptionValue = (JValue)jobj["itemCaption"];
                        string itemCaption = (string)itemCaptionValue.Value;
                        
                        JValue gazoValue = (JValue)jobj["largeImageUrl"];
                        string gazo = (string)gazoValue.Value;

                        /*//書き出し
                        layout.Children.Add(new Label { Text = $"title: { title }" });
                        layout.Children.Add(new Label { Text = $"titleKana: { titleKana }" });
                        layout.Children.Add(new Label { Text = $"itemCaption: { itemCaption }" });
                        layout.Children.Add(new Image { Source = gazo });
                        String A = gazo; */

                        BookDB.insertUser(isbncode, title, titleKana,itemCaption);

                    };
                    
                    /*layout.Children.Add(new Label { Text = "読み取り終了", TextColor = Color.Black });
                    

                    layout.Children.Add(new Label { Text = "" });//改行
                    
                    layout.Children.Add(new Label { Text = "JSON形式で書き出す", TextColor = Color.Red });
                    
                    layout.Children.Add(new Label { Text = json.ToString() });*/
                    

                    
                };
                
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", e.ToString(), "ok");
            }


        }

        //HTTPアクセスメソッド
        public static async Task<string> GetApiAsync()
        {
            string APIurl = requestUrl;

            using (HttpClient client = new HttpClient())
                try
                {
                    string urlContents = await client.GetStringAsync(APIurl);
                    await Task.Delay(1000); //1秒待つ(楽天API規約に違反するため)
                    return urlContents;
                }
                catch (Exception e)
                {
                    string a = e.ToString();
                    return null;
                }
        }

        private void Hyouji()
        {
            var layout2 = new StackLayout { HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
            var scroll = new ScrollView { Orientation = ScrollOrientation.Vertical };
            layout2.Children.Add(scroll);
            var layout = new StackLayout { HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
            scroll.Content = layout;

            var query = BookDB.selectUser();
            foreach(var book in query)
            {
                layout.Children.Add(new Label { Text = book.ISBN });
                layout.Children.Add(new Label {Text = book.Title });
                layout.Children.Add(new Label { Text = book.TitleKana });
                layout.Children.Add(new Label { Text = book.ItemCaption });
            }

            Content = layout2;
        }

    }
}