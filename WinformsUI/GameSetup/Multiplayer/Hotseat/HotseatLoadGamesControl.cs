namespace WinformsUI.GameSetup.Multiplayer.Hotseat
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Client.Entities;
    using GameObjectsLib.Game;

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
                    IOrderedQueryable<HotseatSavedGameInfo> savedGames = from game in db.HotseatSavedGameInfos
                                                                         orderby game.SavedGameDate descending
                                                                         select game;

                    // TODO: might be too slow
                    foreach (HotseatSavedGameInfo savedGame in savedGames)
                    {
                        Invoke(new Action(() => loadedGamesListBox.Items.Add(savedGame)));
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
                (HotseatSavedGameInfo) loadedGamesListBox.Items[loadedGamesListBox.SelectedIndex];
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
            List<HotseatSavedGameInfo> selectedFiles =
                loadedGamesListBox.SelectedItems.Cast<HotseatSavedGameInfo>().ToList();
            for (int i = 0; i < selectedFiles.Count; i++)
            {
                loadedGamesListBox.Items.RemoveAt(i);
            }

            using (UtilsDbContext db = new UtilsDbContext())
            {
                foreach (HotseatSavedGameInfo savedGameInfo in selectedFiles)
                {
                    db.Remove(savedGameInfo);
                }
            }
        }

        private void FormLoad(object sender, EventArgs e)
        {
            RefreshSavedGames();
        }
    }
}
