using api_access.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace api_access
{
    public static class AccessAPI
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string BaseUrl = "https://localhost:44303/api";

        public static async Task<bool> CheckApiConnection()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{BaseUrl}/Departments");
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<(List<Author>, bool)> GetAuthors()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{BaseUrl}/Authors");
                response.EnsureSuccessStatusCode();

                List<Author> authors = await response.Content.ReadAsAsync<List<Author>>();

                return (authors, false); // Zwraca dane i informację o braku błędu
            }
            catch
            {
                return (new List<Author>(), true);
            }
        }


        public static async Task<DashboardDataDto> GetDashboardData()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{BaseUrl}/Dashboard");
                response.EnsureSuccessStatusCode();

                DashboardDataDto data = await response.Content.ReadAsAsync<DashboardDataDto>();

                return data;
            }
            catch
            {
                return new DashboardDataDto();
            }
        }

        public static async Task<Author> CreateAuthor(Author author)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{BaseUrl}/Authors", author);
                response.EnsureSuccessStatusCode();

                Author createdAuthor = await response.Content.ReadAsAsync<Author>();

                return createdAuthor;
            }
            catch
            {
                return null;
            }
        }

        public static async void CreateAuthor_Title(Author__Title title)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{BaseUrl}/Author_Title", title);
                response.EnsureSuccessStatusCode();

                Author__Title result = await response.Content.ReadAsAsync<Author__Title>();

            }
            catch
            {
            }
        }

        public static async void UpdateAllAuthorTitles(List<Author__Title> oldTitles, List<Author__Title> newTitles)
        {
            try
            {
                foreach (var oldTitle in oldTitles)
                {
                    HttpResponseMessage deleteResponse = await httpClient.DeleteAsync($"{BaseUrl}/Author_Title/{oldTitle.id}");
                    deleteResponse.EnsureSuccessStatusCode();
                }

                foreach (var newTitle in newTitles)
                {
                    CreateAuthor_Title(newTitle);
                }
            }
            catch
            {
            }
        }





        public static async Task<Author> UpdateAuthor(Author author)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{BaseUrl}/Authors/{author.id}", author);
                response.EnsureSuccessStatusCode();

                Author updatedAuthor = await response.Content.ReadAsAsync<Author>();

                return updatedAuthor;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<List<Book>> GetBooks()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{BaseUrl}/Books");
                response.EnsureSuccessStatusCode();

                List<Book> books = await response.Content.ReadAsAsync<List<Book>>();

                return books;
            }
            catch
            {
                return new List<Book>();
            }
        }

        public static async Task<List<Conference>> GetConferences()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{BaseUrl}/Conferences");
                response.EnsureSuccessStatusCode();

                List<Conference> conferences = await response.Content.ReadAsAsync<List<Conference>>();

                return conferences;
            }
            catch
            {
                return new List<Conference>();
            }
        }

        public static async Task<Book> GetBookAuthors(int id)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{BaseUrl}/Books/{id}");
                response.EnsureSuccessStatusCode();

                Book result = await response.Content.ReadAsAsync<Book>();

                return result;
            }
            catch
            {
                return new Book();
            }
        }

        public static async Task<Article> GetArticleAuthors(int id)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{BaseUrl}/Articles/{id}");
                response.EnsureSuccessStatusCode();

                Article result = await response.Content.ReadAsAsync<Article>();

                return result;
            }
            catch
            {
                return new Article();
            }
        }

        public static async Task<Conference> GetConferenceAuthors(int id)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{BaseUrl}/Conferences/{id}");
                response.EnsureSuccessStatusCode();

                Conference result = await response.Content.ReadAsAsync<Conference>();

                return result;
            }
            catch
            {
                return new Conference();
            }
        }

        public static async Task<Research> GetResearchAuthors(int id)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{BaseUrl}/Research/{id}");
                response.EnsureSuccessStatusCode();

                Research result = await response.Content.ReadAsAsync<Research>();

                return result;
            }
            catch
            {
                return new Research();
            }
        }

        public static async Task<List<Article>> GetArticles()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{BaseUrl}/Articles");
                response.EnsureSuccessStatusCode();

                List<Article> articles = await response.Content.ReadAsAsync<List<Article>>();

                return articles;
            }
            catch
            {
                return new List<Article>();
            }
        }

        public static async Task<List<Research>> GetResearches()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{BaseUrl}/Research");
                response.EnsureSuccessStatusCode();

                List<Research> researches = await response.Content.ReadAsAsync<List<Research>>();

                return researches;
            }
            catch
            {
                return new List<Research>();
            }
        }

        public static async Task<Research> CreateResearch(Research research)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{BaseUrl}/Research", research);
                response.EnsureSuccessStatusCode();

                Research createdResearch = await response.Content.ReadAsAsync<Research>();

                return createdResearch;
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine($"Błąd odczytu JSON: {ex.Message}");
                Console.WriteLine($"Linia: {ex.LineNumber}, Pozycja: {ex.LinePosition}");

                return null;
            }
        }

        public static async Task<Research> UpdateResearch(Research research)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{BaseUrl}/Research/{research.id}", research);
                response.EnsureSuccessStatusCode();

                Research result = await response.Content.ReadAsAsync<Research>();

                return result;
            }
            catch
            {
                return null;
            }
        }

        public static async Task DeleteResearch(int id)
        {
            try
            {
                await DeleteResearchAuthors(id);
                HttpResponseMessage response = await httpClient.DeleteAsync($"{BaseUrl}/Research/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch
            {
            }
        }

        public static async Task<Author__Research> createAuthorResearch(Author__Research author_research)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{BaseUrl}/Author_Research", author_research);
                response.EnsureSuccessStatusCode();

                Author__Research createdAuthorResearch = await response.Content.ReadAsAsync<Author__Research>();

                return createdAuthorResearch;
            }
            catch
            {
                return null;
            }
        }

        public static async Task DeleteResearchAuthors(int id)
        {
            try
            {
                Research research = await GetResearchAuthors(id);

                if (research.Author__Research != null && research.Author__Research.Any())
                {
                    var deleteTasks = research.Author__Research.Select(author =>
                        httpClient.DeleteAsync($"{BaseUrl}/Author_Research/{author.id}")
                    );

                    await Task.WhenAll(deleteTasks);
                }
            }
            catch
            {
            }
        }

        public static async Task<Book> CreateBook(Book book)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{BaseUrl}/Books", book);
                response.EnsureSuccessStatusCode();

                Book createdBook = await response.Content.ReadAsAsync<Book>();

                return createdBook;
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine($"Błąd odczytu JSON: {ex.Message}");
                Console.WriteLine($"Linia: {ex.LineNumber}, Pozycja: {ex.LinePosition}");

                return null;
            }
        }

        public static async Task<Book> UpdateBook(Book book)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{BaseUrl}/Books/{book.id}", book);
                response.EnsureSuccessStatusCode();

                Book result = await response.Content.ReadAsAsync<Book>();

                return result;
            }
            catch
            {
                return null;
            }
        }




        public static async Task<Article> CreateArticle(Article article)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{BaseUrl}/Articles", article);
                response.EnsureSuccessStatusCode();

                Article createdArticle = await response.Content.ReadAsAsync<Article>();

                return createdArticle;
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine($"Błąd odczytu JSON: {ex.Message}");
                Console.WriteLine($"Linia: {ex.LineNumber}, Pozycja: {ex.LinePosition}");

                return null;
            }
        }

        public static async Task<Article> UpdateArticle(Article article)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{BaseUrl}/Articles/{article.id}", article);
                response.EnsureSuccessStatusCode();

                Article result = await response.Content.ReadAsAsync<Article>();

                return result;
            }
            catch
            {
                return null;
            }
        }



        public static async Task<Conference> CreateConference(Conference conference)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{BaseUrl}/Conferences", conference);
                response.EnsureSuccessStatusCode();

                Conference createdConference = await response.Content.ReadAsAsync<Conference>();

                return createdConference;
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine($"Błąd odczytu JSON: {ex.Message}");
                Console.WriteLine($"Linia: {ex.LineNumber}, Pozycja: {ex.LinePosition}");

                return null;
            }
        }

        public static async Task<Conference> UpdateConference(Conference conference)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{BaseUrl}/Conferences/{conference.id}", conference);
                response.EnsureSuccessStatusCode();

                Conference result = await response.Content.ReadAsAsync<Conference>();

                return result;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Author__Conference> createAuthorConference(Author__Conference author_conference)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{BaseUrl}/Author_Conference", author_conference);
                response.EnsureSuccessStatusCode();

                Author__Conference createdAuthorConference = await response.Content.ReadAsAsync<Author__Conference>();

                return createdAuthorConference;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Author__Article> createAuthorArticle(Author__Article author_article)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{BaseUrl}/Author_Article", author_article);
                response.EnsureSuccessStatusCode();

                Author__Article createdAuthorArticle = await response.Content.ReadAsAsync<Author__Article>();

                return createdAuthorArticle;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Author__Book> createAuthorBook(Author__Book author_book)
        {
            try
            {
                HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{BaseUrl}/Author_Book", author_book);
                response.EnsureSuccessStatusCode();

                Author__Book createdAuthorBook = await response.Content.ReadAsAsync<Author__Book>();

                return createdAuthorBook;
            }
            catch
            {
                return null;
            }
        }

        public static async Task DeleteBookAuthors(int id)
        {
            try
            {
                Book book = await GetBookAuthors(id);

                if (book.Author__Book != null && book.Author__Book.Any())
                {
                    var deleteTasks = book.Author__Book.Select(author =>
                        httpClient.DeleteAsync($"{BaseUrl}/Author_Book/{author.id}")
                    );

                    await Task.WhenAll(deleteTasks);
                }
            }
            catch
            {
            }
        }

        public static async Task DeleteArticleAuthors(int id)
        {
            try
            {
                Article article = await GetArticleAuthors(id);

                if (article.Author__Article != null && article.Author__Article.Any())
                {
                    var deleteTasks = article.Author__Article.Select(author =>
                        httpClient.DeleteAsync($"{BaseUrl}/Author_Article/{author.id}")
                    );

                    await Task.WhenAll(deleteTasks);
                }
            }
            catch
            {
            }
        }

        public static async Task DeleteConferenceAuthors(int id)
        {
            try
            {
                Conference conference = await GetConferenceAuthors(id);

                if (conference.Author__Conference != null && conference.Author__Conference.Any())
                {
                    var deleteTasks = conference.Author__Conference.Select(author =>
                        httpClient.DeleteAsync($"{BaseUrl}/Author_Conference/{author.id}")
                    );

                    await Task.WhenAll(deleteTasks);
                }
            }
            catch
            {
            }
        }

        public static async Task DeleteBook(int id)
        {
            try
            {
                await DeleteBookAuthors(id);
                HttpResponseMessage response = await httpClient.DeleteAsync($"{BaseUrl}/Books/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch
            {
            }
        }

        public static async Task DeleteArticle(int id)
        {
            try
            {
                await DeleteArticleAuthors(id);
                HttpResponseMessage response = await httpClient.DeleteAsync($"{BaseUrl}/Articles/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch
            {
            }
        }

        public static async Task DeleteConference(int id)
        {
            try
            {
                await DeleteConferenceAuthors(id);
                HttpResponseMessage response = await httpClient.DeleteAsync($"{BaseUrl}/Conferences/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch
            {
            }
        }

        public static async Task<List<Publisher>> GetPublishers()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{BaseUrl}/Publishers");
                response.EnsureSuccessStatusCode();

                List<Publisher> publishers = await response.Content.ReadAsAsync<List<Publisher>>();

                return publishers;
            }
            catch
            {
                return new List<Publisher>();
            }
        }

        public static async Task<List<Department>> GetDepartments()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{BaseUrl}/Departments");
                response.EnsureSuccessStatusCode();

                List<Department> departments = await response.Content.ReadAsAsync<List<Department>>();

                return departments;
            }
            catch
            {
                return new List<Department>();
            }
        }

        public static async Task<List<TitleAndDegree>> GetTitlesAndDegrees()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{BaseUrl}/TitlesAndDegrees");
                response.EnsureSuccessStatusCode();

                List<TitleAndDegree> result = await response.Content.ReadAsAsync<List<TitleAndDegree>>();

                return result;
            }
            catch
            {
                return new List<TitleAndDegree>();
            }
        }


    }
}