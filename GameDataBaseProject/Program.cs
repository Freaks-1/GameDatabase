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
                init(services);
            }
            host.Run();
        }
        public static async void init(IServiceProvider serviceProvider)
        {
            using (var context = new GameDataBaseContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<GameDataBaseContext>>()))
            {
                context.Database.EnsureCreated();
                if (context.Games.Any())
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
                result = await gameclient.PostAsync("games/?fields=name,first_release_date,genres,videos,total_rating&limit=500&offset="+i, content);
                i++;
                string body = await result.Content.ReadAsStringAsync();
                GameConversion[] MovieList = JsonSerializer.Deserialize<GameConversion[]>(body);
                foreach(GameConversion game in MovieList)
                {
                    if((await context.Games.FindAsync(game.id))!=null)
                    continue;
                    Game current_game = new Game();
                    current_game.GameID = game.id;
                    if(game.first_release_date!=0)
                    {
                        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                        current_game.Date = dtDateTime.AddSeconds(game.first_release_date).ToLocalTime();
                    }
                    current_game.name = game.name;
                    current_game.Rating = (int)game.total_rating;
                    List<BelongsTo> belongstolist = new List<BelongsTo>();
                    List<Multimedia> multimedialist  = new List<Multimedia>();
                    List<Genre> genrelist = new List<Genre>();
                    if(game.genres!=null)
                    {
                    foreach(int genre_id in game.genres)
                    {
                        BelongsTo temp_belong = new BelongsTo();
                        if((await context.Genres.FindAsync(genre_id))!=null)
                        {
                            temp_belong.GameID = game.id;
                            temp_belong.GenreID = genre_id;
                            belongstolist.Add(temp_belong);   
                            continue;
                        }
                        HttpResponseMessage genre_data = await gameclient.PostAsync("genres/?fields=name&filter[id][eq]="+genre_id,content);
                        string temp_string = await genre_data.Content.ReadAsStringAsync();
                        GenreConversion[] converted_genre = JsonSerializer.Deserialize<GenreConversion[]>(temp_string);
                        Genre current_genre  = new Genre();
                        current_genre.GenreID = converted_genre[0].id;
                        current_genre.name = converted_genre[0].name;
                        temp_belong.GameID = game.id;
                        temp_belong.GenreID = genre_id;
                        belongstolist.Add(temp_belong);
                        genrelist.Add(current_genre);
                    }
                    }
                    if(game.videos!=null)
                    {
                    foreach(int video in game.videos)
                    {
                        Multimedia current_multimedia = new Multimedia();
                        HttpResponseMessage video_data = await gameclient.PostAsync("game_videos/?fields=video_id&filter[id][eq]="+video,content);
                        string temp_string_video =await video_data.Content.ReadAsStringAsync();
                        VideoConversion[] converted_video = JsonSerializer.Deserialize<VideoConversion[]>(temp_string_video);
                        current_multimedia.GameID = game.id;
                        current_multimedia.MultimediaID = video;
                        current_multimedia.URL = converted_video[0].video_id;
                        multimedialist.Add(current_multimedia);
                    }
                    }
                    context.Database.OpenConnection();
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Game ON");
                    await context.Games.AddAsync(current_game);
                        await context.SaveChangesAsync();
                        context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Game OFF");


                    if (multimedialist.Count > 0)
                    {
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Multimedia ON");
                            await context.Multimedias.AddRangeAsync(multimedialist);
                            await context.SaveChangesAsync();
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Multimedia OFF");
                            
                        }
                    if (genrelist.Count > 0)
                    {
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Genre ON");
                            await context.Genres.AddRangeAsync(genrelist);
                            await context.SaveChangesAsync();
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Genre OFF");
                            
                        }
                    if (belongstolist.Count > 0)
                    {
                            await context.Belongings.AddRangeAsync(belongstolist);
                            await context.SaveChangesAsync();
                            
                        }
                        context.Database.CloseConnection();
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
