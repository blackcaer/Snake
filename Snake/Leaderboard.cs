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
        public ObservableCollection<PlayerScore> PlayersScores { get; private set; } = new();
        public string FileName { get; private set; }
        public int MaxPlayerCount { get; private set; }

        public Leaderboard(string fileName = "", int maxPlayerCount = 10)
        {
            SetFileName(fileName); 
            SetMaxPlayerCount(maxPlayerCount);
        }
        public void AddPlayerScore(PlayerScore playerScore)
        {
            PlayersScores.Add(playerScore);
            SortLeaderboard();
            TrimExcessScores();
        }
        private void TrimExcessScores()
        {
            while (PlayersScores.Count > MaxPlayerCount)
                PlayersScores.RemoveAt(0);
        }
        public void SetMaxPlayerCount(int maxPlayerCount)
        {
            MaxPlayerCount = (maxPlayerCount >= 1) ? maxPlayerCount : 1;
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
                PlayersScores = JsonConvert.DeserializeObject<ObservableCollection<PlayerScore>>(fileContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while loading Leaderboard: {ex.Message}");
            }
            TrimExcessScores();
            SortLeaderboard();
        }
        public void SortLeaderboard()
        {
            var tmp = new ObservableCollection<PlayerScore>(
            PlayersScores.OrderByDescending(score => score, PlayerScore.BestScoreComparer));
            PlayersScores.Clear();
            foreach (PlayerScore score in tmp) PlayersScores.Add(score);
        }
    }
}
