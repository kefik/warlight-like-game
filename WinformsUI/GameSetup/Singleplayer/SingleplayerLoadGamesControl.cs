using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameObjectsLib.Game;
using ProtoBuf;

namespace WinformsUI.GameSetup.Singleplayer
{
    using System.Collections.Generic;

    public partial class SingleplayerLoadGamesControl : UserControl
    {
        public SingleplayerLoadGamesControl()
        {
            InitializeComponent();
        }

        public event Action<Game> OnSingleplayerGameLoaded;

        private void LoadControl(object sender, EventArgs e)
        {
            RefreshSavedGames();
        }

        public void RefreshSavedGames()
        {
            Task.Run(() =>
            {
                using (var db = new UtilsDbContext())
                {
                    var savedGames = from game in db.SingleplayerSavedGameInfos
                                     orderby game.SavedGameDate descending
                                     select game;
                    
                    foreach (var savedGame in savedGames)
                    {
                        Invoke(new Action(() => loadedGamesListBox.Items.Add(savedGame)));
                    }
                }
            });
        }


        private void LoadGame(object sender, EventArgs e)
        {
            if (loadedGamesListBox.SelectedIndex < 0) return;

            var savedGameInfo =
                (SingleplayerSavedGameInfo)loadedGamesListBox.Items[loadedGamesListBox.SelectedIndex];
#if (!DEBUG)
            try
            {
#endif
                using (var db = new UtilsDbContext())
                {
                    var game = Game.Load(db, savedGameInfo);
                    OnSingleplayerGameLoaded?.Invoke(game);
                }
#if (!DEBUG)
        }
            catch (Exception)
            {
                MessageBox.Show("Selected game save has been damaged.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
#endif
        }

        private void DeleteGame(object sender, EventArgs e)
        {
            if (loadedGamesListBox.SelectedIndex < 0) return;
            var selectedFiles = loadedGamesListBox.SelectedItems.Cast<SingleplayerSavedGameInfo>().ToList();
            for(int i = 0; i < selectedFiles.Count; i++)
            {
                loadedGamesListBox.Items.RemoveAt(i);
            }

            using (var db = new UtilsDbContext())
            {
                foreach (var savedGameInfo in selectedFiles)
                {
                    db.Remove(savedGameInfo);
                }
            }
        }
    }
}
