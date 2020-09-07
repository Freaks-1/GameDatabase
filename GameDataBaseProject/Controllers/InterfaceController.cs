using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameDataBaseProject.Models;

namespace GameDataBaseProject.Controllers
{
    [Route("api/Game")]
    [ApiController]
    public class InterfaceController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public InterfaceController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Interface
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
        [HttpGet("{searchstring,orderby,genre}")]
        public async Task<ActionResult<Game>> GetGame(string searchstring = null,string orderby = null,string genre=null)
        {
            var games = from m in _context.Games
                 select m;
            if(searchstring!=null)
            games = games.Where(w => w.name.Contains(searchstring));
            if(orderby == "Date")
            {games = from g in games
                    orderby g.Date 
                    select g;}
            else if(orderby == "Rating")
            {games = from g in games
                    orderby g.Rating 
                    select g;}
            if(genre!=null)
            {
                List<String> filter_genrelist = genre.Split(',');
                foreach(string genrelistelement in filter_genrelist)
                {
                    int id = await _context.Genre.Where(element => element.name == genrelistelement);
                    games = from ga in games
                            where has_genre(id,ga)
                            select ga;
                }
            }
            return games;
        }
        private bool GameExists(int id)
        {
            return _context.Game.Any(e => e.GameID == id);
        }

        private bool has_genre(int genreid,Game game)
        {
            foreach(BelongsTo belong in game.BelongToGenre)
            {
                if(belong.GenreID == genreid)
                return true;
            }
            return false;
        }
    }
}
