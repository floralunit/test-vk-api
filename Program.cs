using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

namespace test_vk_api
{
    class Program
    {
        //https://oauth.vk.com/authorize?client_id=8187140&scope=notify,friends,photos,audio,video,stories,pages,menu,status,notes,wall,ads,offline,docs,groups,notifications,stats,email,market,phone_number&redirect_uri=blank.html&response_type=token
        const string token = "";
        static async Task Main(string[] args)
        {
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Пустой токен! Получи и вставь его в код, свой не дам");
                return;
            }
            string jsonPhotos = await GetApiDataPhotosFromAlbum();
            ParseJsonPhotos(jsonPhotos);
            //while (true)
            //{
            //    string jsonPlays = await GetApiDataPlays();
            //    int plays = ParseJsonPlays(jsonPlays);
            //    Console.WriteLine($"Plays: {plays} DateTime.Now: {DateTime.Now}");

            //    string jsonUser = await GetApiDataUser();
            //    ParseUsers(jsonUser);

            //    await Task.Delay(300000);
            //    Console.WriteLine("\n");
            //}
        }

        static async Task<string> GetApiDataPlays()
        {
            string methodURL = "https://api.vk.com/method/audio.getPlaylists";
            string ownerId = "283385573";
            string clientId = "8187140";
            string fullURL = $"{methodURL}?owner_id={ownerId}&count=40&v=5.95";
            // Создаем HTTP-клиент
            using (HttpClient client = new HttpClient())
            {
                // Вызываем API метод и получаем JSON-ответ
                HttpResponseMessage response = await client.GetAsync($"{methodURL}?access_token={token}&owner_id={ownerId}&count=40&v=5.95");

                // Читаем содержимое ответа в строку
                return await response.Content.ReadAsStringAsync();
            }
        }

        static async Task<string> GetApiDataUser()
        {
            string methodURL = "https://api.vk.com/method/users.get";
            string userIds = "283385573,283385573";
            string fullURL = $"{methodURL}?access_token={token}&user_ids={userIds}&fields=last_seen&v=5.95";
            // Создаем HTTP-клиент
            using (HttpClient client = new HttpClient())
            {
                // Вызываем API метод и получаем JSON-ответ
                HttpResponseMessage response = await client.GetAsync(fullURL);

                // Читаем содержимое ответа в строку
                return await response.Content.ReadAsStringAsync();
            }
        }

        static async Task<string> GetApiDataPhotosFromAlbum()
        {
            string methodURL = "https://api.vk.com/method/photos.get";
            string fullURL = $"{methodURL}?access_token={token}&owner_id=283385573&album_id=303362130&extended=1&count=100&fields=last_seen&v=5.95";
            // Создаем HTTP-клиент
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(fullURL);
                return await response.Content.ReadAsStringAsync();
            }
        }

        static int ParseJsonPlays(string json)
        {
            // Парсим JSON-строку в объект
            dynamic data = JsonConvert.DeserializeObject(json);

            foreach (var item in data.response.items)
            {
                if (item.id == 41)
                {
                    return item.plays;
                }
            }

            return 0;
        }
        static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return unixEpoch.AddSeconds(unixTimeStamp).ToLocalTime();
        }
        static void ParseUsers(string json)
        {
            // Парсим JSON-строку в объект
            dynamic data = JsonConvert.DeserializeObject(json);

            foreach (var person in data.response)
            {
                var platform = "";
                switch ((int)person.last_seen.platform)
                {
                    case 1: platform = "mobile"; break;
                    case 2: platform = "iphone"; break;
                    case 3: platform = "ipad"; break;
                    case 4: platform = "android"; break;
                    case 5: platform = "wphone"; break;
                    case 6: platform = "windows"; break;
                    case 7: platform = "web"; break;
                    default: break;
                }
                Console.WriteLine($"Name: {person.first_name} {person.last_name} Platform: {platform} Time: {UnixTimeStampToDateTime((long)person.last_seen.time)}");
            }
        }
        static void ParseJsonPhotos(string json)
        {
            // Парсим JSON-строку в объект
            dynamic data = JsonConvert.DeserializeObject(json);
            int count = 0;
            foreach (var photo in data.response.items)
            {
                var text = photo.text;
                var comments = photo.comments;
                string photourl = "";
                foreach (var size in photo.sizes)
                {
                    if (size.type == "z")
                    {
                        photourl = size.url;
                        break;
                    }
                }
                count++;
                Console.WriteLine($"Comments: {comments} Text: {text} Url: {photourl} Count: {count}");
            }
        }

    }
}