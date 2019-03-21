
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using recipe_api.Models;

namespace recipe_api.Repositories.Interfaces
{
    public interface IRecipeRepository
    {
        Task<List<Recipe>> GetAll();
        Task<Recipe> GetByID(int id);
        Task<List<Recipe>> FindByName(string query);
        
    }
}