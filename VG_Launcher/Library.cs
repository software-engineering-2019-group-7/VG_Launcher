using System;
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
        public Game(string _name, string _path, string _image, int _time, string _lock, string _settings)
        {
            //constructor without provider, will be used when user manually adds a game
            name = _name;
            path = _path;
            image = _image;
            time = _time;
            parentLock = _lock;
            settings = _settings;
            provider = null;
        }
        public Game(string _name, string _path, string _image, int _time, string _lock, string _settings, string _provider)
        {
            //constructor with provider, designating a provider allows us to use that launcher's specific launch commands
            name = _name;
            path = _path;
            image = _image;
            time = _time;
            parentLock = _lock;
            settings = _settings;
            provider = _provider;
        }
        public Game() { }
        public string name;
        public string path;
        public string image;
        public int time;
        public string parentLock;
        public string settings;
        public string provider;

        public void setSettings(string newSet)
        {
            settings = newSet;
        }
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
        public void addGame(Game g)
        {
            /*manually add a game, take in as many parameters as given, leave the rest blank. Path & title
             * will be necessary at minimum, maybe include a default picture for if the user doesn't specify?*/

            //Add the new game object to list, re-serialize the list of games into new json file
            gameList.Add(g);
        }
        public void InitLib()
        {
            //read json file
            //for each, create a new game and push it to the list
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Include;
            //path for JSON file works for VS directories.. may need adjustment for final product
            using (StreamReader r = new StreamReader(@"../../Resources/lib.JSON"))
            {
                string jGames = r.ReadToEnd();
                dynamic glib = JsonConvert.DeserializeObject(jGames);
                if (glib != null){ 
                    for (var i = 0; i < glib.Count; i++)
                    {
                        dynamic inGame = glib[i];
                        Game newGame = new Game((string)inGame.name, (string)inGame.path, (string)inGame.image, (int)inGame.time, (string)inGame.parentLock, (string)inGame.settings, (string)inGame.provider);
                        gameList.Add(newGame);
                    }
                }
                else
                {
                    //No saved Library TODO: this would probably be where we call initial startup prompt
                    return;
                }
            }
        }

        public void SaveJson(Library lib)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Include;
            serializer.Formatting = Formatting.Indented;
            
            using (StreamWriter sw = new StreamWriter(@"../../Resources/lib.JSON"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                    serializer.Serialize(writer, lib.gameList);
            }
        }

        public void updateSettings(Game game, string newSet)
        {
            foreach (Game g in gameList)
            {
                if (game == g)
                {
                    g.setSettings(newSet);
                }
            }
        }
    }

}
