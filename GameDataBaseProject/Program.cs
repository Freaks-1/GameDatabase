using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using GameDataBaseProject.Conversion;
using GameDataBaseProject.Models;
using GameDataBaseProject.Data;
using Microsoft.EntityFrameworkCore;
namespace GameDataBaseProject
{
    public class Program
    {

        
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using(var scope = host.Services.CreateScope())
            {

                var services = scope.ServiceProvider;
                
            }
        }
        public async void init(IServiceProvider serviceProvider)
        {
            using (var context = new GameDataBaseContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<GameDataBaseContext>>()))
            {
            if(context.Games.Any())
            return;
            var gameclient = new HttpClient();
            gameclient.BaseAddress = new Uri("https://api-v3.igdb.com/");
            gameclient.DefaultRequestHeaders.Add("user-key", "4758491416a44e996757b425c4f4377f");
            var content = new FormUrlEncodedContent(new[]
           {
                new KeyValuePair<string, string>("", "")
            });
            HttpResponseMessage result;
            int i = 0;
            do
            {
                result = await gameclient.PostAsync("games/?fields=name,first_release_date&limit=500&offset="+i, content);
                i++;
                string body = await result.Content.ReadAsStringAsync();
                GameConversion[] MovieList = JsonSerializer.Deserialize<GameConversion[]>(body);
                foreach(GameConversion game in MovieList)
                {
                    Game current_game = new Game();
                    current_game.GameID = game.id;
                    if(game.first_release_date!=0)
                    {
                        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                        current_game.Date = dtDateTime.AddSeconds(game.first_release_date).ToLocalTime();
                    }
                    current_game.name = game.name;
                    current_game.Rating = (int)game.total_rating;
                    foreach(int genre_id in game.genres)
                    {
                        BelongsTo temp_belong = new BelongsTo();
                        if((await context.Genres.FindAsync(genre_id))!=null)
                        {
                            temp_belong.GameID = game.id;
                            temp_belong.GenreID = genre_id;
                            await context.Belongings.AddAsync(temp_belong);
                            continue;
                        }
                        HttpResponseMessage genre_data = await gameclient.PostAsync("genres/id="+genre_id,content);
                        GenreConversion converted_genre = JsonSerializer.Deserialize<GenreConversion>(await genre_data.Content.ReadAsStringAsync());
                        Genre current_genre  = new Genre();
                        current_genre.GenreID = converted_genre.id;
                        current_genre.name = converted_genre.name;
                        temp_belong.GameID = game.id;
                        temp_belong.GenreID = genre_id;
                        await context.Belongings.AddAsync(temp_belong);
                        await context.Genres.AddAsync(current_genre);
                    }
                    foreach(int video in game.videos)
                    {
                        Multimedia current_multimedia = new Multimedia();
                        HttpResponseMessage video_data = await gameclient.PostAsync("videos/id="+video,content);
                        VideoConversion converted_video = JsonSerializer.Deserialize<VideoConversion>(await video_data.Content.ReadAsStringAsync());
                        current_multimedia.GameID = game.id;
                        current_multimedia.MultimediaID = video;
                        current_multimedia.URL = converted_video.url;
                        await context.Multimedias.AddAsync(current_multimedia);
                    }
                    await context.Games.AddAsync(current_game);
                    await context.SaveChangesAsync(); 
                }

            } while (result.StatusCode==System.Net.HttpStatusCode.OK);
            }
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
