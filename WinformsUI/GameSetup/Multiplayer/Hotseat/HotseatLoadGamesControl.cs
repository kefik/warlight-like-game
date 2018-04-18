namespace WinformsUI.GameSetup.Multiplayer.Hotseat
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Client.Entities;
    using GameObjectsLib.Game;
    using Helpers;

    public partial class HotseatLoadGamesControl : UserControl
    {
        public HotseatLoadGamesControl()
        {
            InitializeComponent();
        }

        public event Action<Game> OnHotseatGameLoaded;

        public void RefreshSavedGames()
        {
            Task.Run(() =>
            {
                using (UtilsDbContext db = new UtilsDbContext())
                {
                    var savedGames = (from game in db.HotseatSavedGameInfos.AsParallel()
                                     orderby game.SavedGameDate descending
                                     select game).ToList();

                    // TODO: might be too slow
                    foreach (HotseatSavedGameInfo savedGame in savedGames)
                    {
                        loadedGamesListBox.InvokeIfRequired(() => loadedGamesListBox.Items.Add(savedGame));
                    }
                    // TODO: data binding
                }
            });
        }

        private void LoadGame(object sender, EventArgs e)
        {
            if (loadedGamesListBox.SelectedIndex < 0)
            {
                return;
            }

            HotseatSavedGameInfo savedGameInfo =
                (HotseatSavedGameInfo)loadedGamesListBox.Items[loadedGamesListBox.SelectedIndex];
#if (!DEBUG)
            try
            {
#endif
            using (UtilsDbContext db = new UtilsDbContext())
            {
                Game game = Game.Load(db, savedGameInfo);
                OnHotseatGameLoaded?.Invoke(game);
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
            var selectedFiles = loadedGamesListBox.SelectedItems.Cast<HotseatSavedGameInfo>().ToList();

            for (int i = selectedFiles.Count - 1; i >= 0; i--)
            {
                int selectedIndex = loadedGamesListBox.SelectedIndices[i];
                loadedGamesListBox.Items.RemoveAt(selectedIndex);
            }

            using (UtilsDbContext db = new UtilsDbContext())
            {
                foreach (HotseatSavedGameInfo savedGameInfo in selectedFiles)
                {
                    db.Remove(savedGameInfo);
                }
            }
            deleteButton.Enabled = true;
        }

        private void FormLoad(object sender, EventArgs e)
        {
            RefreshSavedGames();
        }
    }
}
