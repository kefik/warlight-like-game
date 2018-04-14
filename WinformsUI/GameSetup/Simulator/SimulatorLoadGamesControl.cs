namespace WinformsUI.GameSetup.Simulator
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Client.Entities;
    using GameObjectsLib.Game;

    public partial class SimulatorLoadGamesControl : UserControl
    {
        public SimulatorLoadGamesControl()
        {
            InitializeComponent();
        }

        public event Action<Game> OnSimulationLoaded;

        public void RefreshSavedGames()
        {
            Task.Run(() =>
            {
                using (UtilsDbContext db = new UtilsDbContext())
                {
                    var savedGames = (from game in db.SimulationRecords.AsParallel()
                                     orderby game.SavedGameDate descending
                                     select game).ToList();

                    // TODO: might be too slow
                    foreach (var savedGame in savedGames)
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

            var savedGameInfo =
                (SimulationRecord)loadedGamesListBox.Items[loadedGamesListBox.SelectedIndex];
#if (!DEBUG)
            try
            {
#endif
            using (UtilsDbContext db = new UtilsDbContext())
            {
                Game game = Game.Load(db, savedGameInfo);
                OnSimulationLoaded?.Invoke(game);
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
            var selectedFiles = loadedGamesListBox.SelectedItems.Cast<SimulationRecord>().ToList();

            for (int i = selectedFiles.Count - 1; i >= 0; i--)
            {
                int selectedIndex = loadedGamesListBox.SelectedIndices[i];
                loadedGamesListBox.Items.RemoveAt(selectedIndex);
            }

            using (UtilsDbContext db = new UtilsDbContext())
            {
                foreach (SimulationRecord savedGameInfo in selectedFiles)
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
