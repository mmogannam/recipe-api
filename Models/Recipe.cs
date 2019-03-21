using System.Collections.Generic;

namespace recipe_api.Models{
    public class Recipe
    {
        public int Id {get; set;}
        public int IngredientsGroupId{get; set;}
        public List<Ingredients> Ingredients {get; set;}
        public Category Category {get; set;}
        public int CategoryId{get;set;}
        public string Title{get;set;}
        public string Instructions{get;set;}
        public int CookTimeMinutes{get;set;}
        public int PrepTimeMinutes{get;set;}

    }

}
