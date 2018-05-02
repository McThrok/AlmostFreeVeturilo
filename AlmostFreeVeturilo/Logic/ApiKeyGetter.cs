﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.OData;

namespace AFVTry.Logic
{
    public class ApiKeyGetter
    {
        private static ApiKeyGetter _instance;
        public static ApiKeyGetter Instance => _instance ?? (_instance = new ApiKeyGetter());
        private ApiKeyGetter() { }

        private int _index = 0;
        private readonly string[] _apiKeys =
        {
            "AIzaSyDdEzYrifjID-xf3EdiB6eNLjpKmYhL-aA",
            "AIzaSyCVTPUCiobBZPW7fe6HQAXpWTMicf-KKIU",
            "AIzaSyDERBopK-6fwHLZdFKblglgu61iyE3e-Ww",
            "AIzaSyAAzOsModWJOhH60xCeXAgqcjloprcNYHE",
            "AIzaSyBaDNUqd96CYOhwAJtHV5sebIY7cMJuMII",
            "AIzaSyDyBpjEq5eGWHSgJOdvYlCuE4NL8SN0KOc",
            "AIzaSyAHTbkqz9omjIFo2JRlZiR8Rgs__1XbtGY",
            "AIzaSyBGtTdHab53o5HgbKwTpfv-uFRNI5VHGhA",
            "AIzaSyAdA3mvtHA95RuLbWmotiBrqUingimV5uc",
            "AIzaSyAF8nScnb7MXplU3MJQYB6e5SYz9lZ8LUI",
        };
        
        public string GetKey()
        {
            return _apiKeys[(++_index) % _apiKeys.Length];
        }
    }
}
