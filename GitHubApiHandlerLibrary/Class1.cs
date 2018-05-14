using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace GitHubApiHandlerLibrary
{
    public class GithubApiHandler
    {
        private static readonly HttpClient client = new HttpClient();

        private const string GithubApiDotnetEventsUrl = "https://api.github.com/orgs/dotnet/events",
            GithubApiDotnetReposUrl = "https://api.github.com/orgs/dotnet/repos",
            GithubApiUsersUrl = "https://api.github.com/users/";

        private static void PrepareClient()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
        }

        public static async Task<List<Repository>> ProcessRepositories(string username = "", int page = 1)
        {
            var serializer = new DataContractJsonSerializer(typeof(List<Repository>));

            try
            {
                PrepareClient();
                string url = string.IsNullOrEmpty(username) ? GithubApiDotnetReposUrl : string.Format("{0}{1}/repos", GithubApiUsersUrl, username);
                url += string.Format("?page={0}", page);
                var streamTask = await client.GetStreamAsync(url);
                var repositories = serializer.ReadObject(streamTask) as List<Repository>;

                return repositories;
            }
            catch (AggregateException ae)
            {
                Console.WriteLine("One or more exceptions occurred: ");
                foreach (var ex in ae.Flatten().InnerExceptions)
                    Console.WriteLine("   {0}", ex.Message);
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException || ex is TimeoutException || ex is TaskCanceledException)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", ex.Message);
                }
            }

            return null;
        }


        public static IEnumerable<Repository> GetRepositoryByLinqCriteria(List<Repository> repositories)
        {
            IEnumerable<Repository> res = from r in repositories where r.forksCount > 100 && !r.isPrivate && r.Watchers > 100 select r;

            return res;
        }

        public static Repository GetRepoWithMaxWatchersCount(List<Repository> repositories)
        {
            return repositories.Aggregate((r1, r2) => r1.Watchers > r2.Watchers ? r1 : r2);
        }

        public static double GetAverageWatchersCount(List<Repository> repositories)
        {
            return repositories.Average(repo => repo.Watchers);
        }

        public static Task<List<Repository>> GetReposByUserName(string username, int page = 1)
        {
            return ProcessRepositories(username, page);
        }
    }
}
