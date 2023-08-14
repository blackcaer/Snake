using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Snake
{
    public class Leaderboard
    {
        public List<PlayerScore> PlayersScores { get; private set; } = new();
        public string FileName { get; private set; }
        public int MaxPlayerCount { get; private set; }

        public Leaderboard(string fileName = "", int maxPlayerCount = 10)
        {

            SetFileName(fileName);
            LoadFromFile();
            SetMaxPlayerCount(maxPlayerCount);
        }
        public void AddPlayerScore(PlayerScore playerScore)
        {
            int index = (PlayersScores).BinarySearch(
                playerScore, PlayerScore.BestScoreComparer);

            if (index < 0)
                index = ~index;
            //int index = 0; //TODO repair
            PlayersScores.Insert(index, playerScore);
            TrimExcessScores();
        }
        private void TrimExcessScores()
        {
            if (PlayersScores.Count > MaxPlayerCount)
                PlayersScores.RemoveAt(PlayersScores.Count - 1);
        }
        public void SetMaxPlayerCount(int maxPlayerCount)
        {
            MaxPlayerCount = maxPlayerCount;
        }
        public void SetFileName(string fileName)
        {
            FileName = Path.ChangeExtension(fileName, "json");
        }
        public void SaveToFile()
        {
            try
            {
                string json = JsonConvert.SerializeObject(PlayersScores, Formatting.Indented);
                File.WriteAllText(FileName, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while saving Leaderboard: {ex.Message}");
            }

        }
        public void LoadFromFile()
        {
            if (!File.Exists(FileName))
                return;
            try
            {
                string fileContent = File.ReadAllText(FileName);
                PlayersScores = JsonConvert.DeserializeObject<List<PlayerScore>>(fileContent);
                //PlayersScores = JsonConvert.DeserializeObject<ObservableCollection<PlayerScore>>(fileContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while loading Leaderboard: {ex.Message}");
            }

        }
    }
}
