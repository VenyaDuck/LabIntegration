using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI;
using Newtonsoft.Json;
using System.Text;
using System.IO;

namespace MoviePicker
{
    public partial class Default : Page
    {
        private const string TMDB_API_KEY = "45df0a8ffc3abdbfc69501cf9820872b";
        private const string OMDB_API_KEY = "582f74de";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadGenres();
            }
        }

        private void LoadGenres()
        {
            ddlGenre.Items.Clear();
            ddlGenre.Items.Add(new System.Web.UI.WebControls.ListItem("Боевик", "28"));
            ddlGenre.Items.Add(new System.Web.UI.WebControls.ListItem("Комедия", "35"));
            ddlGenre.Items.Add(new System.Web.UI.WebControls.ListItem("Драма", "18"));
            ddlGenre.Items.Add(new System.Web.UI.WebControls.ListItem("Фантастика", "878"));
            ddlGenre.Items.Add(new System.Web.UI.WebControls.ListItem("Ужасы", "27"));
        }

        protected async void btnSearch_Click(object sender, EventArgs e)
        {
            btnSaveJson.Visible = false;
            gvMovies.Visible = false;

            int genreId = int.Parse(ddlGenre.SelectedValue);
            double minRating = 7;
            int.TryParse(txtMinRating.Text, out int tmpRating);
            minRating = tmpRating > 0 && tmpRating <= 10 ? tmpRating : 7;

            int yearFrom = 2000;
            int.TryParse(txtYearFrom.Text, out yearFrom);

            int yearTo = 2023;
            int.TryParse(txtYearTo.Text, out yearTo);

            var movies = await GetMoviesFromTmdb(genreId, yearFrom, yearTo, minRating);

            if (movies.Count > 0)
            {
                gvMovies.DataSource = movies;
                gvMovies.DataBind();
                gvMovies.Visible = true;
                btnSaveJson.Visible = true;
                Session["Movies"] = movies;
            }
        }

        private async Task<List<Movie>> GetMoviesFromTmdb(int genreId, int yearFrom, int yearTo, double minRating)
        {
            List<Movie> result = new List<Movie>();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.themoviedb.org/3/");
                // Получаем фильмы жанра с фильтрами по году
                string url = $"discover/movie?api_key={TMDB_API_KEY}&with_genres={genreId}&primary_release_date.gte={yearFrom}-01-01&primary_release_date.lte={yearTo}-12-31&vote_average.gte={minRating}&sort_by=popularity.desc";

                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var tmdbResponse = JsonConvert.DeserializeObject<TmdbResponse>(json);

                    foreach (var item in tmdbResponse.Results)
                    {
                        // Запрос OMDb для расширенной информации
                        var movie = new Movie
                        {
                            Title = item.Title,
                            Year = item.ReleaseDate?.Split('-')[0] ?? "",
                            Rating = item.VoteAverage,
                            Poster = $"https://image.tmdb.org/t/p/w200{item.PosterPath}",
                            Trailer = await GetTrailerUrl(item.Id)
                        };
                        result.Add(movie);
                    }
                }
            }
            return result;
        }

        private async Task<string> GetTrailerUrl(int movieId)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.themoviedb.org/3/");
                string url = $"movie/{movieId}/videos?api_key={TMDB_API_KEY}&language=ru-RU";
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var videoResponse = JsonConvert.DeserializeObject<VideoResponse>(json);
                    foreach (var video in videoResponse.Results)
                    {
                        if (video.Type == "Trailer" && video.Site == "YouTube")
                        {
                            return $"https://www.youtube.com/watch?v={video.Key}";
                        }
                    }
                }
            }
            return "";
        }

        protected void btnSaveJson_Click(object sender, EventArgs e)
        {
            if (Session["Movies"] is List<Movie> movies)
            {
                string json = JsonConvert.SerializeObject(movies, Formatting.Indented);
                Response.Clear();
                Response.ContentType = "application/json";
                Response.AddHeader("Content-Disposition", "attachment; filename=movie_collection.json");
                Response.Write(json);
                Response.End();
            }
        }
    }
    public class TmdbResponse
    {
        [JsonProperty("results")]
        public List<TmdbMovie> Results { get; set; }
    }

    public class TmdbMovie
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }
        [JsonProperty("vote_average")]
        public double VoteAverage { get; set; }
        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }
    }

    public class VideoResponse
    {
        [JsonProperty("results")]
        public List<Video> Results { get; set; }
    }

    public class Video
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("site")]
        public string Site { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
    public class Movie
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public double Rating { get; set; }
        public string Poster { get; set; }
        public string Trailer { get; set; }
    }
}
