using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BookClient.Data
{
    public class BookManager
    {
        const string Url = "http://xam150.azurewebsites.net/api/books/";
        private string authorizationKey;

        async Task<HttpClient> GetClientAsync()
        {
            HttpClient client = new HttpClient();
            if (string.IsNullOrEmpty(authorizationKey))
            {
                authorizationKey = await client.GetStringAsync(Url + "login");
                authorizationKey = JsonConvert.DeserializeObject<string>(authorizationKey);
            }

            client.DefaultRequestHeaders.Add("Authorization", authorizationKey);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return client;
        }

        public async Task<IEnumerable<Book>> GetAll()
        {
            HttpClient client = await GetClientAsync();
            string result = await client.GetStringAsync(Url);
            return JsonConvert.DeserializeObject<IEnumerable<Book>>(result);
        }

        public async Task<Book> AddAsync(string title, string author, string genre)
        {
            Book newBook = new Book()
            {
                ISBN="",
                Title =title,
                Authors= new List<string>() { author },
                Genre = genre,
                PublishDate= DateTime.Now,
            };

            HttpClient client = await GetClientAsync();

            var content = JsonConvert.SerializeObject(newBook);

            var reponse = await client.PostAsync(Url, 
                                           new StringContent(content, Encoding.UTF8, "application/json"));
            return JsonConvert.DeserializeObject<Book>(await reponse.Content.ReadAsStringAsync());

        }

        public async Task Update(Book book)
        {
            HttpClient client = await GetClientAsync();
            await client.PutAsync(Url + book.ISBN,
                                    new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, "application/json"));
        }

        public async Task DeleteAsync(string isbn)
        {
            HttpClient client = await GetClientAsync();
            await client.DeleteAsync(Url + isbn);
        }
    }
}

