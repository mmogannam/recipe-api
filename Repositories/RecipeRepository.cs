using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using Dapper;
using recipe_api.Repositories.Interfaces;
using recipe_api.Models;

namespace recipe_api.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly IConfiguration _config;

        public RecipeRepository(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("RecipeDatabase"));
            }
        }


        public async Task<List<Recipe>> GetAll()
        {
            var sql = @"SELECT * FROM Recipe r WITH (NOLOCK)    
                        INNER JOIN Category c WITH (NOLOCK) on r.categoryId = c.id";

            IEnumerable<Recipe> recipes;
            using (IDbConnection conn = Connection)
            {
                conn.Open();
                recipes = await conn.QueryAsync<Recipe, Category, Recipe>(sql, (recipe, category) => 
                {
                    recipe.Category = category; 
                    return recipe;
                    });
            }

            var sqlIngredients = "SELECT * FROM Ingredients WITH (NOLOCK)";

            IEnumerable<Ingredients> ingredients;
            using (IDbConnection conn = Connection)
            {
                conn.Open();
                ingredients = await conn.QueryAsync<Ingredients>(sqlIngredients);
            }
            //add ingredients for each recipe
            foreach(var r in recipes){
                r.Ingredients = ingredients.Where(i => i.GroupId == r.IngredientsGroupId).ToList<Ingredients>();
            }

            return recipes.ToList();

        }

        //able to get ingredients for one recipe
        public async Task<Recipe> GetByID(int id)
        {
            var sql = @"SELECT * FROM Recipe r WITH (NOLOCK)
                        WHERE r.id = @id
                        SELECT * FROM Ingredients i WITH (NOLOCK)";

            IEnumerable<Recipe> recipe;
            IEnumerable<Ingredients> ingredients;
            using (IDbConnection conn = Connection)
            {
                using (var multi = await conn.QueryMultipleAsync(sql, new{id = id} ))
                {
                    recipe  = multi.Read<Recipe>();
                    ingredients = multi.Read<Ingredients>().ToList();
                    //use linq to get the list based on the id from the first query
                    recipe.First().Ingredients = ingredients.Where(i => i.GroupId == recipe.First().IngredientsGroupId ).ToList();
                }
            }

            return recipe.First();
        }

        // find a list of recipes by name
        public async Task<List<Recipe>> FindByName(string query)
        {
            var sql = @"SELECT * FROM Recipe r WITH (NOLOCK)
                        INNER JOIN Category c WITH (NOLOCK) on r.categoryId = c.id 
                        WHERE r.title like @query";

            IEnumerable<Recipe> recipes;
            using (IDbConnection conn = Connection)
            {
                conn.Open();
                recipes = await conn.QueryAsync<Recipe, Category, Recipe>(sql, (recipe, category) => 
                {
                    recipe.Category = category; 
                    return recipe;
                    
                }, new{query = '%' + query + '%'});
            }

            var sqlIngredients = "SELECT * FROM Ingredients WITH (NOLOCK)";

            IEnumerable<Ingredients> ingredients;
            using (IDbConnection conn = Connection)
            {
                conn.Open();
                ingredients = await conn.QueryAsync<Ingredients>(sqlIngredients);
            }
            //add ingredients for each recipe
            foreach(var r in recipes){
                r.Ingredients = ingredients.Where(i => i.GroupId == r.IngredientsGroupId).ToList<Ingredients>();
            }

            return recipes.ToList();

        }       

    }
}