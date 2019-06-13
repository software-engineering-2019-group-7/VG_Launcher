﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VG_Launcher
{
    public class Game
    {
        public Game(string _name, string _path, string _image, string _hours, string _lock, string _settings)
        {
            name = _name;
            path = _path;
            image = _image;
            hours = _hours;
            parentLock = _lock;
            settings = _settings;
        }
        public Game() { }
        public string name;
        public string path;
        public string image;
        public string hours;
        public string parentLock;
        public string settings;
    }
    public class Library
    {
        public Library() { }
        public List<Game> gameList = new List<Game>();

        // functions will probably be necessary to manual set Titles, paths, etc. for a player to manually change variables
        public void startUp(Game game)
        {
            string gamePath = game.path; //Get path for game to launch
            try
            {
                using (Process myProcess = new Process())
                {
                    
                    myProcess.StartInfo.UseShellExecute = false;

                    myProcess.StartInfo.FileName = gamePath;
                    //myProcess.Start(game.settings);

                }
            }
            catch (Exception e)
            {
                //If an error occurs on start up we should probably notify the user...
                Console.WriteLine(e.Message);
            }
            return;
        } //read saved file of games on app launch
        public void setLock(Game game)
        {
            game.parentLock = "true";
            return;//set a parental lock to a game (probably will need something to remove also)
        } 
        public string isLocked(Game game)
        {
            return game.parentLock; //check and return if game has parental lock on
        } 
        public void addGame()
        {
            /*manually add a game, take in as many parameters as given, leave the rest blank. Append new game to file
                probably will call from a pop-up, path & title will be necessary at minimum*/
            //Add the new game object to list, re-serialize the list of games into new json file
        }
        public void InitLib()
        {
            //read json file
            //for each, create a new game and push it to the list
            JsonSerializer serializer = new JsonSerializer();
            //path for JSON file works for VS directories.. may need adjustment for final product
            using (StreamReader r = new StreamReader(@"../../Resources/lib.JSON"))
            {
                string jGames = r.ReadToEnd();
                dynamic glib = JsonConvert.DeserializeObject(jGames);
                for (var i = 0; i < glib.Count; i++)
                {
                    dynamic inGame = glib[i];
                    Game newGame = new Game((string)inGame.name, (string)inGame.path, (string)inGame.image, (string)inGame.hours, (string)inGame.parentLock, (string)inGame.settings);
                    gameList.Add(newGame);
                }
            }
        }
      
    }

}