using System;

public struct Game
{
    public String Title;
    public String Path;
    public int Hours;
    public String Image; //set to a stock image to start.
    public Boolean Parent_Lock;
    //settings?
	public Game(String title, String path)//probably should take all variables.. leave blank if not there
	{
        Title = title;
        Path = path;
	}
}
public class Library
{
    //list to hold game objects
    public Library()//list of games & paths as parameters
    {
        //for each item in list, add Game object to list
    }
    // functions will probably be necessary to manual set Titles, paths, etc. for a player to manually change variables
    public void startUp(Game game); //read saved file of games on app launch
    public void setLock(Game game); //set a parental lock to a game (probably will need something to remove also)
    public Boolean isLocked(Game game); //check and return if game has parental lock on
    public void addGame();/*manually add a game, take in as many parameters as given, leave the rest blank. Append new game to file
        probably will call from a pop-up, path & title will be necessary at minimum*/


}
public void Library.startUp(Game game)
{
    gamePath = game.Path; //Get path for game to launch
    try
    {
        using (Process myProcess = new Process())
        {
            //
            myProcess.StartInfo.UseShellExecute = false;
            
            myProcess.StartInfo.FileName = gamePath;
            myProcess.Start();
            
        }
    }
    catch (Exception e)
    {
        //If an error occurs on start up we should probably notify the user...
        Console.WriteLine(e.Message);
    }
    return;
}
public void Library.setLock(Game game)
{
    game.Parent_Lock = true;
    return;
}
public Boolean Library.isLocked(Game game)
{
    return game.Parent_Lock;
}