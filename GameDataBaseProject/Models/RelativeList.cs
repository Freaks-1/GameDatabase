using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameDataBaseProject.Models;
using Microsoft.EntityFrameworkCore.Storage;
using GameDataBaseProject.Data;

namespace GameDataBaseProject.Models
{
    public class RelativeList
    {
        public Dictionary<int, int> relativelist;
        public List<KeyValuePair<int, int>> sortedlist;
        public void sort()
        {
            sortedlist = relativelist.ToList();
            sortedlist.Sort(compare);
        }

        public int compare(RelativeListElement a,RelativeListElement b)
        {
            return a.Score < b.Score ? -1 : a.Score > b.Score ? 1 : 0;
        }

        public List<Game> ToList(GameDataBaseContext context)
        {
            List<Game> list = new List<Game>();
            foreach(KeyValuePair<int,int> element in sortedlist)
            {
                list.Add(context.Games.Find(element.Key));
            }
            return list;
        }

        public int compare(KeyValuePair<int,int> a,KeyValuePair<int,int> b)
        {
            return a.Value < b.Value ? -1 : a.Value == b.Value ? 0 : 1;
        }
    }
}
