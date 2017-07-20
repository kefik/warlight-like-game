using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DatabaseMapping;
using GameObjectsLib.Game;
using ProtoBuf;

namespace WinformsUI.GameSetup.Singleplayer
{
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
                var savedGames = from game in new UtilsDbContext().SingleplayerSavedGameInfos
                                 orderby game.SavedGameDate descending
                                 select game;

                // TODO: might be too slow
                foreach (var savedGame in savedGames)
                {
                    Invoke(new Action(() => loadedGamesListBox.Items.Add(savedGame)));
                }
            });
        }


        private void LoadGame(object sender, EventArgs e)
        {
            if (loadedGamesListBox.SelectedIndex < 0) return;

            var savedGameInfo =
                (SingleplayerSavedGameInfo)loadedGamesListBox.Items[loadedGamesListBox.SelectedIndex];

            try
            {
                using (var db = new UtilsDbContext())
                {
                    var game = Game.Load(db, savedGameInfo);
                    OnSingleplayerGameLoaded?.Invoke(game);
                }
                
            }
            catch (Exception)
            {
                MessageBox.Show("Selected game save has been damaged.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void DeleteGame(object sender, EventArgs e)
        {
            if (loadedGamesListBox.SelectedIndex < 0) return;
            var savedGameInfo =
                (SingleplayerSavedGameInfo)loadedGamesListBox.Items[loadedGamesListBox.SelectedIndex];

            using (var db = new UtilsDbContext())
            {
                db.Remove(savedGameInfo);
                // TODO: refresh listbox
            }
        }
    }
}
