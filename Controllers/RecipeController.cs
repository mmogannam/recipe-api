using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Dapper;
using recipe_api.Repositories.Interfaces;
using recipe_api.Models;

namespace recipe_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
    private readonly IRecipeRepository _recipeRepoitory;

        public RecipeController(IRecipeRepository recipeRepository)
        {
            _recipeRepoitory = recipeRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Recipe>>> Get()
        {
            return await _recipeRepoitory.GetAll();
        }

        [HttpGet("{query}")]
        [Route("findbyname/{query}")]
        public async Task<ActionResult<List<Recipe>>> FindByName(string query)
        {
            if (query != "")
            {
                return await _recipeRepoitory.FindByName(query);            
            }
            return null;
        }       

        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>>  Get(int id)
        {
            if(id > 0){
                return await _recipeRepoitory.GetByID(id);
            }
            return null;

        }

    }
}

