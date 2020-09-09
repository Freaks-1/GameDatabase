using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameDataBaseProject.Models;
using GameDataBaseProject.Data;
namespace GameDataBaseProject.Controllers
{
    [Route("api/Game")]
    [ApiController]
    public class InterfaceController : ControllerBase
    {
        private readonly GameDataBaseContext _context;

        public InterfaceController(GameDataBaseContext context)
        {
            _context = context;
        }
        [Route("genre")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genre>>> GetGameGenre()
        {
            return await _context.Genres
                .ToListAsync();
        }
        // GET: api/Interface
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetGame()
        {
            var games = from m in _context.Games
                        select m;
            return add_all(games).ToList();
        }

        // GET: api/Interface/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetGame(int id)
        {
            var game = await _context.Games.FindAsync(id);

            if (game == null)
            {
                return NotFound();
            }

            return game;
        }
        [Route("similar/")]
        [HttpGet("{id}")]

        public  async Task<ActionResult<IEnumerable<Game>>> SimilarGame(int id)
        {
            List<int> genre_list = new List<int>();
            var belong_list = _context.Belongings.Where(belong => belong.GameID==id);
            Dictionary<int, int> relativelist = new Dictionary<int, int>();
            var remaining_result = from Game a in _context.Games
                                   select a;
            //List<Game> Result = await remaining_result.ToListAsync();
            //Result = await add_all(Result).ToListAsync();
            foreach(var bel in belong_list)
            {
                remaining_result = from rem in remaining_result
                                   where rem.BelongToGenre.Contains(bel)
                                   select rem;
            }
            /*foreach(BelongsTo bel in belong_list)
            {
                foreach(Game resu in Result)
                {
                    if (!has_genre(bel.GenreID, resu))
                        Result.Remove(resu);
                }
            }
            /*RelativeList relativeList = new RelativeList();
            relativeList.relativelist = relativelist;
            relativeList.sort();
            List<Game> result = relativeList.ToList(_context);
            var res = from Game ress in result
                      select ress;
            return await add_all(res.ToList()).ToListAsync();
            */
            return await add_all(remaining_result).ToListAsync();
        }
        [HttpGet("query/{searchstring}/{orderby?}/{genre?}")]
        public async Task<ActionResult<IEnumerable<Game>>> QueryGame(string searchstring,string? orderby,string? genre)
        {
            
            var games = from m in _context.Games
                 select m;
            if(searchstring!=null)
            games = games.Where(w => w.name.Contains(searchstring));

            if(orderby == "Date")
            {
                games = from g in games
                    orderby g.Date 
                    select g;
            }
            else if(orderby == "Rating")
            {
                games = from g in games
                    orderby g.Rating 
                    select g;
            }
            if(genre!=null)
            {
                String[] filter_genrelist = genre.Split(',');
                foreach(string genrelistelement in filter_genrelist)
                {
                    int id = ((Genre) _context.Genres.Where(element => element.name == genrelistelement)).GenreID;
                    games = from ga in games
                            where has_genre(id,ga)
                            select ga;
                }
            }
            
            return await add_all(games).ToListAsync();
        }
        private IQueryable<Game> add_all(IQueryable<Game> games)
        {
            foreach (Game gam in games)
            {
                var multimedia_list = _context.Multimedias.Where(mult => mult.GameID == gam.GameID);
                gam.Multimedias = multimedia_list.ToList();
            }
            foreach (Game gam_bel in games)
            {
                var bel_list = _context.Belongings.Where(bel => bel.GameID == gam_bel.GameID);
                gam_bel.BelongToGenre = bel_list.ToList();
            }
            return games;
        }
        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.GameID == id);
        }

        private bool has_genre(int genreid,Game game)
        {
            foreach(BelongsTo bel in game.BelongToGenre)
            {
                if (bel.GenreID == genreid)
                    return true;
            }
            return false;
        }
    }
}
