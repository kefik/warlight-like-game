using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DatabaseMapping;
using ProtoBuf;

namespace WinformsUI.GameSetup.Singleplayer
{
    public partial class SingleplayerLoadGamesControl : UserControl
    {
        public SingleplayerLoadGamesControl()
        {
            InitializeComponent();
        }

        public event Action<GameObjectsLib.Game.Game> OnSingleplayerGameLoad;

        private void LoadControl(object sender, System.EventArgs e)
        {
            Task.Run(() =>
            {
                var savedGames = from game in new UtilsDbContext().SavedGameInfos
                                 orderby game.SavedGameDate descending
                                 select game;

                // TODO: might be too slow
                foreach (var savedGame in savedGames)
                {
                    this.Invoke(new Action(() => loadedGamesListBox.Items.Add(savedGame)));
                }
            });
        }
        

        private void LoadGame(object sender, EventArgs e)
        {
            var savedGameInfo =
                (SingleplayerSavedGameInfo)loadedGamesListBox.Items[loadedGamesListBox.SelectedIndex];

            var fs = new FileStream(savedGameInfo.Path, FileMode.Open);
            GameObjectsLib.Game.Game game = Serializer.Deserialize<GameObjectsLib.Game.Game>(fs);
            fs.Close();

            OnSingleplayerGameLoad?.Invoke(game);
        }
    }
}
