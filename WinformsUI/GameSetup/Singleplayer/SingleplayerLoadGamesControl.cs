namespace WinformsUI.GameSetup.Singleplayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Client.Entities;
    using GameObjectsLib.Game;

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
                using (UtilsDbContext db = new UtilsDbContext())
                {
                    IOrderedQueryable<SingleplayerSavedGameInfo> savedGames = from game in db.SingleplayerSavedGameInfos
                                                                              orderby game.SavedGameDate descending
                                                                              select game;

                    foreach (SingleplayerSavedGameInfo savedGame in savedGames)
                    {
                        Invoke(new Action(() => loadedGamesListBox.Items.Add(savedGame)));
                    }
                }
            });
        }


        private void LoadGame(object sender, EventArgs e)
        {
            if (loadedGamesListBox.SelectedIndex < 0)
            {
                return;
            }

            SingleplayerSavedGameInfo savedGameInfo =
                (SingleplayerSavedGameInfo) loadedGamesListBox.Items[loadedGamesListBox.SelectedIndex];
#if (!DEBUG)
            try
            {
#endif
                using (UtilsDbContext db = new UtilsDbContext())
                {
                    Game game = Game.Load(db, savedGameInfo);
                    OnSingleplayerGameLoaded?.Invoke(game);
                }
#if (!DEBUG)
            }
            catch (Exception)
            {
                MessageBox.Show("Selected game save has been damaged.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
#endif
        }

        private void DeleteGame(object sender, EventArgs e)
        {
            if (loadedGamesListBox.SelectedIndex < 0)
            {
                return;
            }

            deleteButton.Enabled = false;
            List<SingleplayerSavedGameInfo> selectedFiles =
                loadedGamesListBox.SelectedItems.Cast<SingleplayerSavedGameInfo>().ToList();

            for (int i = selectedFiles.Count - 1; i >= 0; i--)
            {
                int selectedIndex = loadedGamesListBox.SelectedIndices[i];
                loadedGamesListBox.Items.RemoveAt(selectedIndex);
            }


            using (UtilsDbContext db = new UtilsDbContext())
            {
                foreach (SingleplayerSavedGameInfo savedGameInfo in selectedFiles)
                {
                    db.Remove(savedGameInfo);
                }
            }
            deleteButton.Enabled = true;
        }
    }
}
