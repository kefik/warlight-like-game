﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DatabaseMapping;
using GameObjectsLib.Game;

namespace WinformsUI.GameSetup.Multiplayer.Hotseat
{
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
                using (var db = new UtilsDbContext())
                {
                    var savedGames = from game in db.HotseatSavedGameInfos
                                     orderby game.SavedGameDate descending
                                     select game;

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
            if (loadedGamesListBox.SelectedIndex < 0) return;

            var savedGameInfo =
                (HotseatSavedGameInfo)loadedGamesListBox.Items[loadedGamesListBox.SelectedIndex];

            try
            {
                using (var db = new UtilsDbContext())
                {
                    var game = Game.Load(db, savedGameInfo);
                    OnHotseatGameLoaded?.Invoke(game);
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Selected game save has been damaged.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        private void DeleteGame(object sender, EventArgs e)
        {

        }

        private void FormLoad(object sender, EventArgs e)
        {
            RefreshSavedGames();
        }
    }
}
